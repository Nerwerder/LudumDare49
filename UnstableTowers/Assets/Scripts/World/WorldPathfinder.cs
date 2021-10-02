using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WorldPathfinder : MonoBehaviour
{
    public WorldPath findPathFromTo(WorldNode start, WorldNode end) {
        var path = new WorldPath();

        //A*
        List<WorldNode> open = new List<WorldNode>();
        List<int> openIDs = new List<int>();    //Better performance for "contains"
        List<int> closed = new List<int>();
        open.Add(start);
        openIDs.Add(start.id);
        start.parent = null;
        while (open.Count != 0) {
            var cNode = open[0];
            if(cNode.id == end.id) {
                while(cNode.parent !=  null) {
                    path.nodes.Add(cNode);
                    cNode = cNode.parent;
                }
                break;
            }
            foreach(var n in cNode.neighbors) {
                if(((n.type == NodeType.Free) || (n.id == end.id)) && !(closed.Contains(n.id)) && !(openIDs.Contains(n.id))) {
                    n.parent = cNode;
                    open.Add(n);
                    openIDs.Add(n.id);
                }
            }
            open.Remove(cNode);
            openIDs.Remove(cNode.id);
            closed.Add(cNode.id);
            if(closed.Count > 10000) {
                Assert.IsTrue(false, "This is the deadlock prevention");
                break;
            }
        }

        Assert.IsFalse(path.nodes.Count == 0, "No PATH to target found");
        foreach(var p in path.nodes) {
            p.ground.GetComponent<Renderer>().material.color = Color.cyan;
        }

        return path;
    }
}
