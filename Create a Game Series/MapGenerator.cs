using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public Map[] maps;
    public int mapIndex;

    public Transform tilePrefab; // �ν��Ͻ�ȭ�� Ÿ��
    public Transform obstaclePrefab; // ��ֹ� ������
    public Transform navmeshFloor;
    public Transform navmeshMaskPrefab;
    public Vector2 maxMapSize;

    [Range(0,1)] // outlinePercent�� ������ 0~1�� ����
    public float outlinePercent;

    public float tileSize;

    List<Coord> allTileCoords; // ��� Ÿ�� ��ǥ�� ���� ����Ʈ
    Queue<Coord> shuffledTileCoords;// ��� ��ǥ�鿡 ���� ť, �������� �� ���� � Ÿ���� ��ǥ�� �������� ���� ����.
    Queue<Coord> shuffledOpenTileCoords; // ��ǥ ť, ���� �� ���� ��ֹ� ���� Ÿ���� ��ǥ�� �������� ���� ����.

    Transform[,] tileMap; // �츮�� �����ߴ� ��� Ÿ�ϵ� ����

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

        // ==================== ��ǥ(Coord) ���� ====================
        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));

        // ==================== map holder ������Ʈ ���� ====================
        string holderName = "Generated Map"; // ��� Ÿ�ϵ��� �ڽ����� ������ ���� ������Ʈ
        // holderName �Ʒ��� �ڽ��� �����Ѵٸ� �ı�
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        // ==================== Ÿ�� ���� ====================
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                // �ʿ��� ���� ��� �𼭸��� �������� ������ ��
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right*90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize; // �׵θ� ũ�⸸ŭ Ÿ���� ũ�⸦ �ٿ��� �Ҵ�
                newTile.parent = mapHolder;
                tileMap[x, y] = newTile;
            }
        }

        // ==================== ��ֹ� ���� ====================
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y* currentMap.obstaclePercent);
        int currentObstacleCount = 0;
        List<Coord> allOpenCoords = new List<Coord>(allTileCoords); // �ϴ�, ��� Ÿ���� ��ǥ�� ������ ������ �ʱ�ȭ

        for (int i= 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != currentMap.mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                // ��ֹ��� ���̸� minObstacleHeight~maxObstacleHeight ���̷� ����, ���� �ۼ�Ʈ ��
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());

                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);

                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight/2, Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);

                // ���� �÷��� �ĸ� �÷� ���̸� ����
                float colourPercent = randomCoord.y / (float)currentMap.mapSize.y; // �� ���� ������ �׳� ������ 0�� ����. �󸶳� ���ʿ� ��ġ�ߴ��� Ȯ��
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColour, currentMap.backgroundColour, colourPercent); // Color�� ���� �Լ� Color.Lerp()
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

        // ==================== nevmesh mask ���� ====================
        // maskLeft
        Transform maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;// mapHolder�� �ڽ����� �� �� �ְ� ����

        // ����ŷ�� �������� ������ ������ �ְ� ũ�⸦ ����
        // x�����δ� ���� ���� �����ڸ��� �ִ� �� �������� �����ڸ� ������ �Ÿ�(maxMapSize.x -mapSize.x)/ 2
        // y�������δ� 1. z�δ� ���� ���� ���� mapSize.y�� ����
        maskLeft.localScale =  new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        
        // maskRight�� Left�� ����
        Transform maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;// mapHolder�� �ڽ����� �� �� �ְ� ����
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        // maskTop
        // z���� ���� ���� �������� �̵��ϵ���, Vector3.forward�� ����
        Transform maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;// mapHolder�� �ڽ����� �� �� �ְ� ����
        // x���⿡ ���� ��ü ������ ����쵵�� maxMapSize.x
        // mapSize.y�� ���ؼ��� ���� �� �κп��� x�������� �ߴ� �Ͱ� ���� ó�� (maxMapSize.y - mapSize.y) / 2
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        // maskBottom
        // z���� ���� ���� �������� �̵��ϵ���, Vector3.forward�� ����
        Transform maskBottom = Instantiate(navmeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskBottom.parent = mapHolder;// mapHolder�� �ڽ����� �� �� �ְ� ����
        maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
    }

    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        // ���� ���⿡ ���߾� Ÿ���� �ִ°ɷ� ����
        queue.Enqueue(currentMap.mapCentre);
        mapFlags[currentMap.mapCentre.x, currentMap.mapCentre.y] = true;

        int accessibleTileCount = 1; // mapCentre�� ���ٰ����ϴٴ� �� �˰� ���������ϱ�

        // Flood Fill �κ�
        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue(); // ť�� ù��° �������� ��������, �װ��� ť���� ����

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;

                    // �밢�� ������ üũ���� �ʰ�
                    if (x == 0 || y == 0)
                    {
                        // ��ǥ�� obstacleMap ���ο� �ִ��� Ȯ��
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            // ���� �˻����� ���� �̿�Ÿ���� ã�Ұ� �װ��� ��ֹ� Ÿ���� �ƴ϶��
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

        // ��� ���� ������ ��, ���⿡ ���� ��ֹ��� �ƴ� Ÿ���� �󸶳� �����߾�� �ߴ��� �˾ƾ� �Ѵ�.
        int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;

    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }

    public Transform GetTileFromPosition(Vector3 position)
    {
        // ������ x�� ���� �ű� ������ �ݿø� �Լ� Mathf.RoundToInt�� ���
        // (int)�� ����ȯ�� �ϸ� �׻� ������ ������ �ݿø� �Լ��� �����
        int x = Mathf.RoundToInt(position.x / tileSize + (currentMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / tileSize + (currentMap.mapSize.y - 1) / 2f);
        // tileMap�� ������ ����� ��ġ�� �������� �Ѵٸ�, �ε��� ����
        // x, y ������ �˸°� ������ �־� �Ѵ�.
        x = Mathf.Clamp(x, 0, tileMap.GetLength(0) -1);
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1) -1);

        return tileMap[x, y];
    }

    // ť�� ���� ���� �������� ��� ���� ��ǥ�� ��ȯ
    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);

        return randomCoord;
    }

    // �������� ���� Ÿ��(��ֹ�x Ÿ��) ��������
    public Transform GetRandomOpenTile()
    {
        Coord randomCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(randomCoord);
        return tileMap[randomCoord.x, randomCoord.y];
    }

    [System.Serializable]
    public struct Coord // ��ǥ
    {
        public int x;
        public int y;

        // ������
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
        public Coord mapSize; // �� ũ��
        

        [Range(0, 1)] // obstaclePercent�� ������ 0~1�� ����
        public float obstaclePercent;

        public int seed;
        // ��ֹ� ���̸� ����ȭ�ϱ� ���� ����
        public float minObstacleHeight; // ��ֹ��� �ּ� ����
        public float maxObstacleHeight; // ��ֹ��� �ִ� ����

        public Color foregroundColour;
        public Color backgroundColour;

        //GenerateMap���� ���� ������ �� ���� �ؾ� �� ���� ���� ���߾��� �����ϴ� ��
        public Coord mapCentre
        {
            get
            {
                return new Coord(mapSize.x / 2, mapSize.y / 2);
            }
        }

    }
}
