using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class StructureManager : MonoBehaviour
{
    //Buildable Structure
    public GameObject laserTower;
    public GameObject reactor;

    private WorldManager worldManager;
    public enum KnownStructures{ None, Laser, Reactor }
    private KnownStructures preparedStructure = KnownStructures.None;

    public void prepareStructure(KnownStructures s) {
        //TODO: check Money?
        Debug.Log("prepareStructure: " + s);
        preparedStructure = s;
    }

    public void buildPreparedStructure(WorldNode n) {
        if(n.GetNodeType() == NodeType.Free && preparedStructure == KnownStructures.Laser && worldManager.metal> laserTower.GetComponent<LaserTower>().cost) {
            n.structure = Instantiate(laserTower, n.empty.transform);
            n.SetNodeType(NodeType.Tower);
            prepareStructure(KnownStructures.None);
            worldManager.metal -= laserTower.GetComponent<LaserTower>().cost;
        }
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
