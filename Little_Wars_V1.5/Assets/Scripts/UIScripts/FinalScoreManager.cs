using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalScoreManager : MonoBehaviour {
    public static int Score1 = 0;
    public static int Score2 = 0;

    private int winner;

	// Use this for initialization
	void Start () {

        GameObject.Find("Score1").GetComponent<Text>().text = "Player 1: " + Score1;
        GameObject.Find("Score2").GetComponent<Text>().text = "Player 2: " + Score2;

        Animator anim1 = GameObject.Find("InfFinal_B").GetComponent<Animator>();
        Animator anim2 = GameObject.Find("InfFinal_R").GetComponent<Animator>();

        string winner = "Winner: ";

        if (Score1 > Score2)
        {
            winner += "Player 1!";
            anim1.Play("HuzzahStart");
            anim2.Play("CryStart");
        }
        else if (Score1 == Score2)
        {
            winner += "Draw!";
            anim1.Play("CryStart");
            anim2.Play("CryStart");
        }
        else
        {
            winner += "Player 2!";
            anim2.Play("HuzzahStart");
            anim1.Play("CryStart");
        }

        GameObject.Find("winner").GetComponent<Text>().text = winner;


    }

    public static void ResetScore()
    {
        Score1 = 0;
        Score2 = 0;
    }
}
