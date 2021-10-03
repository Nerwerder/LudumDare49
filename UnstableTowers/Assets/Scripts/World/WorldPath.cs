using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WorldPath
{
    public List<WorldNode> nodes { get; set; }
    private bool prepared;
    private WorldPathfinder pathfinder;
    private WorldManager worldManager;

    public WorldPath(WorldManager _m, WorldPathfinder _p) {
        nodes = new List<WorldNode>();
        prepared = false;
        pathfinder = _p;
        worldManager = _m;
        worldManager.RegisterPath(this);
        nodes = null;
    }

    ~WorldPath() {
        worldManager.DeregisterPath(this);
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
        }
        worldManager.DrawPath(this);
    }

    public void Update() {
        Assert.IsTrue(prepared);
        prepared = false;
        foreach (var p in nodes) {
            p.paths.Remove(this);
        }
        pathfinder.UpdatePath(this); 
    }
}
