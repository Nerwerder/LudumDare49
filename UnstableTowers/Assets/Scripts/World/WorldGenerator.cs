using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class WorldGenerator : MonoBehaviour
{
    //File with instructions on "how to build the World"
    public TextAsset worldGenInstruction;
    //How big should the Generated Part of the Instruction (the "G" Line) be
    public float yOffset;
    //How many Lines should be places instead of  "G" Line
    public int expansion;
    //WORKAROUND: the only reason for this Array is to display/manipulate the onctent in the Inspector
    public WorldGrid.Generationinstruction[] instructions;
    private WorldGrid grid;

    void Start()
    {
        Assert.IsFalse(instructions.Length == 0);
        Assert.IsNotNull(worldGenInstruction, "worldGenInstruction empty in WorldGenerator");
        grid = GetComponentInChildren<WorldGrid>();
        Assert.IsNotNull(grid, "grid was not found in WorldGenerator");
        grid.initialize(instructions);
        grid.generateGridFromString(worldGenInstruction.text, yOffset, expansion);
    }
}
