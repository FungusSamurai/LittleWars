using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class UnitDistributionMaster : MonoBehaviour {
    private const int unitCap = 84;
    public enum UnitType { INFANTRY, CAVALRY, CANNONS }

    private static int[] p1Distribution = new int[4] { 20, 12, 4, 84 }, p2Distribution = new int [4] { 20, 12, 4, 84 }; //Holds the distribution of units as well as the points total for each distribution. 0 = inf, 1 = cav, 2 = cann, 3 = points total.
    static int[] standardDist = new int[] { 20, 12, 4, 84 }; //Holds a standard distribution to use as a starter.
    int[] unitLimits; //Holds the upper limit for each unit type. Indices of the array correspond to the UnitType.
    const char DELIM = ':'; // what each data Vector2 is split up by in the files
    int iCost, cCost, cnCost, iCap, cCap, cnCap, pointsLimit;
    UnitType currUnitP1, currUnitP2;
    

    StreamWriter sW;
    Button startButton;
    static public bool correctDist;

    public string p1ListName, p2ListName;
    public Text p1ArmyTracker;
    public Text p1UnitAmount;
    public Text p1CavAmount;
    public Text p1CanAmount;
    public Text p2ArmyTracker;
    public Text p2UnitAmount;
    public Text p2CavAmount;
    public Text p2CanAmount;
    public Button p1Inf;
    public Button p1Cav;
    public Button p1Can;
    public Button p2Inf;
    public Button p2Cav;
    public Button p2Can;

    static Button[] buttons;
    static Text[] ratios1;
    static Text[] ratios2;

    Button currentP1;
    Button currentP2;

    void Start()
    {
        buttons = new Button[] { p1Inf, p1Cav, p1Can, p2Inf, p2Cav, p2Can };
        ratios1 = new Text[] { p1UnitAmount, p1CavAmount, p1CanAmount };
        ratios2 = new Text[] { p2UnitAmount, p2CavAmount, p2CanAmount };
        p1ListName = "Player_1_List";
        p2ListName = "Player_2_List";
        p1Distribution = new int[4];
        p2Distribution = new int[4];
        standardDist.CopyTo(p1Distribution, 0);
        standardDist.CopyTo(p2Distribution, 0);
        correctDist = false;
        pointsLimit = 84;
        iCost = 1;
        cCost = 2;
        cnCost = 10;
        iCap = 30;
        cCap = 24;
        cnCap = 6;
        unitLimits = new int[] { iCap, cCap, cnCap };
        currUnitP1 = UnitType.INFANTRY;
        currUnitP2 = UnitType.INFANTRY;
        UpdateArmyTrackers();
        startButton = GameObject.Find("Start").GetComponent<Button>();

        UpdateDisplayedAmmount(p1UnitAmount, P1Dist[0], unitLimits[(int)UnitType.INFANTRY]);
        UpdateDisplayedAmmount(p1CavAmount, P1Dist[1], unitLimits[(int)UnitType.CAVALRY]);
        UpdateDisplayedAmmount(p1CanAmount, P1Dist[2], unitLimits[(int)UnitType.CANNONS]);
        UpdateDisplayedAmmount(p2UnitAmount, P2Dist[0], unitLimits[(int)UnitType.INFANTRY]);
        UpdateDisplayedAmmount(p2CavAmount, P2Dist[1], unitLimits[(int)UnitType.CAVALRY]);
        UpdateDisplayedAmmount(p2CanAmount, P2Dist[2], unitLimits[(int)UnitType.CANNONS]);

        foreach (Button b in buttons)
        {
          //  b.interactable = false;
        }
        buttons[0].interactable = false;
        buttons[3].interactable = false;
        //  buttons[3].interactable = true;

        currentP1 = buttons[0];
        currentP2 = buttons[3];

    }

    public void UpdateDisplayedAmmount(Text t, int used, int total)
    {
        t.text = (used + "\n--\n" + total);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
            resetDistribution(p1Distribution);


        if (Input.GetKeyDown(KeyCode.Alpha2))
            resetDistribution(p2Distribution);

        if (Input.GetKeyDown(KeyCode.S) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))) // X + Alt key to save the current map
        {
            SaveList(Application.dataPath + "/Resources/ArmyLists/" + p1ListName + ".txt", p1Distribution);
        }

        if (Input.GetKeyDown(KeyCode.X) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))) // X + Alt key to save the current map
        {
            SaveList(Application.dataPath + "/Resources/ArmyLists/" + p2ListName + ".txt", p2Distribution);
        }

        if (Input.GetKeyDown(KeyCode.O) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))) // X + Alt key to save the current map
        {
            LoadListFromTextAsset((TextAsset)Resources.Load("ArmyLists/" + p1ListName), p1Distribution);
            UpdateArmyTrackers();
        }

        if (Input.GetKeyDown(KeyCode.L) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))) // X + Alt key to save the current map
        {
            LoadListFromTextAsset((TextAsset)Resources.Load("ArmyLists/" + p2ListName), p2Distribution);
            UpdateArmyTrackers();
        }
    }

    public void AddUnitPlayer1()
    {
        AddUnit(currUnitP1, p1Distribution);
        UpdateArmyTrackers();
    }

    public void AddUnitPlayer2()
    {
        AddUnit(currUnitP2, p2Distribution);
        UpdateArmyTrackers();
    }

    public void RemoveUnitPlayer1()
    {
        RemoveUnit(currUnitP1, p1Distribution);
        UpdateArmyTrackers();
    }

    public void RemoveUnitPlayer2()
    {
        RemoveUnit(currUnitP2, p2Distribution);
        UpdateArmyTrackers();
    }

    public void ChangeUnitPlayer1(int i)
    {
        currUnitP1 = (UnitType)i;
        /*  switch(currUnitP1)
          {
              case UnitType.INFANTRY:
                  currUnitP1 = UnitType.CAVALRY;
                 // currUnitP1Text.text = currUnitP1.ToString();
                  break;
              case UnitType.CAVALRY:
                  currUnitP1 = UnitType.CANNONS;
               //   currUnitP1Text.text = currUnitP1.ToString();
                  break;
              case UnitType.CANNONS:
                  currUnitP1 = UnitType.INFANTRY;
               //   currUnitP1Text.text = currUnitP1.ToString();
                  break;
          }*/
    }

    public void SetCurrentP1(int b)
    {
        currentP1.interactable = true;
        currentP1 = buttons[b];
        buttons[b].interactable = false;
     //   currUnitP1 = (UnitType)u;
    }

    public void SetCurrentP2(int b)
    {
        currentP2.interactable = true;
        currentP2 = buttons[b];
        buttons[b].interactable = false;
    }

    public void ChangeUnitPlayer2(int i)
    {
        currUnitP2 = (UnitType)i;
        /*   switch (currUnitP2)
           {
               case UnitType.INFANTRY:
                   currUnitP2 = UnitType.CAVALRY;
                 //  currUnitP2Text.text = currUnitP2.ToString();
                   break;
               case UnitType.CAVALRY:
                   currUnitP2 = UnitType.CANNONS;
               //    currUnitP2Text.text = currUnitP2.ToString();
                   break;
               case UnitType.CANNONS:
                   currUnitP2 = UnitType.INFANTRY;
               //    currUnitP2Text.text = currUnitP2.ToString();
                   break;
           }*/

    }


    void UpdateArmyTrackers()
    {
        p1ArmyTracker.text =  p1Distribution[3] + "/" + unitCap;
        p2ArmyTracker.text =  p2Distribution[3] + "/" + unitCap;
        checkTotalPoints();
    }

    public void UpdateP1UnitAmount()
    {
        UpdateDisplayedAmmount(ratios1[(int)currUnitP1], P1Dist[(int)currUnitP1], unitLimits[(int)currUnitP1]);
    }

    public void UpdateP2UnitAmount()
    {
        UpdateDisplayedAmmount(ratios2[(int)currUnitP2], P2Dist[(int)currUnitP2], unitLimits[(int)currUnitP2]);
    }

    void AddUnit(UnitType uType, int[] playerDist)
    {
        if (playerDist[(int)uType] < unitLimits[(int)uType])
        {
            playerDist[(int)uType] += 1;
            switch ((int)uType)
            {
                case 0:
                    playerDist[3] += iCost;
                    break;
                case 1:
                    playerDist[3] += cCost;
                    break;
                case 2:
                    playerDist[3] += cnCost;
                    break;
            }
        }
    }

    void RemoveUnit(UnitType uType, int[] playerDist)
    {
        if(playerDist[(int)uType] > 0)
        {
            playerDist[(int)uType] -= 1;
            switch ((int)uType)
            {
                case 0:
                    playerDist[3] -= iCost;
                    break;
                case 1:
                    playerDist[3] -= cCost;
                    break;
                case 2:
                    playerDist[3] -= cnCost;
                    break;
            }
        }

    }

    void checkTotalPoints()
    {
        if (p1Distribution[3] == pointsLimit && p2Distribution[3] == pointsLimit)
        {
            correctDist = true;

            if (MapSelectDriver.mapChosen)
            {
                startButton.interactable = true;
            }
        }
        else
        {
            correctDist = false;
            startButton.interactable = false;
        }

        //Debug.Log("Player 1 Army: {" + p1Distribution[0] + " " + p1Distribution[1] + " " + p1Distribution[2] + "} Total Points: " + p1Distribution[3]);
        //Debug.Log("Player 2 Army: {" + p2Distribution[0] + " " + p2Distribution[1] + " " + p2Distribution[2] + "} Total Points: " + p2Distribution[3]);

    }
    void resetDistribution(int[] playerDist)
    {
        standardDist.CopyTo(playerDist, 0);
        UpdateArmyTrackers();
    }

    // Loops through the ArmyLists folder and saves it to a file.
    public void SaveList(string path, int[] playerDist)
    {

        if (!File.Exists(path))
        {
            File.Create(path).Close();
        }

        using (sW = new StreamWriter(path, false))
        {
            sW.Write(playerDist[0] + ":");
            sW.Write(playerDist[1] + ":");
            sW.Write(playerDist[2] + ":");
            sW.Write(playerDist[3]);
            sW.Close();
        }
    }

    public void LoadListFromTextFile(string path, int[] playerDist) // loads the map from a given path, needs better exception handling 
    {
        string lines = File.ReadAllText(path);

        LoadList(lines, playerDist);
    }

    public static void LoadListFromTextAsset(TextAsset t, int[] playerDist) // loads the map from a given path, needs better exception handling 
    {
        string lines = t.text;

        LoadList(lines, playerDist);
    }

    private static void LoadList(string lines, int[] playerDist)
    {
        string[] data = lines.Split(DELIM);
        for(int i = 0; i < playerDist.Length; i++)
        {
            playerDist[i] = int.Parse(data[i]);
        }
    }

    public static int[] P1Dist
    {
        get { return p1Distribution; }
    }

    public static int[] P2Dist
    {
        get { return p2Distribution; }
    }

    /*
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Calculates Total Points in current distribution.
    foreach(int i in currentDistribution)
        {
            switch(i)
            {
                case 0:
                    totalPoints += i * iCost;
                    break;
                case 1:
                    totalPoints += i * cCost;
                    break;
                case 2:
                    totalPoints += i * cnCost;
                    break;
            }
        }
    */
}
