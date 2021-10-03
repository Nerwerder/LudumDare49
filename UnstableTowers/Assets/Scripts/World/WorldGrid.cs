using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;


public class WorldGrid : MonoBehaviour
{
    public bool heightPerlin;
    [System.Serializable]
    public struct Generationinstruction {
        public char letter;
        public NodeType type;
        public GameObject ground;
        public GameObject structure;
    }

    //WORKAROUND: the only reason for this Array is to display/manipulate the onctent in the Inspector
    private Dictionary<char, Generationinstruction> instructions = new Dictionary<char, Generationinstruction>();
    public List<List<WorldNode>> world;

    private EnemyManager enemyManager;
    private WorldManager worldManager;

    public void initialize(Generationinstruction[] i) {
        world = new List<List<WorldNode>>();
        foreach (var k in i) { instructions.Add(k.letter, k); }
        enemyManager = FindObjectOfType<EnemyManager>();
        Assert.IsNotNull(enemyManager, "WorldGrid was not able to find the EnemyManager");
        worldManager = FindObjectOfType<WorldManager>();
        Assert.IsNotNull(worldManager, "WorldGrid was not able to find the WorldManager");
    }

    private int isB(List<List<char>> l, int x, int z) {
        if(x > 0 && x < l.Count && z > 0 && z < l[x].Count && l[x][z] == 'B') {
            return 1;
        }
        return 0;
    }

    private List<List<char>> getGeneratedChars(int dimZ, int dimX) {
        float seedRate = 0.08f;
        float clusterRate = 0.2f;
        var ret = new List<List<char>>();
        //Step 1: seed some Blocks into the World
        for (int z = 0; z < dimZ; ++z) {
            var rX = new List<char>();
            for (int x = 0; x < dimX; ++x) {
                if(Random.Range(0f, 1f) <= seedRate) {rX.Add('B');} 
                else {rX.Add('0');}
            }
            ret.Add(rX);
        }
        //Step 2: Do some Clustering
        for (int z = 0; z < dimZ; ++z) {
            for (int x = 0; x < dimX; ++x) {
                var e = ret[z][x];
                if(e == 'B') { continue; }
                int nb = isB(ret, z-1, x) + isB(ret, z+1, x) + isB(ret, z, x+1) + isB(ret, z, x-1);
                if(nb > 0 && (Random.Range(0f, 1f) < nb*clusterRate)) {
                    ret[z][x] = 'B';
                }
            }
        }
        return ret;
    }

    private List<List<char>> getGeneratedWorldChars(string s, int ex) {
        var retLines = new List<List<char>>();
        using (StringReader reader = new StringReader(s)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                var retLine = new List<char>();
                foreach (char c in line) {
                    if (instructions.ContainsKey(c)) {
                        retLine.Add(c);
                    } else if (c == 'G') {
                        //Short check if the Line is 'pure'
                        foreach (char g in line) { Assert.AreEqual('G', g); }
                        var gen = getGeneratedChars(ex, line.Length);
                        foreach (var k in gen) { retLines.Add(k); }
                        break;
                    } else {
                        Assert.IsTrue(false, "Unknown Character in worldGenFile");
                    }
                }
                if (retLine.Count > 0) {
                    retLines.Add(retLine);
                }
            }
        }
        return retLines;
    }

    private void registerNode(WorldNode n) {
        switch(n.GetNodeType()) {
            case NodeType.EnemySpawner:
                var spawner = n.structure.GetComponent<EnemySpawner>();
                Assert.IsNotNull(spawner);
                spawner.Initialize(n);
                enemyManager.RegisterSpawner(spawner);
                break;
            case NodeType.Reactor:
                worldManager.reactorNode = n;
                break;
            default:
                //Nothing
                break;
        }
    }

    private void generateWorld(List<List<char>> chars, float heightOffset) {
        int nodeID = 0;
        int zc = 0;
        foreach (var row in chars) {
            int xc = 0;
            List<WorldNode> nodeRow = new List<WorldNode>();
            foreach (var type in row) {
                Assert.IsTrue(instructions.ContainsKey(type), "Unknown instruction?");
                var instruction = instructions[type];
                //Create the Empty
                GameObject empty = new GameObject(zc + "_" + xc);
                empty.transform.parent = transform;
                empty.transform.position = new Vector3((zc*2), 0, (xc*2));
                //Create the WayPoint
                GameObject wayPoint = new GameObject("wayPoint" + zc + "_" + xc);
                wayPoint.transform.parent = empty.transform;
                wayPoint.transform.position = empty.transform.position + Vector3.up * heightOffset;
                //Create the Node
                WorldNode node = empty.AddComponent<WorldNode>();
                node.initialize(nodeID++, instruction.type, empty, wayPoint, xc, zc);
                //Add the Block and|or structure
                if (instruction.ground) { node.ground = Instantiate(instruction.ground, empty.transform); }
                if (instruction.structure) { node.structure = Instantiate(instruction.structure, empty.transform); }
                //Is the Node required to register by a Manager?
                registerNode(node);
                //Add the Node to the row
                nodeRow.Add(node);
                ++xc;
            }
            ++zc;
            world.Add(nodeRow);
        }
    }

    private void offsetWorld(float maxY) {
        int xLen = world.Count;
        foreach (var row in world) {
            int zLen = row.Count;
            foreach (var node in row) {
                var p = node.empty.transform.position;
                var px = ((float)p.x) / ((float)xLen);
                var pz = ((float)p.z) / ((float)zLen);
                float sample;
                if (heightPerlin) {
                    sample = Mathf.PerlinNoise(px, pz) * 5;
                } else {
                    sample = Random.Range(-maxY, +maxY);
                }
                node.empty.transform.position = new Vector3(p.x, sample, p.z);
            }
        }
    }

    private void unifyOffset(NodeType t, float off = 0f) {
        List<WorldNode> nodes = new List<WorldNode>();
        float offset = 0f;
        foreach (var row in world) {
            foreach (var node in row) {
                if (node.GetNodeType() == t) {
                    nodes.Add(node);
                    offset += node.empty.transform.position.y;
                }
            }
        }
        offset /= (float)nodes.Count;
        offset -= off;
        foreach(var node in nodes) {
            var p = node.empty.transform.position;
            node.empty.transform.position = new Vector3(p.x, offset, p.z);
        }
    }

    private void addRelPosNode(List<WorldNode> nodes, int zO, int xO) {
        if(zO >= 0 && zO < world.Count && xO >= 0 && xO < world[zO].Count) {
            nodes.Add(world[zO][xO]);
        }
    }

    /// <summary>
    /// Iterate over all Nodes and add the 4 neigboard Nodes to the List
    private void registerNeighbors() {
        foreach(var row in world) {
            foreach(var node in row) {
                addRelPosNode(node.neighbors, node.z+1, node.x);
                addRelPosNode(node.neighbors, node.z-1, node.x);
                addRelPosNode(node.neighbors, node.z, node.x+1);
                addRelPosNode(node.neighbors, node.z, node.x-1);
            }
        }
    }

    /// <summary>
    /// Generate a World from a Given String containing multiple lines and known symbols
    /// </summary>
    /// <param name="s">Input String containing multiple lines of known symbols</param>
    /// <param name="y">Maximum/Minimum y offset</param>
    /// <param name="h">Height offset for the waypoints</param>
    /// <param name="e">Extension, how many rows to create for every 'GGGGGG' row</param>
    public void generateGridFromString(string s, float y, float h, int e) {
        var chars = getGeneratedWorldChars(s, e);
        generateWorld(chars, h);
        offsetWorld(y);
        unifyOffset(NodeType.Water, 0.2f);
        unifyOffset(NodeType.Reactor);
        registerNeighbors();
    }
}
