using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WorldPath
{
    public List<WorldNode> nodes { get; set; }
    private bool prepared;
    private WorldPathfinder pathfinder;

    public WorldPath(WorldPathfinder _p) {
        nodes = new List<WorldNode>();
        prepared = false;
        pathfinder = _p;
        nodes = null;
    }
    public WorldNode GetStart() {
        Assert.IsFalse(nodes.Count == 1);
        return nodes[0];
    }

    public WorldNode GetEnd() {
        Assert.IsFalse(nodes.Count <= 1);
        return nodes[(nodes.Count) - 1];
    }

    //Prepare for use
    public void Prepare(List<WorldNode> _n) {
        if(_n != null) {
            nodes = _n;
        }
        Assert.IsFalse(prepared);
        prepared = true;
        nodes.Reverse();
        foreach (var p in nodes) {
            p.paths.Add(this);  //Listener
            p.ground.GetComponent<Renderer>().material.color = Color.cyan;//TODO: Remove
        }
    }

    public void Update() {
        Assert.IsTrue(prepared);
        prepared = false;
        foreach (var p in nodes) {
            p.paths.Remove(this);
            p.ground.GetComponent<Renderer>().material.color = Color.green;//TODO: Remove
        }
        pathfinder.UpdatePath(this); 
    }
}
