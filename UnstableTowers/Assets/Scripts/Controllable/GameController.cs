using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private EnemyManager enemyManager;
    private CameraControl pCamera;
    private StructureManager sManager;
    private WorldManager worldManager;

    public Toggle gunToggle;
    public Toggle mortarToggle;
    public Toggle removeToggle;
    public bool started;
    public Toggle curToggle { get; set; }
    private bool pathToggle;
    public AudioSource audio;
    private bool audiotoggle;

    public void Start() {
        pCamera = FindObjectOfType<CameraControl>();
        Assert.IsNotNull(pCamera, "GameController: No Camera with CameraControl script found");
        sManager = FindObjectOfType<StructureManager>();
        Assert.IsNotNull(sManager, "GameController: No Manager with StructureManager script found");
        worldManager = FindObjectOfType<WorldManager>();
        Assert.IsNotNull(worldManager, "GameController: No Manager with WorldManager script found");
        enemyManager = FindObjectOfType<EnemyManager>();
        Assert.IsNotNull(enemyManager, "GameController: No Manager with EnemyManager script found");
        if(started) {
            enemyManager.StartGame();
        }
        pathToggle = false;
        audiotoggle = true;
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

    public void StartGame() {
        enemyManager.StartGame();
    }

    public void EndGame() {
        Application.Quit();
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
            curToggle = gunToggle;
            sManager.TogglePlacementMode(StructureManager.PlacementModes.Gun);  
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            curToggle = mortarToggle;
            sManager.TogglePlacementMode(StructureManager.PlacementModes.Mortar);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            curToggle = removeToggle;
            sManager.TogglePlacementMode(StructureManager.PlacementModes.Remove);
        }

        //N - Toggle Lanes
        if(Input.GetKeyDown(KeyCode.N)) {
            pathToggle = !pathToggle;
            worldManager.TogglePathDrawing(pathToggle);
        }

        //M - Mute Audio
        if(Input.GetKeyDown(KeyCode.M)) {
            audiotoggle = !audiotoggle;
            audio.mute = audiotoggle;
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            EndGame();
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
