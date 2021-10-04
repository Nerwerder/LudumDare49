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
    private int maxAStarIteration = 10000;

    private void Start() {
        worldManager = FindObjectOfType<WorldManager>();
        Assert.IsNotNull(worldManager);
    }

    private List<WorldNode> AStar(WorldNode start, WorldNode end, NodeType ignore = NodeType.Undefined) {
        //A*
        OpenNodes open = new OpenNodes();
        List<int> closed = new List<int>();
        open.Add(start);
        start.parent = null;
        while (open.Count() != 0) {
            var cNode = open[0];
            //Found a Tath - return the nodes
            if (cNode.id == end.id) {
                List<WorldNode> ret = new List<WorldNode>();
                while (cNode != null) {
                    ret.Add(cNode);
                    cNode = cNode.parent;
                }
                return ret;
            }
            foreach (var n in cNode.neighbors) {
                if (((n.GetNodeType() == NodeType.Free) || (n.id == end.id) || (n.GetNodeType() == ignore)) && 
                    !((closed.Contains(n.id)) || (open.Contains(n.id)))) {
                    n.parent = cNode;
                    open.Add(n);
                }
            }
            open.Remove(cNode);
            closed.Add(cNode.id);
            //Deadlock prevention
            if (closed.Count > maxAStarIteration) {
                Assert.IsTrue(false, "Deadlock prevention");
                break;
            }
        }
        return null;
    }

    private List<WorldNode> GetPath(WorldNode start, WorldNode end) {
        //Try to find a Path the nice way: only using free Nodes
        var nodes = AStar(start, end);
        //If that didn't work, use the less nice way: find a Path ignoring towers, enemys will attack every structure on the way
        if(nodes == null) {
            nodes = AStar(start, end, NodeType.Tower);
        }
        Assert.IsNotNull(nodes, "Unable fo tind a Path, even ignoring Towers");
        //A* returns the Path from Target to Start, reverse it
        nodes.Reverse();
        return nodes;
    }

    public WorldPath FindPathFromTo(WorldNode start, WorldNode end) {
        var nodes = GetPath(start, end);
        WorldPath path = new WorldPath(worldManager, this);
        path.Prepare(nodes);
        return path;
    }

    public void UpdatePath(WorldPath path) {
        var nodes = GetPath(path.GetStart(), path.GetEnd());
        path.Prepare(nodes);
    }
}
