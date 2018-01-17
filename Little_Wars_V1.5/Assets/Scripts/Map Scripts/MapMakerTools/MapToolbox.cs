using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ToolTypes // list of currently made tools and their index
{
    DrawTool,
    TerrainTool
}

public class MapToolbox // this things only job is to hold onto instances of each of the tool scripts.
{
    private const int toolCount = 2; 
    private MapTools[] tools;

    public MapToolbox()
    {
        tools = new MapTools[toolCount] {new DrawTool(), new TerrainTool()};
    }

    public void RunTool(int t) // what drive the tool behavior and makes sure only one is active at a time
    {
        tools[t].Behavior();
    }
}
