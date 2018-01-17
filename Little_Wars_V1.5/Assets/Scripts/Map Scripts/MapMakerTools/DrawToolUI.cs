using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawToolUI : MonoBehaviour {

    public enum ToolType { None,DrawTool,TerrainTool }

    public static int DTH_Num;  // The current drawStr.
    public static int DTR_Num;  // The current radius number for the draw tool.
    public static int DTMT;     // The current Draw Tool Material Type.
    public static int Tool_Num; // The current Tool mode active. 0 means no tools active.
    public static int DT_Mode;  // The current Draw Tool mode;

    public static int TT_Mode; // The current internal mode for the Terrain Tool.
    public static int curr_Terr; // The current terrain that can be placed.

    Text text;                      // Reference to the Text component.

    public Button placeTerr; // Reference to the Place Terrain Button
    public Button removeTerr; // Reference to the Remove Terrain Button
    public Button rotTerr; // Reference to the Rotate Terrain Button
    public Button drawHeight; //Reference to the Tile Height Button
    public Button drawType; //Reference to the Tile Type Button

    void Awake()
    {
        // Set up the reference.
        text = GetComponent<Text>();

        DTH_Num = 0;
        DTR_Num = 0;
        Tool_Num = 0;

        TurnOffAllButtons();
    }


    void Update()
    {
        // Display the relevant buttons and text for the current Map Tool.
        OnGui();
    }

    //Will handle displaying the appropriate GUI elements based on what mode the map maker tools are in.
    void OnGui()
    {
        switch(Tool_Num)
        {
            case (int)ToolType.None:
                text.text = "Tool Mode: None";
                TurnOffAllButtons();
                break;
            case (int)ToolType.DrawTool:
                text.text = "Tool Mode: " + (ToolType)Tool_Num + '\n' + "Draw Setting: " + (DrawTool.DrawMode)DT_Mode + '\n' + "Draw Tool Height: " + DTH_Num + '\n' + "Draw Tool Material Type: " + (DrawTool.MaterialTypes)DTMT + '\n' + "Draw Tool Radius: " + DTR_Num;
                TurnOffAllButtons();
                TurnOnDrawButtons();
                break;
            case (int)ToolType.TerrainTool:
                text.text = "Tool Mode: " + (ToolType)Tool_Num + '\n' + "Terrain Setting: " + (TerrainTool.TerrainToolMode)TT_Mode + '\n' + "Current Terrain: " + (TerrainTool.TerrainType)curr_Terr;
                TurnOffAllButtons();
                TurnOnTerrainButtons();
                break;
        }
    }

    void TurnOnTerrainButtons()
    {
        placeTerr.GetComponent<Image>().enabled = true;
        placeTerr.GetComponent<Button>().interactable = true;
        placeTerr.GetComponentInChildren<Text>().enabled = true;

        removeTerr.GetComponent<Image>().enabled = true;
        removeTerr.GetComponent<Button>().interactable = true;
        removeTerr.GetComponentInChildren<Text>().enabled = true;

        rotTerr.GetComponent<Image>().enabled = true;
        rotTerr.GetComponent<Button>().interactable = true;
        rotTerr.GetComponentInChildren<Text>().enabled = true;
    }

    void TurnOnDrawButtons()
    {
        drawHeight.GetComponent<Image>().enabled = true;
        drawHeight.GetComponent<Button>().interactable = true;
        drawHeight.GetComponentInChildren<Text>().enabled = true;

        drawType.GetComponent<Image>().enabled = true;
        drawType.GetComponent<Button>().interactable = true;
        drawType.GetComponentInChildren<Text>().enabled = true;
    }

    void TurnOffAllButtons()
    {
        placeTerr.GetComponent<Image>().enabled = false;
        placeTerr.GetComponent<Button>().interactable = false;
        placeTerr.GetComponentInChildren<Text>().enabled = false;

        removeTerr.GetComponent<Image>().enabled = false;
        removeTerr.GetComponent<Button>().interactable = false;
        removeTerr.GetComponentInChildren<Text>().enabled = false;

        rotTerr.GetComponent<Image>().enabled = false;
        rotTerr.GetComponent<Button>().interactable = false;
        rotTerr.GetComponentInChildren<Text>().enabled = false;

        drawHeight.GetComponent<Image>().enabled = false;
        drawHeight.GetComponent<Button>().interactable = false;
        drawHeight.GetComponentInChildren<Text>().enabled = false;

        drawType.GetComponent<Image>().enabled = false;
        drawType.GetComponent<Button>().interactable = false;
        drawType.GetComponentInChildren<Text>().enabled = false;

    }

    /*    
    void TurnOffTerrainButtons()
    {
        placeTerr.GetComponent<Image>().enabled = false;
        placeTerr.GetComponent<Button>().interactable = false;
        placeTerr.GetComponentInChildren<Text>().enabled = false;

        removeTerr.GetComponent<Image>().enabled = false;
        removeTerr.GetComponent<Button>().interactable = false;
        removeTerr.GetComponentInChildren<Text>().enabled = false;

        rotTerr.GetComponent<Image>().enabled = false;
        rotTerr.GetComponent<Button>().interactable = false;
        rotTerr.GetComponentInChildren<Text>().enabled = false;

    
    void TurnOffDrawButtons()
    {
        drawHeight.GetComponent<Image>().enabled = false;
        drawHeight.GetComponent<Button>().interactable = false;
        drawHeight.GetComponentInChildren<Text>().enabled = false;

        drawType.GetComponent<Image>().enabled = false;
        drawType.GetComponent<Button>().interactable = false;
        drawType.GetComponentInChildren<Text>().enabled = false;
    }
    }*/
}
