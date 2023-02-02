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
        // DrawDefaultInspector()는 bool 값을 리턴하는데, 인스펙터에서 값이 변경되었을 때만 true 반환
        // 값이 변경되었을 때만, 맵을 재생성
        if (DrawDefaultInspector())
        {
            map.GenerateMap();
        }

        // 스크립트에서 값을 바꿨을 때도, 맵을 재생성하고 싶을 것
        // 수동으로 맵을 재생성할 수 있게 버튼 생성
        if (GUILayout.Button("Generate Map"))
        {
            map.GenerateMap();
        }
    }
}