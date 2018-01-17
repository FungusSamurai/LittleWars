using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapLoader // what it says on the tin. Probably should be made static down the line
{
    enum CellData // an enum to make the reading of what is being taken from the file a little less annoying to understand 
    { q,r,unit,unitRotation,size,type }

    const char DELIM = ':'; // what each data Vector2 is split up by in the files
    private StreamReader sR; // for reading from the file
    private List<HexCell> toBuild; // a list to store hexes marked as having terrain on them
    private List<HexCell> toSize; // a list to store hexes marked as needing to be resized
    private List<HexCell> toType; // a list to store hexes marked as needing to have their type changed.
    private TileDictionary dictionary;

    static GameObject cube1, cube2;

    public MapLoader() //  initializes the variables
    {
        toBuild = new List<HexCell>();
        toSize = new List<HexCell>();
        toType = new List<HexCell>();
        dictionary = new TileDictionary();
    }

    public void LoadMapFromTextFile(string path) // loads the map from a given path, needs better exception handling 
    {
        string[] lines = File.ReadAllText(path).Split('\n');

        LoadMap(lines);
    }

    public void LoadMapFromTextAsset(TextAsset t) // loads the map from a given path, needs better exception handling 
    {
        string[] lines = t.text.Split('\n');

        LoadMap(lines);
    }

    private void LoadMap(string [] lines)
    {
        string rawData; // the entire line of text read from the file
        string[] dData; // the data after being split up based on the given delim above.

        string data = lines[0];

        int rad = System.Int32.Parse(data);
        MapMaster.SetMap(rad);

        for (int i = 1; i < lines.Length - 1; i++)
        {
            rawData = lines[i];
            dData = rawData.Split(DELIM);

            if (dData.Length >= 5)
            {
                int q = int.Parse(dData[(int)CellData.q]);
                int r = int.Parse(dData[(int)CellData.r]);
                Unit u = ((dData[(int)CellData.unit] == "null") ? null : MapMaster.MakeTerrain(dData[(int)CellData.unit])); // if the hex has any terrain, the Get Terrain method returns the related type after spawning it.
                if (u != null)
                {
                    u.UDirection = int.Parse(dData[(int)CellData.unitRotation]); // rotation of the unit
                }
                HexSize s = (HexSize)int.Parse((dData[(int)CellData.size]));
                TileType type;

                if (dData.Length <= (int)CellData.type)
                {
                    type = 0;
                }
                else
                {
                    type = (TileType)int.Parse((dData[(int)CellData.type]));
                }

                MapMaster.MakeTile(q, r);

                if (u != null) // depending on the natue of how we handle loading terrain, we might be checking for the string "null" here instead and not loading the object itself until we get to TerranPass
                {
                    MapMaster.Map[r, q].Unit = u;
                    MapMaster.Map[r, q].Unit.CurrentHex = MapMaster.Map[r, q];
                    toBuild.Add(MapMaster.Map[r, q]);
                }

                if ((int)s != 0)
                {
                    MapMaster.Map[r, q].Size = s;
                    toSize.Add(MapMaster.Map[r, q]);
                }

                if((int)type != 0)
                {
                    MapMaster.Map[r, q].Type = type;
                    toType.Add(MapMaster.Map[r, q]);
                }
            }
        }

        SizePass(); // any cells needing to have their scale changed are handled here.
        TerrainPass(); // any cells that are to have terrain is handled here.
        TypePass(); //any cells that need to have their cost and matieral changed are handled here.

        ViewMaster.PlaceCubes();
    }

    private void SizePass() 
    {
        foreach (HexCell h in toSize)
        {
            h.SizeUp();
        }
    }

    //will be filled in once the needs of placing terrain are better understood
    private void TerrainPass()
    {
        foreach (HexCell h in toBuild)
        {
            ITerrain iT = (ITerrain)h.Unit; // references to that units ITerrain implementation
            iT.SetTiles(); // changes the tilles the terrain is covering to not passable
            iT.SetPosition(); // places the terrain at the proper location
            iT.SetCollider(); // turns on ther collider(s) and sets any other rigidbody data that needs to be changed
            h.Unit.transform.rotation = h.Unit.URotation();
        }
    }

    private void TypePass()
    {
        foreach (HexCell h in toType)
        {
            h.SetType();
        }
    }

    
}
