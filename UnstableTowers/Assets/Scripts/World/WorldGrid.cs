using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;


public class WorldGrid : MonoBehaviour
{
    public bool heightPerlin;

    public enum Type { Free, Water, Block, Reactor, Reactor_Pump, Reactror_Control, EnemySpawner, Generate }

    [System.Serializable]
    public struct Generationinstruction {
        public char letter;
        public Type type;
        public GameObject ground;
        public GameObject structure;
    }

    public struct Node
    {
        public Type type { get; }
        //Network
        public GameObject empty { get; }
        public List<GameObject> neighbors { get; set; } //Required for A*
        //GO
        public GameObject ground { get; set; }
        public GameObject structure { get; set; }
        public Node(Type t, GameObject e) {
            type = t;
            empty = e;
            neighbors = new List<GameObject>();
            ground = null;
            structure = null;
        }
    }

    //WORKAROUND: the only reason for this Array is to display/manipulate the onctent in the Inspector
    private Dictionary<char, Generationinstruction> instructions = new Dictionary<char, Generationinstruction>();
    public List<List<Node>> world = new List<List<Node>>();

    private EnemyManager enemyManager;

    public void initialize(Generationinstruction[] i) {
        foreach (var k in i) { instructions.Add(k.letter, k); }
        enemyManager = FindObjectOfType<EnemyManager>();
        Assert.IsNotNull(enemyManager, "WorldGrid was not able to find a enemyManager");
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

    private void registerNode(Node n) {
        switch(n.type) {
            case Type.EnemySpawner:
                enemyManager.registerSpawner(n.structure.GetComponent<EnemySpawner>());
                break;
            default:
                //Nothing
                break;
        }
    }

    private void generateWorld(List<List<char>> chars) {
        int zc = 0;
        foreach (var row in chars) {
            int xc = 0;
            List<Node> nodeRow = new List<Node>();
            foreach (var type in row) {
                Assert.IsTrue(instructions.ContainsKey(type), "Unknown instruction?");
                var instruction = instructions[type];
                //Create the Empty
                GameObject empty = new GameObject(zc + "_" + xc);
                empty.transform.parent = transform;
                empty.transform.position = new Vector3(zc, 0, xc);
                //Create the Node
                Node node = new Node(instruction.type, empty);
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
                float sample = 0f;
                if (heightPerlin) {
                    sample = Mathf.PerlinNoise(px, pz) * 5;
                } else {
                    sample = Random.Range(-maxY, +maxY);
                }
                node.empty.transform.position = new Vector3(p.x, sample, p.z);
            }
        }
    }

    private void unifyOffset(Type t, float off = 0f) {
        List<Node> nodes = new List<Node>();
        float offset = 0f;
        foreach (var row in world) {
            foreach (var node in row) {
                if (node.type == t) {
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

    /// <summary>
    /// Generate a World from a Given String containing multiple lines and known symbols
    /// </summary>
    /// <param name="s">Input String containing multiple lines of known symbols</param>
    /// <param name="y">Maximum/Minimum y offset</param>
    /// <param name="e">Extension, how many rows to create for every 'GGGGGG' row</param>
    public void generateGridFromString(string s, float y, int e) {
        var chars = getGeneratedWorldChars(s, e);
        generateWorld(chars);
        offsetWorld(y);
        unifyOffset(Type.Water, 0.2f);
        unifyOffset(Type.Reactor);
    }
}
