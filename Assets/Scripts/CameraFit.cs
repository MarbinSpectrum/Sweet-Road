using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : 해상도에 따른 카메라의 크기를 조정합니다.
////////////////////////////////////////////////////////////////////////////////
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraFit : MonoBehaviour
{
    [SerializeField] private float sceneWidth = 10;
    [SerializeField] private float sceneHeight = 10;
    [SerializeField] private float baseSceneWidth = 1080;
    [SerializeField] private float baseSceneHeight = 1920;
    [SerializeField] private Camera _camera;

    private void Update()
    {
        float rate = baseSceneWidth / baseSceneHeight;
        float unitsPerPixel = 0;
        if (rate > (float)Screen.width / (float)Screen.height)
        {
            //높이가 높은 상태
            //너비 기준으로 해상도를 맞춘다.
            unitsPerPixel = sceneWidth / Screen.width;
        }
        else
        {
            //너비가 넓은 상태
            //높이 기준으로 해상도를 맞춘다.
            unitsPerPixel = sceneHeight / Screen.height;
        }

        float desiredHalf = 0.5f * unitsPerPixel * Screen.height;

        _camera.orthographicSize = desiredHalf;
    }
}