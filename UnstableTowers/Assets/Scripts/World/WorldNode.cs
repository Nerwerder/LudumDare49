using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType { Free, Water, Block, Reactor, Reactor_Pump, Reactror_Control, EnemySpawner, Generate }
public class WorldNode : MonoBehaviour
{
    public int id { get; set; } //WARNING: id has to be guaranted unique
    public NodeType type { get; set; }
    //Index
    public int x { get; set; }
    public int z { get; set; }
    //Network (A*)
    public WorldNode parent { get; set; }
    public List<WorldNode> neighbors { get; set; }
    //Every Node knows the paths, so it is able to tell them if something blocks this node
    public List<WorldPath> paths { get; set; } 
   //Game Objects
    public GameObject empty { get; set; }
    public GameObject wayPoint { get; set; }
    public GameObject ground { get; set; }
    public GameObject structure { get; set; }
    public void initialize(int _i, NodeType t, GameObject e, GameObject w, int _x, int _z) {
        id = _i;
        type = t;
        empty = e;
        wayPoint = w;
        neighbors = new List<WorldNode>();
        paths = new List<WorldPath>();
        ground = null;
        structure = null;
        x = _x;
        z = _z;
    }
}
