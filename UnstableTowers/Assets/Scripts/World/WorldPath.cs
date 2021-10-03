using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WorldPath
{
    public List<WorldNode> nodes { get; set; }
    private bool finalized;
    public WorldPath() {
        nodes = new List<WorldNode>();
        finalized = false;
    }
    //Reverse the Path
    public void finalize() {
        Assert.IsFalse(finalized);
        finalized = true;
        nodes.Reverse();
        foreach (var p in nodes) {
            p.paths.Add(this);  //Listener
            p.ground.GetComponent<Renderer>().material.color = Color.cyan;//TODO: Remove
        }
    }
}
