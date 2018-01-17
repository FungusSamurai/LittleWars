using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMaster
{
    private static MapMaster instance;

    private static int map_radius = 30; // the size of the map, or number of hexes to be used in terms of length and height.
    private static float radius = 1; // the radius of the hex, in this case, 1
    private static float width = 2 * radius; // width of the hexes
    private static float height = (Mathf.Sqrt(3) / 2.0f) * width; // height of the hexes
    private static float horzDistance = width * (3.0f / 4.0f);// the horizontal distance from one hex to another
    private static float vertDistance = height; // the vertical distance from one hex to the another
    private static Highlights hl = new Highlights();

    private GameObject g_map;

    private HexCell[,] map; // holds all the cells at their given r,q cordinate, when I do more de coupling, we'll use the map manager map instead

    private static Vector2[,] directions = { // int order; SE, NE, N, NW, SW, S 
        {
            new Vector2(+1, +1), new Vector2(+1, 0), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(-1, +1), new Vector2(0, +1)
        },
        {
            new Vector2(+1, 0), new Vector2(+1, -1), new Vector2(0, -1), new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(0, +1)
        }
    };

    public MapMaster()
    {
        map = new HexCell[0,0];
        g_map = GameObject.Find("MapControl");
        instance = this;
    }

    //Possibly handy for use in dev tools. Such as mass selecting and scaling tiles. Recommend making it static if you are to use it that way
    public static List<HexCell> CellsWithinArea(HexCell center, int range)
    {
        List<HexCell> results = new List<HexCell>();
        int dq, dr;

        for (int dx = -1 * range; dx <= range; dx++)
        {
            for (int dy = Mathf.Max(-1 * range, -dx - range); dy <= Mathf.Min(range, (range - dx)); dy++)
            {
                int dz = -1 * dx - dy;

                Vector3 co = new Vector3(dx, dy, dz);

                Vector2 d = HexCell.NewLocationInOffset (center.Cube, co);
                dq = (int)d.x;
                dr = (int)d.y;

                if (IsCellOnMap(dq, dr))
                {
                    results.Add(Map[dr, dq]);
                }
            }
        }

        return results;
    }

    // Like Cells within area, but only for a radius of 1. Carefull, this will return null values.
    private static void Neighbor(HexCell c, int d, out HexCell t)
    {
        int parity = c.Q & 1;
        Vector2 dir = directions[parity, d];
        int row = c.R + (int)dir.y;
        int col = c.Q + (int)dir.x;

        if (row < 0 || col < 0)
        {
            t = null;
        }
        else
        {
            t = Map[row, col];
        }
    }

    public static bool IsCellOnMap(int q, int r) // if the provided cordinates are within the map's array, returns true. False otherwise
    {
        if (q < map_radius && r < map_radius && q > -1 && r > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void MakeTile(int x, int z) // remember to make Highlight g static later
    {
        float xPos;
        float zPos;

        xPos = x * horzDistance;
        zPos = z * vertDistance;
        if (x % 2 == 1)
        {
            zPos -= vertDistance * 0.5f;
        }
        GameObject g = (GameObject)GameObject.Instantiate(Resources.Load("prefabs/HexFab"), new Vector3(xPos, 0f, -zPos), Quaternion.Euler(0, 0, 0));
        Map[z, x] = g.gameObject.GetComponent<HexCell>();
        Map[z, x].SetCoord(x, z);
        g.transform.parent = gMap.transform;

        if (Map[z, x].R >= 2 && Map[z, x].R <= 3)
        {
            Highlights.HighlightGameObject(g, 2);
        }
        else if (Map[z, x].R >= map_radius - 4 && Map[z, x].R <= map_radius - 3)
        {
            Highlights.HighlightGameObject(g, 3);
        }

    }

    public static Unit MakeTerrain(string name) // Spawns a prefab instance with the given name parameter at the zero position and returns it's attached Unit child script 
    {
        GameObject g = (GameObject)GameObject.Instantiate(Resources.Load("prefabs/terrain/" + name), Vector3.zero, Quaternion.identity);
        return g.GetComponent<Unit>();
    }

    public static void SetMap(int r)
    {
        instance.map = new HexCell[r, r];
        map_radius = r;
    }

    //
    // Accessor Methods
    //
    public static HexCell[,] Map
    {
        get { return instance.map; }
    }

    public static GameObject gMap
    {
        get { return instance.g_map; }
    }

    public static int MapRadius
    {
        get { return map_radius; }
    }

    public static float Height //Added this so that cannoncamtest can clamp itself (Brennan)
    {
        get { return height; }
    }

    public static float Width
    {
        get { return width; }
    }
}
