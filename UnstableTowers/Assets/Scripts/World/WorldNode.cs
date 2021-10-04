using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum NodeType { Undefined, Free, Water, Block, Reactor, Reactor_Pump, Reactror_Control, EnemySpawner, Generate, Tower }
public class WorldNode : MonoBehaviour
{
    public int id { get; set; } //WARNING: id has to be guaranted unique
    private NodeType type = NodeType.Undefined;
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
    public NodeType GetNodeType() { return type; }

    public void AddStructure(NodeType _n, GameObject _s) {
        Assert.IsTrue(type == NodeType.Free);
        type = _n;
        structure = _s;
        //Inform every Path about the change (this Node is not passable anymore)
        List<WorldPath> tmpPaths = new List<WorldPath>(paths);
        foreach (var p in tmpPaths) {
            p.Update();
        }
    }

    public void RemoveStructure() {
        Assert.IsTrue(type != NodeType.Free);
        Assert.IsNotNull(structure);
        type = NodeType.Free;
        structure = null;
    }

    public Vector3 GetDirectionToWaypoint(Vector3 _v) {
        return (wayPoint.transform.position - _v);
    }
}
