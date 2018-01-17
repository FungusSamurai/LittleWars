using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreMaster
{
    static ScoreMaster instance;

    static int breachBonus = 100;
    static int[] pointValues = new int[] { Infantry.PointValue, Cavalry.PointValue, Cannon.PointValue };

    int score1;
    int score2;
    int winner;

    public ScoreMaster()
    {
        score1 = 0;
        score2 = 0;
        winner = -1;
        instance = this;
    }

    public static void UpdateScore(int player, int amount)
    {
        if (player == 0)
        {
            instance.score1 = amount;
        }
        else
        {
            instance.score2 = amount;
        }
    }

    static void FindWinner()
    {
        string winner = "Winner: ";

        if (instance.score1 > instance.score2)
        {
            winner += "Player 1!";
        }
        else if (instance.score1 == instance.score2)
        {
            winner += "Draw!";
        }
        else
        {
            winner += "Player 2!";
        }
    }

    public static void ResetScore()
    {
        instance.score1 = 0;
        instance.score2 = 0;
    }

    public static void UpdateScore(Unit u)
    {

        switch (u.UName)
        {
            case "Infantry":
                SwapPoints(UnitTypes.Infantry, u.Player);
                break;
            case "Cavalry":
                SwapPoints(UnitTypes.Cavalry, u.Player);
                break;
            case "Cannon":
                SwapPoints(UnitTypes.Cannon, u.Player);
                break;
            default:
                break;
        }
    }

    public static void SwapPoints(UnitTypes uT, int from)
    {
        int amt = pointValues[(int)uT];

        PlayerMaster.UnitsPlayer(from).Score -= amt;
        PlayerMaster.UnitsPlayer(from == 0 ? 1 : 0).Score += amt;
        instance.score1 = PlayerMaster.UnitsPlayer(0).Score;
        instance.score2 = PlayerMaster.UnitsPlayer(1).Score;
        UIMaster.DisplayScore();
    }

    public static void GivePoints(int amt, int p)
    {
        PlayerMaster.UnitsPlayer(p).Score += amt;
        if (p == 0)
        {
            instance.score1 = PlayerMaster.UnitsPlayer(p).Score;
        }
        else
        {
            instance.score2 = PlayerMaster.UnitsPlayer(p).Score;
        }
    }

    public static int[] PointValues
    {
        get { return pointValues; }
    }
    public static int Score1
    {
        get { return instance.score1; }
    }
    public static int Score2
    {
        get { return instance.score2; }
    }
    public static int BreachBonus
    {
        get { return breachBonus; }
    }
}
