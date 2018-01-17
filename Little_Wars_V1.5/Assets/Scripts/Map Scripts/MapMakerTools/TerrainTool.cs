using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TerrainTool : MapTools {

    bool canPlace, canRotate, canRemove; //determines whether the tool is on/off for placing objects
    TerrainType currTerrain; // the current terrain to be placed by the tool
    TerrainToolMode currMode; // the current mode desired for the terrain tool. Default is Place = 0;
    string[] terrainArray; // holds the string names of the terrain pieces. I'm bad at names.
    GameObject buttonGet;

    Button placeButton;
    Button rotButton;
    Button remButton;

    public enum TerrainType{ SmlBox, MidBox, BigBox, RubbleSmall, RubbleLarge, Fence, Pitchfork, BarnTest, BarnRoofless }

    public enum TerrainToolMode { Off, Place, Rotate, Remove }

    private int terrTypeMax;
    private int currTerrInt;

    public TerrainTool()
    {
        currMode = TerrainToolMode.Off;
        currTerrain = TerrainType.SmlBox;
        currTerrInt = (int)currTerrain;
        terrainArray = new string[] { "SmlBox", "MidBox", "BigBox", "RubbleSmall", "RubbleLarge", "Fence", "Pitchfork", "BarnTest", "BarnRoofless" };

        buttonGet = GameObject.Find("Place Terr Button");
        placeButton =  buttonGet.GetComponent<Button>();
        placeButton.onClick.AddListener(PlaceButtonTask);

        buttonGet = GameObject.Find("Rotate Terr Button");
        rotButton = buttonGet.GetComponent<Button>();
        rotButton.onClick.AddListener(RotateButtonTask);

        buttonGet = GameObject.Find("Remove Terr Button");
        remButton = buttonGet.GetComponent<Button>();
        remButton.onClick.AddListener(RemoveButtonTask);

        terrTypeMax = terrainArray.Length-1;
    }

    public override void Behavior()
    {
        if (Input.anyKeyDown) // if any key is pressed, check to see if currTerrain needs to be changed
        {
            //Debug.Log("Checking");
            SetTerrain();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && !ToolManager.IsPointerOverUIObject()) // ray cast to a hex and see if it needs/can be resized
        {
            switch(currMode)
            {
                case TerrainToolMode.Off:
                    //nothing. tool's off.
                    break;
                case TerrainToolMode.Place:
                    ClickHex();
                    break;
                case TerrainToolMode.Rotate:
                    RotateTerrain();
                    break;
                case TerrainToolMode.Remove:
                    RemoveTerrain();
                    break;
            }
        }
       
    }

    private void SetTerrain() // these keys work for the NUMERIC keypad only while the escape key toggles if the tool is "on"
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            currMode = TerrainToolMode.Off;
            DrawToolUI.TT_Mode = (int)currMode;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currTerrInt == terrTypeMax)
                currTerrInt = 0;
            else
                currTerrInt++;

            currTerrain = (TerrainType)currTerrInt;
            
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currTerrInt == 0)
                currTerrInt = terrTypeMax;
            else
                currTerrInt--;

            currTerrain = (TerrainType)currTerrInt;
        }

        DrawToolUI.curr_Terr = (int)currTerrain;
    }

    private void ClickHex() // checks if a valid hex was clicked
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        HexCell h;
        if (Physics.Raycast(ray, out rayHit))
        {
            h = rayHit.transform.gameObject.GetComponent<HexCell>();
            if (h != null) // it might be tempting to put this and the nested if statment together with an &&, but that'll throw an error if h is null when it check h.Unit
            {
                if (h.Unit == null && h.Passable)
                {
                    //code to place current terrain object.
                    Unit u = MapMaster.MakeTerrain(terrainArray[(int)currTerrain]);
                    u.CurrentHex = h;
                    h.Unit = u;
                    ITerrain uIT = u.GetComponent<ITerrain>();
                    uIT.SetTiles();
                    uIT.SetPosition();
                    uIT.SetCollider();
                }
            }
        }
    }

    private void RemoveTerrain() // removes the clicked terrain and resets the tiles around it.
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Unit u;
        if (Physics.Raycast(ray, out rayHit))
        {
            u = rayHit.transform.gameObject.GetComponent<Unit>();
            if (u != null) // it might be tempting to put this and the nested if statment together with an &&, but that'll throw an error if h is null when it check h.Unit
            {
                ITerrain uIT = u.GetComponent<ITerrain>();
                uIT.ResetTiles();
                u.CurrentHex = null;
                uIT.DestroySelf();
                
            }
        }
    }

    private void RotateTerrain() // rotates the clicked terrain once 90 degrees clockwise
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Unit u;
        if (Physics.Raycast(ray, out rayHit))
        {
            u = rayHit.transform.gameObject.GetComponent<Unit>();
            if (u != null) // it might be tempting to put this and the nested if statment together with an &&, but that'll throw an error if h is null when it check h.Unit
            {
                switch(u.UDirection)
                {
                    case (int)UnitRotations.none:
                        u.UDirection = (int)UnitRotations.quarter;
                        u.transform.eulerAngles = new Vector3(0.0f, 90 * u.UDirection, 0.0f);
                        break;
                    case (int)UnitRotations.quarter:
                        u.UDirection = (int)UnitRotations.half;
                        u.transform.eulerAngles = new Vector3(0.0f, 90 * u.UDirection, 0.0f);
                        break;
                    case (int)UnitRotations.half:
                        u.UDirection = (int)UnitRotations.threeQ;
                        u.transform.eulerAngles = new Vector3(0.0f, 90 * u.UDirection, 0.0f);
                        break;
                    case (int)UnitRotations.threeQ:
                        u.UDirection = (int)UnitRotations.full;
                        u.transform.eulerAngles = new Vector3(0.0f, 90 * u.UDirection, 0.0f);
                        break;
                    case (int)UnitRotations.full:
                        u.UDirection = (int)UnitRotations.quarter;
                        u.transform.eulerAngles = new Vector3(0.0f, 90 * u.UDirection, 0.0f);
                        break;
                }
                    
            }
        }
    }

    private void PlaceButtonTask()
    {
       currMode = TerrainToolMode.Place;
        DrawToolUI.TT_Mode = (int)currMode;
    }

    private void RotateButtonTask()
    {
        currMode = TerrainToolMode.Rotate;
        DrawToolUI.TT_Mode = (int)currMode;
    }

    private void RemoveButtonTask()
    {
        currMode = TerrainToolMode.Remove;
        DrawToolUI.TT_Mode = (int)currMode;
    }
}
