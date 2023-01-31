using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab; // �ν��Ͻ�ȭ�� Ÿ��
    public Vector2 mapSize;

    [Range(0,1)] // outlinePercent�� ������ 0~1�� ����
    public float outlinePercent;

    void Start()
    {
        GeneratorMap();
    }

    public void GeneratorMap()
    {

        // ��� Ÿ�ϵ��� �ڽ����� ������ ���� ������Ʈ
        string holderName = "Generated Map";
        // holderName �Ʒ��� �ڽ��� �����Ѵٸ� �ı�
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
                // �ʿ��� ���� ��� �𼭸��� �������� ������ ��
                Vector3 tilePosition = new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right*90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent); // �׵θ� ũ�⸸ŭ Ÿ���� ũ�⸦ �ٿ��� �Ҵ�
                newTile.parent = mapHolder;
            }
        }
    }
}
