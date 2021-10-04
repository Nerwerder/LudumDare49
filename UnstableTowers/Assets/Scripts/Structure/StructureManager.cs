using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class StructureManager : MonoBehaviour
{
    //Buildable Structure
    public GameObject laserTower;
    public GameObject gunTower;
    public GameObject mortarTower;
    public GameObject reactor;
    public GameObject reactor_Control;
    public GameObject reactor_Pump;
    public float recyclingRate;

    private WorldManager worldManager;
    private GameController gameController;
    public enum KnownStructures{ None, Laser, Gun, Mortar, Reactor, Reactor_Control, Reactor_Pump }
    public enum PlacementModes { None, Laser, Gun, Mortar, Remove }
    private PlacementModes placementMode = PlacementModes.None;

    public void TogglePlacementMode(PlacementModes s) {
        if(placementMode == s) {
            placementMode = PlacementModes.None;
            gameController.curToggle.isOn = false;
        } else {
            placementMode = s;
            if(placementMode != PlacementModes.None) {
                gameController.curToggle.isOn = true;
            }
        }
    }

    public bool validTowerPlacement(WorldNode n, GameObject o) {
        return (n.GetNodeType() == NodeType.Free) && (worldManager.metal >= o.GetComponent<Tower>().cost);
    }

    public void placeTower(WorldNode n, GameObject o) {
        var st = Instantiate(o, n.empty.transform);
        n.AddStructure(NodeType.Tower, st);
        var tower = st.GetComponent<Tower>();
        worldManager.metal -= tower.cost;
        worldManager.towers.Add(tower);
    }

    public bool Work(WorldNode n) {
        if(placementMode == PlacementModes.None) { return false; }

        switch(placementMode) {
            case PlacementModes.Laser:
                if (validTowerPlacement(n, laserTower)) {
                    placeTower(n, laserTower);
                    return true;
                }
                break;
            case PlacementModes.Gun:
                if (validTowerPlacement(n, gunTower)) {
                    placeTower(n, gunTower);
                    return true;
                }
                break;
            case PlacementModes.Mortar:
                if (validTowerPlacement(n, mortarTower)) {
                    placeTower(n, mortarTower);
                    return true;
                }
                break;
            case PlacementModes.Remove:
                if (n.structure != null) {
                    var s = n.structure.GetComponent<Structure>();
                    if(s && s.removable) {
                        worldManager.metal += (int)(s.cost * recyclingRate);
                        s.Remove();
                        worldManager.UpdateAllPaths();
                        return true;
                    }
                }
                break;
            default:
                Assert.IsTrue(false, "PlacementMode not handled");
                break;
        }
        return false;
    }

    public GameObject getStructure(KnownStructures _s) {
       switch(_s) {
            case KnownStructures.Reactor:
                return reactor;
            case KnownStructures.Reactor_Control:
                return reactor_Control;
            case KnownStructures.Reactor_Pump:
                return reactor_Pump;
            default:
                Assert.IsTrue(false);
                break;
        }
        return null;
    }

    void Start()
    {
        worldManager = FindObjectOfType<WorldManager>();
        Assert.IsNotNull(worldManager);
        gameController = FindObjectOfType<GameController>();
        Assert.IsNotNull(gameController);
    }
}
