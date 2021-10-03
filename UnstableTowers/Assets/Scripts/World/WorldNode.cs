using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType { Free, Water, Block, Reactor, Reactor_Pump, Reactror_Control, EnemySpawner, Generate }
public class WorldNode
{
    public int id { get; } //WARNING: id has to be guaranted unique
    public NodeType type { get; }
    //Index
    public int x { get; }
    public int z { get; }
    //Network (A*)
    public WorldNode parent { get; set; }
    public List<WorldNode> neighbors { get; set; }
    //Every Node knows the paths, so it is able to tell them if something blocks this node
    public List<WorldPath> paths { get; set; } 
   //Game Objects
    public GameObject empty { get; }
    public GameObject wayPoint { get; }
    public GameObject ground { get; set; }
    public GameObject structure { get; set; }
    public WorldNode(int _i, NodeType t, GameObject e, GameObject w, int _x, int _z) {
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
