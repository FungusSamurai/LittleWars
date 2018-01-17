using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawTool : MapTools // the map tool that lets us resize tiles and "draw" on the map during runtime
{
    bool canDraw; // allows us to turn off the drawing without disabling the tool compleatly 
    HexSize drawStr; // the current scale to set the hex tiles to
    private int multCellRadius; //stores the size of the mutli cell select setting

    float scroll;
    //public int cellRadiusMax; // public variable to set cap for mutli cell select 

    public enum MaterialTypes { Grass, Mud, Water }
    MaterialTypes currType;

    public enum DrawMode { Height, Type }
    DrawMode currMode;

    GameObject buttonGet;
    Button heightButton;
    Button typeButton;

    private int tileTypeMax;
    private int currTypeInt;

    public DrawTool()
    {
        canDraw = true;
        drawStr = HexSize.normal;
        multCellRadius = 0;
        scroll = 0;
        currMode = DrawMode.Height;
        currType = MaterialTypes.Grass;
        currTypeInt = 0;

        buttonGet = GameObject.Find("Tile Height Button");
        heightButton = buttonGet.GetComponent<Button>();
        heightButton.onClick.AddListener(HeightButtonTask);

        buttonGet = GameObject.Find("Tile Type Button");
        typeButton = buttonGet.GetComponent<Button>();
        typeButton.onClick.AddListener(TypeButtonTask);

        //Everytime a material type is added to the enum, update this. 
        tileTypeMax = 2; //int of last enum in MaterialTypes
    }

    public override void Behavior()
    {
        if (Input.anyKeyDown) // if any key is pressed, check to see if the draw str needs to be changed
        {
            //Debug.Log("Checking");
            if (currMode == DrawMode.Height)
                SetDrawStr();
            else if (currMode == DrawMode.Type)
                SetDrawType();            
        }

        if (Input.GetMouseButton(0)  && !ToolManager.IsPointerOverUIObject()/*Input.GetKeyDown(KeyCode.Mouse0)*/) // ray cast to a hex and see if it needs/can be resized
        {
            ClickHex();
        }

        if((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
            scroll = Input.GetAxis("Mouse ScrollWheel");

        if(scroll > 0 && multCellRadius < 5)
        {
            multCellRadius++;
            DrawToolUI.DTR_Num++;
        }

        if(scroll < 0 && multCellRadius > 0)
        {
            multCellRadius--;
            DrawToolUI.DTR_Num--;
        }

    }

    private void SetDrawStr() // these keys work for the NUMERIC keypad only while the escape key toggles if the tool is "on"
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canDraw = !canDraw;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            drawStr = HexSize.normal;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            drawStr = HexSize.grade1;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            drawStr = HexSize.grade2;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            drawStr = HexSize.grade3;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            drawStr = HexSize.grade4;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            drawStr = HexSize.grade5;
        }
        DrawToolUI.DTH_Num = (int)drawStr;
    }

    private void SetDrawType()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currTypeInt == tileTypeMax)
                currTypeInt = 0;
            else
                currTypeInt++;

            currType = (MaterialTypes)currTypeInt;

        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currTypeInt == 0)
                currTypeInt = tileTypeMax;
            else
                currTypeInt--;

            currType = (MaterialTypes)currTypeInt;
        }

        DrawToolUI.DTMT = (int)currType;
    }

    private void ClickHex() // checks if a valid hex was clicked
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        HexCell h;
        if (Physics.Raycast(ray, out rayHit))
        {
            h = rayHit.transform.gameObject.GetComponent<HexCell>();
            if (h != null) // it might be tempting to but this and the nested if statment together with an &&, but that'll throw an error if h is null when it check h.Unit
            {
                if (h.Unit == null && h.Passable)
                {
                    List<HexCell> CellsToChange = MapMaster.CellsWithinArea(h, multCellRadius);

                    foreach (HexCell cell in CellsToChange)
                    {
                        if (cell.R < 2 || cell.R > 3 && cell.R < MapMaster.MapRadius - 4 || cell.R > MapMaster.MapRadius - 3)
                        {
                            if (cell.Passable)
                                ChangeHex(cell);
                        }

                    }
                }
            }
        }
    }

    private void ChangeHex(HexCell hex) // if the hex is not already the scale that drawStr is set to, we reset it the normal scale if need be, then resize it to the new scale
    {
        if (canDraw && hex.Size != drawStr && currMode == DrawMode.Height)
        {
            if ((int)hex.Size != 0)
            {
                hex.ResetSize();
            }
            hex.Size = drawStr;
            hex.SizeUp();
        }

        if(canDraw && currMode == DrawMode.Type)
        {
            switch (currType)
            {
                case MaterialTypes.Grass:
                    hex.Type = TileType.Grass;
                    hex.SetType();
                    break;
                case MaterialTypes.Mud:
                    hex.Type = TileType.Mud;
                    hex.SetType();
                    break;
                case MaterialTypes.Water:
                    hex.Type = TileType.Water;
                    hex.SetType();
                    break;
            }
        }


    }

    private void HeightButtonTask()
    {
        currMode = DrawMode.Height;
        DrawToolUI.DT_Mode = (int)currMode;
    }

    private void TypeButtonTask()
    {
        currMode = DrawMode.Type;
        DrawToolUI.DT_Mode = (int)currMode;
    }
}
