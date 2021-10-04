using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class StructureManager : MonoBehaviour
{
    //Buildable Structure
    public GameObject laserTower;
    public GameObject reactor;
    public float recyclingRate;

    private WorldManager worldManager;
    public enum KnownStructures{ None, Laser, Reactor }
    public enum PlacementModes { None, Laser, Remove }
    private PlacementModes placementMode = PlacementModes.None;

    public void TogglePlacementMode(PlacementModes s) {
        Debug.Log("TogglePlacementMode: " + s);
        if(placementMode == s) {
            placementMode = PlacementModes.None;
        } else {
            placementMode = s;
        }
    }

    public bool Work(WorldNode n) {
        if(placementMode == PlacementModes.None) { return false; }

        switch(placementMode) {
            case PlacementModes.Laser:
                //The Node has to be free and we need enough Metal in the Bank
                if ((n.GetNodeType() == NodeType.Free) && (worldManager.metal > laserTower.GetComponent<LaserTower>().cost)) {
                    var s = Instantiate(laserTower, n.empty.transform);
                    n.AddStructure(NodeType.Tower, s);
                    worldManager.metal -= laserTower.GetComponent<LaserTower>().cost;
                    return true;
                }
                break;
            case PlacementModes.Remove:
                if (n.structure != null) {
                    var s = n.structure.GetComponent<Structure>();
                    Assert.IsNotNull(s);
                    if(s.removable) {
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
    }
}
