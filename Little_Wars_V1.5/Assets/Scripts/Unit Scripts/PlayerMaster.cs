using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaster
{
    private static PlayerMaster instance;

    private Player[] players = new Player[2] { new Player(Quaternion.Euler(90, 0, 0)), new Player(Quaternion.Euler(90, 180 ,0)) };
    private int currentTurn = 0;
    private int priorTurn = 1;

    private static SoundMaster soundM;

    public PlayerMaster()
    {
        currentTurn = 0;
        priorTurn = 1;
        players = new Player[2] { new Player(Quaternion.Euler(60, 180, 0)), new Player(Quaternion.Euler(55, 0, 0)) };
        instance = this;
        soundM = GameObject.Find("SoundMaster").GetComponent<SoundMaster>();
    }

    public static void SetBackLines(HexCell[,] m)
    {
        int radius = m.GetLength(1);
        for (int r = 2; r <= 3; r++)
        {
            for (int q = 0; q < radius; q++)
            {
                instance.players[0].backLine.Add(m[r, q]);
                instance.players[1].backLine.Add(m[radius - 1 - r, q]);
            }
        }

        instance.players[0].winZone = MapMaster.MapRadius - 2;
        instance.players[1].winZone = 1;

    }

    public static void SwitchPlayer()
    {
        if (instance.currentTurn == 0)
        {
            instance.currentTurn = 1;
            instance.priorTurn = 0;
        }
        else
        {
            instance.currentTurn = 0;
            instance.priorTurn = 1;
        }
    }

    public static Player UnitsPlayer(int p)
    {
        return instance.players[p];
    }

    public static void RefreshMovement()
    {
        foreach (Player p in instance.players)
        {
            p.ResetMovable();
        }
    }

    public static void KillUnit(Unit unit) //Removes unit from the game world and the player's list of units and updates the score
    {
        MapMaster.Map[unit.CurrentHex.R, unit.CurrentHex.Q].Unit = null;
        MapMaster.Map[unit.CurrentHex.R, unit.CurrentHex.Q].Passable = true;
        ScoreMaster.UpdateScore(unit);
        instance.players[unit.Player].DeleteUnit(unit);
        GameObject skullbones = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/SkullAndBones"), 
            unit.gameObject.transform.position, unit.gameObject.transform.rotation); //Spawns Skull and Bones by the player

        if (skullbones.transform.position.y < 1)
            skullbones.transform.position = new Vector3(skullbones.transform.position.x, 1, skullbones.transform.position.z);

        soundM.UnitDeath();
        GameObject.Destroy(unit.gameObject);
    }

    public static Player GetPlayer(int p)
    { return instance.players[p]; }
    public static Player CurrentPlayer
    { get { return instance.players[instance.currentTurn]; } }
    public static Player OtherPlayer
    { get { return instance.players[instance.priorTurn]; } }
    public static int CurrentTurn
    { get { return instance.currentTurn; } }
    public static int PriorTurn
    { get { return instance.priorTurn; } }
}
