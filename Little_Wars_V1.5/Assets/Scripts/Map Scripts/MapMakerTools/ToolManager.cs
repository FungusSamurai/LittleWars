using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolManager : MonoBehaviour // allows for switching between tools
{
    private MapToolbox mT;
    private int mode;

    // Use this for initialization
    void Start ()
    {
        mT = new MapToolbox();
        mode = -1;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.anyKeyDown)
        {
            SetMode();
        }

        RunMode();
    }

    void SetMode()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Disables any tools so you can move around without changing stuff
        {
            mode = -1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1)) // Activates the draw tool for terrain and its related keys
        {
            mode = (int)ToolTypes.DrawTool;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // Activates the terrain tool for terrain and its related keys
        {
            mode = (int)ToolTypes.TerrainTool;
        }

        DrawToolUI.Tool_Num = mode + 1;
    }

    void RunMode() // drives the ToolBox and Behavior Scripts as a result
    {
        if (mode != -1)
        {
            mT.RunTool(mode);
        }
    }

    //When Touching UI
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
