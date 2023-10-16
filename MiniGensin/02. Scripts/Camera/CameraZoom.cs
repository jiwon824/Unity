using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] [Range(0f, 10f)] private float defaultDistance = 6f;
    [SerializeField] [Range(0f, 10f)] private float minDistance = 1f;
    [SerializeField] [Range(0f, 10f)] private float maxDistance = 6f;

    [SerializeField] [Range(0f, 10f)] private float smoothing = 4f;
    [SerializeField] [Range(0f, 10f)] private float zoomSensitivity = 1f;

    private float currentTargetDistance;

    private CinemachineFramingTransposer framingTransposer;
    private CinemachineInputProvider inputProvider;

    void Awake()
    {
        
        framingTransposer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
        inputProvider = GetComponent<CinemachineInputProvider>();

        currentTargetDistance = defaultDistance;
    }

    void Update()
    {
        Zoom();
    }

    private void Zoom()
    {
        float zoomValue = inputProvider.GetAxisValue(2)*zoomSensitivity; // z Axis index is 2

        currentTargetDistance = Mathf.Clamp(currentTargetDistance+zoomValue, minDistance, maxDistance);

        // 현재거리와 목표거리가 같으면 아무것도 하지 않도록 설정 
        float currentDistance = framingTransposer.m_CameraDistance;
        if(currentDistance == currentTargetDistance)
        {
            return;
        }
        // 여기서는 "스무딩 * Time.deltaTime"을 전달하는데,
        // 이는 전체 러프에 대해 초 단위가 아니라 매번 변경할 때마다 가능한 한 일관성을 유지하면서 도달할 때마다 그 시간에 도달하기를 원하기 때문입니다.
        float lerpedZoomValue = Mathf.Lerp(currentDistance, currentTargetDistance, smoothing * Time.deltaTime);
        framingTransposer.m_CameraDistance = lerpedZoomValue;
    }
}
