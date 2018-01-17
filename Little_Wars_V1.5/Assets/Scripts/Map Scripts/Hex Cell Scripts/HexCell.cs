using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the current range of sizes. If we need more, it'll be a simple matter of adding to them here and accounting for the range increase in scripts like DrawTool
// should also be able to make code that creates an array based off of each element to further allow ease of change but current implementation is servicable. 
public enum HexSize
{
    normal, grade1, grade2, grade3, grade4, grade5
}
// the current range of types a tile can be.
public enum TileType { Grass, Mud, Water }

/// <summary>
/// A class attached to each hex cell prefab to hold information used by the HexController and other scripts.
/// Invariant: The cube coordinates must sum to equal 0.
/// </summary>
public class HexCell : MonoBehaviour
{
    const int sizeConstant = 10; // the factor by which we scale the size values. In terms of world scale 10 equals 1 in-game unit (verticaly). So a tile with y-scale of 1 is 1/10th a unit high.
    // We can fiddle with the above constant to see what's a good size range for these hills

    public int q, r; // the posistion of the hex in odd-q cordinates. q is for column, r is for row.
    public GameObject spawnPoint; // the empty game object attached to the hex cell prefab, used when spawning and moving units.
    static Vector3 yVector = new Vector3(0.0f, 0.513f, 0.0f); // the amount by which units are spawned above the spawn point. 

    //int cx, cz, cy; // the cordinates of the hex in cube or x/y/z cordinates. Required for some algorithms.
    private Vector3 cubeCord;
    public int cost; // the amount of movment speed that must be spent to move here. 

    private HexSize size;
    public TileType tileType;


    public Unit unit; // the current unit on the tile, if any.
    public bool passable; // if the unit is a passible tile, not really used at the moment, but will be when terrain and other eviromental models are put in the game.
    public bool Traversing;

    // Use this for initialization
    void Awake()
    {
        unit = null;
        passable = true;
        size = HexSize.normal;
        cost = 1;
        tileType = TileType.Grass;
        //   yVector = new Vector3(0.0f, 0.513f, 0.0f);

    }

    /// <summary>
    /// Sets's the cube and odd-q cordinates of a hex based on given odd-q. Use after creating a new tile to register its map coordinates.    
    /// </summary>
    /// <param name="q">the new column pos</param>
    /// <param name="r">the new row pos</param>
    public void SetCoord(int q, int r)
    {
        this.q = q;
        this.r = r;
        int x = q;
        int z = (r - (q + (q & 1)) / 2);
        int y = -x - z;

        cubeCord = new Vector3(x, y, z);
        Debug.Assert(x + z + y == 0);
    }

    // Raises up a hex and scales it based on the hex's Size value. Right now, this can cause mistakes in distance if called on an already non-default sized tile 
    // with out calling reset size first. Recomend tweaking the two methods into one that call the other when needed, but the trick is doing that and having it
    // not make reading AND dynamicly sizing at run time a pain. Currently not a problem if acounted for in other sources like DrawTool. But I am aware this 
    // can and will need to be done better.
    public void SizeUp()
    {
        gameObject.transform.localScale = Vector3.right * 0.96f + Vector3.up * (((int)size) + 1) * sizeConstant + Vector3.forward * 0.96f;
        gameObject.transform.position += Vector3.up * ((int)size) * 0.1f;
    }

    // Resets a tile to it's neutral scale and posistion from what it had been beforehand.
    public void ResetSize()
    {
        gameObject.transform.position -= Vector3.up * ((int)size) * 0.1f;
        gameObject.transform.localScale = Vector3.right * 0.96f + (Vector3.up * sizeConstant) + Vector3.forward * 0.96f;
        size = HexSize.normal;
    }

    public void SetType()
    {
        gameObject.GetComponent<Renderer>().material =  TileDictionary.TileStats[(int)tileType].Key;
        cost = TileDictionary.TileStats[(int)tileType].Value;
    }

    // Accessor  Methods
    public Unit Unit
    { get { return unit; } set { unit = value; } }
    public bool Passable
    { get { return passable; } set { passable = value; } }
    public Vector3 SpawnVector
    { get { return spawnPoint.transform.position + yVector; } }

    public Vector3 Cube
    { get { return cubeCord; } }

    public int CubeX
    { get { return (int)cubeCord.x; } }
    public int CubeZ
    { get { return (int)cubeCord.z; } }
    public int CubeY
    { get { return (int)cubeCord.y; } }

    public int Q
    { get { return q; } }
    public int R
    { get { return r; } }

    public int Cost
    { get { return cost; } set { cost = value; } }

    public  TileType Type
    { get { return tileType; } set { tileType = value; } }

    public HexSize Size
    { get { return size; } set { size = value; } }


    public override string ToString() // prints the hex cell by 
    {
        return "Hex Cell: q(" + q + ") r(" + r + ")";
    }


    //Static Methods

    // Given the inital distance and distance travled (a vector with a magnitude), 
    // where on the map in hex cordinates is returned.
    public static Vector2 NewLocationInOffset(Vector3 initalDistance, Vector3 distanceTravled)  
    {
        Vector3 destination = initalDistance + distanceTravled;

        //   int nx = (int)destination.x;
        //   int nz = (int)destination.z + (((int)destination.x + ((int)destination.x & 1)) / 2);

        return HexCell.OffsetCord(destination);
    }
    
    // Returns the direction vector in cube cordinates of two hex cells.
    public static Vector3 TravelDirection(Vector3 from, Vector3 to)
    {
        Vector3 result = to - from;
        return result;
    }

    // Returns at what angle (cube cordinates) two hex cells are 
    public static Vector3 TravelAngle(Vector3 start, Vector3 end)
    {
        Vector3 dir = TravelDirection(start, end);

        dir.Normalize();

        float xzd = Mathf.Sqrt(dir.x * dir.x + dir.z * dir.z);

        Vector3 angl = new Vector3(Mathf.Rad2Deg*(-Mathf.Atan2(dir.y, xzd)), Mathf.Rad2Deg *(Mathf.Atan2(dir.x, dir.z)), 0.0f);

        return angl;
    }

    /* public static void CubeToOffset(out int q, out int r, HexCell h)
     {
         q = h.X;
         r = h.Z + ((h.X + (h.X & 1)) / 2);
     }*/

    /*  public static void OffsetToCube(out int x, out int z, out int y, HexCell h)
      {
          x = h.Q;
          z = (h.R - (h.Q + (h.Q & 1)) / 2);
          y = -x - z;
      }*/

    // Convert cube cordinates to offset (hex) cordinates when given a hex cell
    public static Vector2 OffsetCord(HexCell h)
    {
        return new Vector2(h.CubeX, h.CubeZ + ((h.CubeX + (h.CubeX & 1)) / 2));
    }

    // Convert cube cordinates to offset (hex) cordinates when given a vector3
    public static Vector2 OffsetCord(Vector3 cube)
    {
        return new Vector2((int)cube.x, (int)cube.z + (((int)cube.x + ((int)cube.x & 1)) / 2));

    }

    // Translates offset (hex) cordinates to cube cordinates when given a hex cell
    public static Vector3 OffsetToCube(HexCell h)
    {
        int x = h.Q;
        int z = (h.R - (h.Q + (h.Q & 1)) / 2);
        int y = -x - z;

        return new Vector3(x, y, z);
    }

    // Translates offset (hex) cordinates to cube cordinates when given a vecotr2
    public static Vector3 OffsetToCube(Vector2 h)
    {
        int x = (int)h.x;
        int z = ((int)h.y - ((int)h.x + ((int)h.x & 1)) / 2);
        int y = -x - z;

        return new Vector3(x, y, z);
    }

    // returns the distance by cube cordinates of two hex cells
    public static int CubeDistance(HexCell a, HexCell b)
    {
        return (Mathf.Abs(a.CubeX - b.CubeX) + Mathf.Abs(a.CubeZ - b.CubeZ) + Mathf.Abs(a.CubeY - b.CubeY)) / 2;
    }

    // returns true if two hex cells are within the given range
    public static bool WithinMoveDistance(HexCell a, HexCell b, int range)
    {
        int d = CubeDistance(a, b);
        return (d <= range && d > 0);
    }

    // The diference (rounded) in height of two hex cells by their unity units
    public static int ElevationDifference(HexCell source, HexCell target)
    {
        return target.size - source.Size;
    }

    // the "cost" it would take to travel between two cells based on their height difference
    public static int CostToHex(HexCell source, HexCell target)
    {
        return target.cost + ElevationDifference(source, target);
    }
}