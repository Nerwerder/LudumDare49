using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraControl : MonoBehaviour
{
    public float smoothSpeed = 10f;
    public float zoomFactor = 3f;
    public float maxZoom = 5000f;
    public float minZoom = 10f;

    Camera cam;
    public void Start() {
        cam = GetComponent<Camera>();
        Assert.IsNotNull(cam);
    }

    public void Move(Vector3 userInput) {
        var direction = (userInput.x * transform.right) + (userInput.z * transform.up);
        var desPosition = transform.position + direction;
        var smoPosition = Vector3.Lerp(transform.position, desPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoPosition;
    }

    public void Zoom(float c) {
        cam.orthographicSize = Mathf.Clamp((cam.orthographicSize + (-c * zoomFactor)), minZoom, maxZoom);
    }
}
