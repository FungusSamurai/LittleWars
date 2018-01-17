using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDictionary {

    List<string> types = new List<string> { "grass", "ground", "water" };

    List<int> costs = new List<int> { 1, 2, 2 };

    List<Material> materials = new List<Material>();

    public static List<KeyValuePair<Material, int>> TileStats = new List<KeyValuePair<Material, int>>();

    public TileDictionary()
    {
        KeyValuePair<Material, int> kvp;
        Material mat;

        for (int i = 0; i < types.Count; i++)
        {
            mat = (Material)Resources.Load("Models/Materials/" + types[i] + "_mat");
            materials.Add(mat);
        }

        for(int j = 0; j < materials.Count; j++)
        {
            kvp = new KeyValuePair<Material, int>(materials[j], costs[j]);
            TileStats.Add(kvp);
        }
    }


}
