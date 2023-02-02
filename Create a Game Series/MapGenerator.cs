using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public Map[] maps;
    public int mapIndex;

    public Transform tilePrefab; // 인스턴스화할 타일
    public Transform obstaclePrefab; // 장애물 프리팹
    public Transform navmeshFloor;
    public Transform navmeshMaskPrefab;
    public Vector2 maxMapSize;

    [Range(0,1)] // outlinePercent의 범위를 0~1로 한정
    public float outlinePercent;

    public float tileSize;

    List<Coord> allTileCoords; // 모든 타일 좌표에 대한 리스트
    Queue<Coord> shuffledTileCoords;// 모든 좌표들에 대한 큐, 랜덤으로 맵 상의 어떤 타일의 좌표를 가져오기 위해 만듦.
    Queue<Coord> shuffledOpenTileCoords; // 좌표 큐, 랜덤 맵 상의 장애물 없는 타일의 좌표를 가져오기 위해 만듦.

    Transform[,] tileMap; // 우리가 생성했던 모든 타일들 저장

    Map currentMap;

    void Start()
    {
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }
    
    void OnNewWave(int waveNumber)
    {
        mapIndex = waveNumber - 1;
    }

    public void GenerateMap()
    {
        currentMap = maps[mapIndex];
        tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
        System.Random prng = new System.Random(currentMap.seed);
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, 0.05f, currentMap.mapSize.y * tileSize);

        // ==================== 좌표(Coord) 생성 ====================
        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));

        // ==================== map holder 오브젝트 생성 ====================
        string holderName = "Generated Map"; // 모든 타일들을 자식으로 가지고 있을 오브젝트
        // holderName 아래에 자식이 존재한다면 파괴
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        // ==================== 타일 생성 ====================
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                // 맵에서 좌측 상단 모서리을 시작으로 만들어가는 것
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right*90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize; // 테두리 크기만큼 타일의 크기를 줄여서 할당
                newTile.parent = mapHolder;
                tileMap[x, y] = newTile;
            }
        }

        // ==================== 장애물 생성 ====================
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y* currentMap.obstaclePercent);
        int currentObstacleCount = 0;
        List<Coord> allOpenCoords = new List<Coord>(allTileCoords); // 일단, 모든 타일의 좌표를 복사해 가져와 초기화

        for (int i= 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != currentMap.mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                // 장애물의 높이를 minObstacleHeight~maxObstacleHeight 사이로 설정, 랜덤 퍼센트 값
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());

                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);

                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight/2, Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);

                // 전면 컬러와 후면 컬러 사이를 보간
                float colourPercent = randomCoord.y / (float)currentMap.mapSize.y; // 두 값이 정수라서 그냥 나누면 0이 나옴. 얼마나 앞쪽에 위치했는지 확인
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColour, currentMap.backgroundColour, colourPercent); // Color의 보간 함수 Color.Lerp()
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                allOpenCoords.Remove(randomCoord);
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }

        shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray(allOpenCoords.ToArray(), currentMap.seed));

        // ==================== nevmesh mask 생성 ====================
        // maskLeft
        Transform maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;// mapHolder의 자식으로 들어갈 수 있게 정리

        // 마스킹될 영역들을 실제로 덮어씌울수 있게 크기를 지정
        // x값으로는 실제 맵의 가장자리와 최대 맵 사이즈의 가장자리 사이의 거리(maxMapSize.x -mapSize.x)/ 2
        // y방향으로는 1. z로는 실제 맵의 높이 mapSize.y를 설정
        maskLeft.localScale =  new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        
        // maskRight도 Left와 동일
        Transform maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;// mapHolder의 자식으로 들어갈 수 있게 정리
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        // maskTop
        // z축을 따라 맵의 위쪽으로 이동하도록, Vector3.forward로 수정
        Transform maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;// mapHolder의 자식으로 들어갈 수 있게 정리
        // x방향에 대해 전체 영역을 덮어씌우도록 maxMapSize.x
        // mapSize.y에 대해서는 위쪽 이 부분에서 x방향으로 했던 것과 같은 처리 (maxMapSize.y - mapSize.y) / 2
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        // maskBottom
        // z축을 따라 맵의 위쪽으로 이동하도록, Vector3.forward로 수정
        Transform maskBottom = Instantiate(navmeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskBottom.parent = mapHolder;// mapHolder의 자식으로 들어갈 수 있게 정리
        maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
    }

    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        // 먼저 여기에 정중앙 타일을 넣는걸로 시작
        queue.Enqueue(currentMap.mapCentre);
        mapFlags[currentMap.mapCentre.x, currentMap.mapCentre.y] = true;

        int accessibleTileCount = 1; // mapCentre이 접근가능하다는 걸 알고 시작했으니까

        // Flood Fill 부분
        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue(); // 큐의 첫번째 아이템을 가져오고, 그것을 큐에서 제거

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;

                    // 대각선 방향은 체크하지 않게
                    if (x == 0 || y == 0)
                    {
                        // 좌표가 obstacleMap 내부에 있는지 확인
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            // 아직 검사하지 않은 이웃타일을 찾았고 그것이 장애물 타일이 아니라면
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        // 모든 것이 끝났을 때, 여기에 본래 장애물이 아닌 타일이 얼마나 존재했어야 했는지 알아야 한다.
        int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;

    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }

    public Transform GetTileFromPosition(Vector3 position)
    {
        // 정수로 x를 받을 거기 때문에 반올림 함수 Mathf.RoundToInt를 사용
        // (int)로 형변환을 하면 항상 내리기 때문에 반올림 함수를 사용함
        int x = Mathf.RoundToInt(position.x / tileSize + (currentMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / tileSize + (currentMap.mapSize.y - 1) / 2f);
        // tileMap의 범위를 벗어나는 위치를 가져오려 한다면, 인덱스 오류
        // x, y 값들을 알맞게 제한해 둬야 한다.
        x = Mathf.Clamp(x, 0, tileMap.GetLength(0) -1);
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1) -1);

        return tileMap[x, y];
    }

    // 큐로 부터 다음 아이템을 얻어 랜덤 좌표를 반환
    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);

        return randomCoord;
    }

    // 무작위로 오픈 타일(장애물x 타일) 가져오기
    public Transform GetRandomOpenTile()
    {
        Coord randomCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(randomCoord);
        return tileMap[randomCoord.x, randomCoord.y];
    }

    [System.Serializable]
    public struct Coord // 좌표
    {
        public int x;
        public int y;

        // 생성자
        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator == (Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
        /*
        public override bool Equals(object o)
        {
            return true;
        }
        public override int GetHashCode()
        {  
            return 0;
        }
        */
    }

    [System.Serializable]
    public class Map
    {
        public Coord mapSize; // 맵 크기
        

        [Range(0, 1)] // obstaclePercent의 범위를 0~1로 한정
        public float obstaclePercent;

        public int seed;
        // 장애물 높이를 랜덤화하기 위한 변수
        public float minObstacleHeight; // 장애물의 최소 높이
        public float maxObstacleHeight; // 장애물의 최대 높이

        public Color foregroundColour;
        public Color backgroundColour;

        //GenerateMap으로 맵을 생성할 때 먼저 해야 할 일은 맵의 정중앙을 지정하는 것
        public Coord mapCentre
        {
            get
            {
                return new Coord(mapSize.x / 2, mapSize.y / 2);
            }
        }

    }
}
