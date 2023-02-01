using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab; // 인스턴스화할 타일
    public Transform obstaclePrefab; // 장애물 프리팹
    public Vector2 mapSize;

    [Range(0,1)] // outlinePercent의 범위를 0~1로 한정
    public float outlinePercent;

    [Range(0, 1)] // outlinePercent의 범위를 0~1로 한정
    public float obstaclePercent;

    List<Coord> allTileCoords; // 모든 타일 좌표에 대한 리스트
    Queue<Coord> shuffledTileCoords;// 모든 좌표들에 대한 큐

    public int seed = 10;
    Coord mapCentre;

    void Start()
    {
        GeneratorMap();
    }

    public void GeneratorMap()
    {
        allTileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));

        //GenerateMap으로 맵을 생성할 때 먼저 해야 할 일은 맵의 정중앙을 지정
        mapCentre = new Coord((int)mapSize.x / 2, (int)mapSize.y / 2);

        // 모든 타일들을 자식으로 가지고 있을 오브젝트
        string holderName = "Generated Map";
        // holderName 아래에 자식이 존재한다면 파괴
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                // 맵에서 좌측 상단 모서리을 시작으로 만들어가는 것
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right*90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent); // 테두리 크기만큼 타일의 크기를 줄여서 할당
                newTile.parent = mapHolder;
            }
        }

        bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];

        int obstacleCount = (int)(mapSize.x * mapSize.y* obstaclePercent);
        int currentObstacleCount = 0;

        for (int i= 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);

                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }
    }

    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        // 먼저 여기에 정중앙 타일을 넣는걸로 시작
        queue.Enqueue(mapCentre);
        mapFlags[mapCentre.x, mapCentre.y] = true;

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
        int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;

    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
    }

    // 큐로 부터 다음 아이템을 얻어 랜덤 좌표를 반환
    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);

        return randomCoord;
    }


    public struct Coord
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
    }
}
