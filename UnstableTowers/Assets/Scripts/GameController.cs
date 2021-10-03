using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public CameraControl gCamera;
    public Player gPlayer;


    private void Update() {
        //Move the player
        if (Input.GetMouseButtonDown(1)) {
            RaycastHit hit;
            Debug.Log("RayCast");
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000)) {
                Debug.Log("Hit");
                var node = hit.collider.gameObject.GetComponentInParent<WorldNode>();
                if (node != null) {
                    Debug.Log("Node");
                    gPlayer.SetTarget(node);
                }
            }
        }
    }

    private void FixedUpdate() {
        //Move the camera
        Vector3 cameraMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if(cameraMovement.magnitude != 0) {
            gCamera.Move(cameraMovement);
        }
        float cameraZoom = Input.GetAxis("Mouse ScrollWheel");
        if(cameraZoom != 0f) {
            gCamera.Zoom(cameraZoom);
        }
    }
}
