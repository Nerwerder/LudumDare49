using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WorldPathfinder : MonoBehaviour
{
    /// <summary>
    /// Helper Class that increases A* performance by improving the "open.contains" check by a lot.
    /// </summary>
    public class OpenNodes
    {
        public List<WorldNode> nodes { get; set; }
        public List<int> ids { get; set; }  //Better performance for "contains"
        public OpenNodes() {
            nodes = new List<WorldNode>();
            ids = new List<int>();
        }
        public void Add(WorldNode n) {
            nodes.Add(n);
            ids.Add(n.id);
        }
        public void Remove(WorldNode n) {
            nodes.Remove(n);
            ids.Remove(n.id);
        }
        public bool Contains(int id) {
            return ids.Contains(id);
        }
        public int Count() {
            return nodes.Count;
        }
        public WorldNode this[int i] {
            get { return nodes[i]; }
        }
    }

    private WorldManager worldManager;

    private void Start() {
        worldManager = FindObjectOfType<WorldManager>();
        Assert.IsNotNull(worldManager);
    }

    private List<WorldNode> AStar(WorldNode start, WorldNode end) {
        //A*
        List <WorldNode> ret = new List<WorldNode>();
        OpenNodes open = new OpenNodes();
        List<int> closed = new List<int>();
        open.Add(start);
        start.parent = null;
        while (open.Count() != 0) {
            var cNode = open[0];
            if (cNode.id == end.id) {
                while (cNode != null) {
                    ret.Add(cNode);
                    cNode = cNode.parent;
                }
                return ret;
            }
            foreach (var n in cNode.neighbors) {
                if (((n.GetNodeType() == NodeType.Free) || (n.id == end.id)) && !(closed.Contains(n.id)) && !(open.Contains(n.id))) {
                    n.parent = cNode;
                    open.Add(n);
                }
            }
            open.Remove(cNode);
            closed.Add(cNode.id);
            if (closed.Count > 10000) {
                Assert.IsTrue(false, "This is the deadlock prevention");
                break;
            }
        }
        return ret;
    }

    public WorldPath FindPathFromTo(WorldNode start, WorldNode end) {
        var nodes = AStar(start, end);
        if(nodes.Count > 0) {
            //Listener: Path knows Pathfinder so it can ask for a update if something changes
            WorldPath path = new WorldPath(worldManager, this);
            path.Prepare(nodes);
            return path;
        }
        Assert.IsTrue(false, "No Path found");
        return null;
    }

    public void UpdatePath(WorldPath path) {
        var nodes = AStar(path.GetStart(), path.GetEnd());
        if(nodes.Count > 0) {
            path.Prepare(nodes);
        } else {
            Assert.IsTrue(false, "No Path found");
        }
    }
}
