using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player
{
    public Vector3 CameraPosistion;
    public Quaternion CameraRotation;

    public float CameraZoom;

    public List<Unit> Inf;
    public List<Unit> Cav; 
    public List<Cannon> Can;
    public List<HexCell> backLine;
    public List<string> UnitOrder;

    public Text scoretext;
    public int Score;
    public int winZone;
    public int numMoved;

    // Use this for initialization
    public Player()
    {
        CameraRotation = new Quaternion();
        CameraPosistion = new Vector3();
        CameraZoom = 60.0f;
        Inf = new List<Unit>();
        Cav = new List<Unit>();
        Can = new List<Cannon>();
        backLine = new List<HexCell>();
        UnitOrder = new List<string>();
        Score = 0;
        numMoved = 0;
        winZone = -1;
    }

    public Player(Quaternion rot)
    {
        CameraPosistion = new Vector3();
        CameraRotation = rot;
        CameraZoom = 60.0f;
        Inf = new List<Unit>();
        Cav = new List<Unit>();
        Can = new List<Cannon>();
        backLine = new List<HexCell>();
        UnitOrder = new List<string>();
        Score = 0;
        numMoved = 0;
        winZone = -1;
    }

    public void DeleteUnit(Unit u)
    {
        switch (u.UName)
        {
            case "Infantry":
                Inf.Remove(u);
                break;
            case "Cavalry":
                Cav.Remove(u);
                break;
            default:
                Debug.Log("Tried to delete " + u.UName);
                break;
        }
    }

    public void ScoreDisplay(int i)
    {

    }

    public void ResetMovable()
    {
        foreach (Unit u in Inf)
        {
            u.Moved = false;
        }
        foreach (Unit u in Cav)
        {
            u.Moved = false;
        }
        foreach (Cannon c in Can)
        {
            CannonMaster.SetCannon(c);
        }
    }

}
