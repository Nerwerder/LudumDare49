using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WorldPath
{
    public List<WorldNode> nodes { get; set; }
    /// <summary>
    /// Every Path needs to know which Enemies are on it.
    /// This is important because if a Path changes on Node N and some Enemies are already on N+X, they have to leave the Path.
    /// </summary>
    public List<Enemy> enemies { get; set; }
    private bool prepared;
    private WorldPathfinder pathfinder;
    private WorldManager worldManager;

    public WorldPath(WorldManager _m, WorldPathfinder _p) {
        nodes = null;
        enemies = new List<Enemy>();
        prepared = false;
        pathfinder = _p;
        worldManager = _m;
        worldManager.RegisterPath(this);
    }

    /// <summary>
    /// Copy Constructor for the WorldPath - Creates a tmp Node Copy without all Functions
    /// Diff:
    /// - No WorldManager
    /// - Not Registered with worldManager
    /// - No Enemies (they can not be on two Paths at the same time)
    /// - No Pathfinder
    /// </summary>
    /// <param name="_o"></param>
    public WorldPath(WorldPath _o) {
        nodes = _o.nodes;
        enemies = null;
        prepared = _o.prepared;
        pathfinder = null;
        worldManager = null;
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
        foreach (var p in nodes) {
            p.paths.Add(this);  //Listener
        }
        worldManager.DrawPath(this);
    }

    /// <summary>
    /// Find a new Path (most likely because the old one is blocked)
    /// </summary>
    public void Update() {
        Assert.IsTrue(prepared);
        prepared = false;
        //Remove the Listener from the nodes (will be added again if the path uses the same node again)
        foreach (var p in nodes) {
            p.paths.Remove(this);  
        }
        //Tell all Enemies to prepare for a Path Update
        List<Enemy> tmpEnemies = new List<Enemy>(enemies);
        foreach (var e in tmpEnemies) {
            e.PreparePathUpdate();
        }
        pathfinder.UpdatePath(this); 
    }
}
