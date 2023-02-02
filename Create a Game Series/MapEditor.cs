using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator map = target as MapGenerator;
        // DrawDefaultInspector()�� bool ���� �����ϴµ�, �ν����Ϳ��� ���� ����Ǿ��� ���� true ��ȯ
        // ���� ����Ǿ��� ����, ���� �����
        if (DrawDefaultInspector())
        {
            map.GenerateMap();
        }

        // ��ũ��Ʈ���� ���� �ٲ��� ����, ���� ������ϰ� ���� ��
        // �������� ���� ������� �� �ְ� ��ư ����
        if (GUILayout.Button("Generate Map"))
        {
            map.GenerateMap();
        }
    }
}