using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private CameraControl pCamera;
    private Player pPlayer;
    private StructureManager sManager;

    public Toggle gunToggle;
    public Toggle mortarToggle;
    public Toggle removeToggle;

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
                sManager.TogglePlacementMode(StructureManager.PlacementModes.Laser);
                break;
            case "Gun":
                sManager.TogglePlacementMode(StructureManager.PlacementModes.Gun);
                break;
            case "Mortar":
                sManager.TogglePlacementMode(StructureManager.PlacementModes.Mortar);
                break;
            case "Remove":
                sManager.TogglePlacementMode(StructureManager.PlacementModes.Remove);
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
                sManager.Work(node);
            }
        }

        //Numbers
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            sManager.TogglePlacementMode(StructureManager.PlacementModes.Gun);
            gunToggle.isOn = true; 
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            sManager.TogglePlacementMode(StructureManager.PlacementModes.Mortar);
            mortarToggle.isOn = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            sManager.TogglePlacementMode(StructureManager.PlacementModes.Remove);
            removeToggle.isOn = true;
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
