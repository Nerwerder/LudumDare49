using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameController : MonoBehaviour
{
    private CameraControl pCamera;
    private Player pPlayer;
    private StructureManager sManager;

    public void Start() {
        pCamera = FindObjectOfType<CameraControl>();
        Assert.IsNotNull(pCamera, "GameController: No Camera with CameraControl script found");
        pPlayer = FindObjectOfType<Player>();
        Assert.IsNotNull(pCamera, "GameController: No Player with Player script found");
        sManager = FindObjectOfType<StructureManager>();
        Assert.IsNotNull(sManager, "GameController: No Manager with StructureManager script found");
    }

    public void PressButton(string type) {
        switch(type) {
            case "Laser":
                sManager.prepareStructure(StructureManager.KnownStructures.Laser);
                break;
            default:
                Assert.IsTrue(false, "Unknown Type");
                break;
        }
    }

    private WorldNode RayCastForNode() {
        WorldNode node = null;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000)) {
            node = hit.collider.gameObject.GetComponentInParent<WorldNode>();
        }
        return node;
    }

    private void Update() {
        //Left Mouse Key
        if(Input.GetMouseButtonDown(0)) {
            var node = RayCastForNode();
            if (node != null) {
                sManager.buildPreparedStructure(node);
            }
        }

        //Numbers
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            sManager.prepareStructure(StructureManager.KnownStructures.Laser);
        }

        //Right Mouse Key
        if (Input.GetMouseButtonDown(1)) {
            var node = RayCastForNode();
            if (node != null) {
                pPlayer.SetTarget(node);
            }
        }

        //WASD
        Vector3 cameraMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (cameraMovement.magnitude != 0) {
            pCamera.Move(cameraMovement);
        }
        float cameraZoom = Input.GetAxis("Mouse ScrollWheel");
        if (cameraZoom != 0f) {
            pCamera.Zoom(cameraZoom);
        }
    }
}