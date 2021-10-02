using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPath
{
    public List<WorldNode> nodes { get; set; }
    public WorldPath() {
        nodes = new List<WorldNode>();
    }
}
