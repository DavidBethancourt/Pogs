using UnityEngine;
using System.Collections;

public class CameraBehavior : MonoBehaviour {

    private float _cameraDistanceMax = 50f;
    private float _cameraDistanceMin = 2f;
    public float CameraDistance = 10f;
    private float _scrollSpeed = 4.0f;
    public Camera MainCamera;
    void Start()
    {
       
    }

    void Update()
    {
        CameraDistance -= Input.GetAxis("Mouse ScrollWheel") * _scrollSpeed;
        CameraDistance = Mathf.Clamp(CameraDistance, _cameraDistanceMin, _cameraDistanceMax);
        MainCamera.orthographicSize = CameraDistance;

        float xAxisValue = Input.GetAxis("Horizontal");
        float yAxisValue = Input.GetAxis("Vertical");
        MainCamera.transform.position = new Vector3(MainCamera.transform.position.x + xAxisValue, MainCamera.transform.position.y + yAxisValue, MainCamera.transform.position.z);
    }
}
