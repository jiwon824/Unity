using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab; // 인스턴스화할 타일
    public Vector2 mapSize;

    [Range(0,1)] // outlinePercent의 범위를 0~1로 한정
    public float outlinePercent;

    void Start()
    {
        GeneratorMap();
    }

    public void GeneratorMap()
    {

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
                Vector3 tilePosition = new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right*90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent); // 테두리 크기만큼 타일의 크기를 줄여서 할당
                newTile.parent = mapHolder;
            }
        }
    }
}
