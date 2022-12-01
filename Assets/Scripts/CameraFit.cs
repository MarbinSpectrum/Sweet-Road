using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : �ػ󵵿� ���� ī�޶��� ũ�⸦ �����մϴ�.
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
            //���̰� ���� ����
            //�ʺ� �������� �ػ󵵸� �����.
            unitsPerPixel = sceneWidth / Screen.width;
        }
        else
        {
            //�ʺ� ���� ����
            //���� �������� �ػ󵵸� �����.
            unitsPerPixel = sceneHeight / Screen.height;
        }

        float desiredHalf = 0.5f * unitsPerPixel * Screen.height;

        _camera.orthographicSize = desiredHalf;
    }
}