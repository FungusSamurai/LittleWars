using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAI  {

    //static bool moving = false;
    static float distance;
    static List<Unit> ai_units;

    public TutorialAI()
    {
        ai_units = new List<Unit>();
    }
	
	public static IEnumerator Deploy()
    {
        GameBrain.ChangeAcceptInput(false);
        for (int i = 19; i < 29; ++i)
        {
            GameObject g = (GameObject)GameObject.Instantiate(Resources.Load("prefabs/Infantry1"), PlayerMaster.CurrentPlayer.backLine[i].SpawnVector, Quaternion.identity);
            ai_units.Add(g.GetComponent<Unit>());
            MapMaster.Map[PlayerMaster.CurrentPlayer.backLine[i].R, PlayerMaster.CurrentPlayer.backLine[i].Q].Unit = g.GetComponent<Unit>();
            MapMaster.Map[PlayerMaster.CurrentPlayer.backLine[i].R, PlayerMaster.CurrentPlayer.backLine[i].Q].Unit.Player = 1;
            MapMaster.Map[PlayerMaster.CurrentPlayer.backLine[i].R, PlayerMaster.CurrentPlayer.backLine[i].Q].Unit.CurrentHex = PlayerMaster.CurrentPlayer.backLine[i];
            PlayerMaster.CurrentPlayer.Inf.Add(g.GetComponent<Unit>());
            yield return new WaitForSeconds(0.2f);
        }
        //GameObject cannon = (GameObject)GameObject.Instantiate(Resources.Load("prefabs/Cannon"), PlayerMaster.CurrentPlayer.backLine[30].SpawnVector, Quaternion.identity);
        yield return GameBrain.ChangeTurn();
        //GameBrain.ChangeAcceptInput(true);
    }

    public static IEnumerator Move()
    {
        GameBrain.ChangeAcceptInput(false);
        yield return new WaitForSeconds(1.0f);
        HexCell moveTo;
        for (int i = 0; i < ai_units.Count; ++i)
        {
            if(ai_units[i] != null)
            {
                HexCell unit_cell = ai_units[i].CurrentHex;
                MoveMaster.SetTarget(ai_units[i]);
                moveTo = MapMaster.Map[ai_units[i].CurrentHex.R - 2, ai_units[i].CurrentHex.Q];
                MoveMaster.EvaluateTile(moveTo); //Actually moves unit
            }
        }
        yield return new WaitForSeconds(.75f);

        while (MoveMaster.movingUnits != 0)
        {
            yield return null;
        }

        yield return GameBrain.ChangeTurn();
        //GameBrain.ChangeAcceptInput(true);
    }

    public static IEnumerator IDontHaveCannons()
    {
        GameBrain.ChangeAcceptInput(false);
        yield return new WaitForSeconds(2.0f);
        yield return GameBrain.ChangeTurn();
        //GameBrain.ChangeAcceptInput(true);
    }
}
