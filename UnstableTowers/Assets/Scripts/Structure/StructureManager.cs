using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    //Buildable Structure
    public GameObject LaserTower;

    public enum KnownStructures{ None, Laser }
    private KnownStructures preparedStructure = KnownStructures.None;

    public void prepareStructure(KnownStructures s) {
        //TODO: check Money?
        Debug.Log("prepareStructure: " + s);
        preparedStructure = s;
    }

    public void buildPreparedStructure(WorldNode n) {
        if(n.GetNodeType() == NodeType.Free && preparedStructure == KnownStructures.Laser) {
            n.structure = Instantiate(LaserTower, n.empty.transform);
            n.SetNodeType(NodeType.Tower);
            prepareStructure(KnownStructures.None);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
