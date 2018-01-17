using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapMaker : MonoBehaviour
{
    MapMaster mM;
    public string CurrentMap; //the file name of the current map being loaded. You can also change this during run time and hit the S key to save as a new map
    public int mapRadius;

    StreamWriter sW; // writes to a file
    MapLoader mL; // load the maps

    void Awake() // takes into account and calculates the physical parts of the hex game object and the constants by which we will space them out
    {
        ReadyHexmap();
    }

    // if a given file name exists, load it. If not, make a blank map and save it under the given file name. All maps go to their sub-folder in the resources folder.
    void ReadyHexmap()
    {
        mM = new MapMaster();

        string path = Application.dataPath + "/Resources/Maps/" + CurrentMap + ".txt";

        if (!File.Exists(path))
        {
            //MapMaster.SetMap(MapMaster.MapRadius);
            MapMaster.SetMap(mapRadius);
            Debug.Log("Making");
            CreateBlankMap();
            File.Create(path).Close();
            SaveMap(path);
        }
        else
        {
            mL = new MapLoader();
            mL.LoadMapFromTextFile(path);
        }

        foreach (HexCell h in MapMaster.Map)
        {
            h.gameObject.isStatic = false;
        }
    }

    // Loops through the map collection and saves it to a file. Do not mess with the pattern of how it's input unless you are going to mirror the change in LoadMap
    private void SaveMap(string path)
    {

        if (!File.Exists(path))
        {
            File.Create(path).Close();
        }

        using (sW = new StreamWriter(path, false))
        {
            sW.WriteLine(MapMaster.MapRadius); // need to make it so map radius can't be changed after loading a map to help protect the file integrity 
            for (int x = 0; x < MapMaster.Map.GetLength(0); x++)
            {
                for (int z = 0; z < MapMaster.Map.GetLength(0); z++)
                {
                    sW.Write(MapMaster.Map[z, x].q + ":");
                    sW.Write(MapMaster.Map[z, x].r + ":");
                    sW.Write((MapMaster.Map[z, x].Unit == null ? ("null") : (MapMaster.Map[z, x].Unit.UName)) + ":");
                    sW.Write((MapMaster.Map[z, x].Unit == null ? ("null") : (MapMaster.Map[z, x].Unit.UDirection.ToString())) + ":");
                    sW.Write((int)MapMaster.Map[z, x].Size + ":");
                    sW.Write((int)MapMaster.Map[z, x].Type);
                    sW.WriteLine();
                }
            }

            sW.Close();
        }
    }

    // Takes a loaded map and flattens it back to a plain one.
    private void ResetMap()
    {
        foreach (HexCell h in MapMaster.Map)
        {
            h.ResetSize();
        }
        // We will also need code to remove any terain and reset the hex unit values to null once we get to that part of development
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))) // X + Alt key to save the current map
        {
            SaveMap(Application.dataPath + "/Resources/Maps/" + CurrentMap + ".txt");
        }
        if (Input.GetKeyDown(KeyCode.R) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
        {
            ResetMap(); // R + Alt key to reset a current map to a blank state
        }
    }

    // called if the desired map to read does not exsist, building a plain one instead.
    private void CreateBlankMap()
    {
        for (int x = 0; x < MapMaster.MapRadius; x++)
        {
            for (int z = 0; z < MapMaster.MapRadius; z++)
            {
                MapMaster.MakeTile(x, z);
            }
        }
     }
}