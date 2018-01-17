using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FightSceneMaster {

    static bool fighting; //Bool that lets the GameBrain know when FightSceneMaster is doing things
    static List<Unit> fighters, toKill; //List of all the units fighting and a list of which units need to be killed
    static Vector2 p1Start, p2Start;
    static Dictionary<Unit, GameObject> UnitDict; //Dictionary matching the picture of the units with their actual gameObject
    static float screenheight = Screen.height;
    static float screenwidth = Screen.width;

	public static void SetUpFight(List<Unit> f, List<Unit> tk)
    {
        fighting = true; //Fighting has begun
        UnitDict = new Dictionary<Unit, GameObject>();
        p1Start = GameObject.Find("Player1Units").GetComponent<RectTransform>().anchoredPosition;
        p2Start = GameObject.Find("Player2Units").GetComponent<RectTransform>().anchoredPosition;
        fighters = f; //List of all the units participating in the fight
        toKill = tk; //Which units we are going to kill
        UIMaster.SetFightScenePanel(true); //Activates the Canvas for the FightScene
        UIMaster.SetPanelAlpha(true, (int)UIPannels.Fight); //Turns the alpha to 1 so that it is visible
        PlaceUnits();
    }

    static void PlaceUnits() //Places units on seperate sides of the screen and fills the P1Units and P2Units dictionaries
    {
        GameObject FightSceneCanvas = GameObject.Find("FightSceneCanvas");
        GameObject picture;

        //position of the units we are about to place. Will change every time a unit is placed
        //float picWidth;
        //float picHeight;
        int blueXPos= 0;
        int blueYPos = 30;
        int redXPos = 0;
        int redYPos = 30;

        for (int i = 0; i < fighters.Count; ++i)
        {
            if (fighters[i].Player == 0) //Blue team
            {
                if (fighters[i].gameObject.tag == "Infantry")
                    picture = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/UI/BlueInfantry"));
                else
                    picture = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/UI/BlueCavalry"));

                picture.transform.SetParent(FightSceneCanvas.transform.GetChild(2)); //Sets unit as a child to make moving all units easier
                picture.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f); //Sets scale to make picture the correct size
                picture.GetComponent<RectTransform>().anchoredPosition = new Vector2(blueXPos, blueYPos); //Sets location relative to parent's anchor
                blueYPos += 60; //The next picture will be placed higher
                if(blueYPos > 150) //If above a certain height (about 3 picture lengths)
                {
                    blueYPos = 30;
                    blueXPos -= 60; //The next picture will be placed to the side
                }
            }

            else //Red team (Check above for comments)
            {
                if (fighters[i].gameObject.tag == "Infantry")
                    picture = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/UI/RedInfantry"));
                else
                    picture = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/UI/RedCavalry"));

                picture.transform.SetParent(FightSceneCanvas.transform.GetChild(3));
                picture.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
                picture.GetComponent<RectTransform>().anchoredPosition = new Vector2(redXPos, redYPos);
                redYPos += 60;
                if (redYPos > 150)
                {
                    redYPos = 30;
                    redXPos += 60;
                }
            }

            if(!UnitDict.ContainsKey(fighters[i])) //To avoid inconsistent error
                UnitDict.Add(fighters[i], picture);
        }
    }

    public static IEnumerator ToBattle()
    {
        GameObject P1Side = GameObject.Find("Player1Units");
        GameObject P2Side = GameObject.Find("Player2Units");

        float increment = 0;
        //Vector2 meetingPoint = new Vector2(Screen.width/2 /*1000/2*/, 0);
        Vector2 meetingPoint = new Vector2((Screen.width / 2) - (Screen.width/8) /*1000/2*/, 0);
        bool handlingSmoke = false; //The smokecloud is currently not on the screen

        SoundMaster.AttackCharge();

        while(/*P1Side.transform.position.x < P2Side.transform.position.x*/ P1Side.GetComponent<RectTransform>().anchoredPosition.x < meetingPoint.x) //While the two sides have not crossed over each other
        {
            //Moves both sides closer to each other
            P1Side.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(p1Start, meetingPoint, increment);
            P2Side.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(p2Start, -meetingPoint, increment);
            increment += Time.deltaTime*0.25f;

            if (!handlingSmoke && increment > 0.8f) //If the smoke cloud is not on screen and P1 side has passed a certain point
            {
                fighters[0].StartCoroutine(HandleSmoke());
                handlingSmoke = true; //The smoke cloud is now on screen
            }
            yield return null;
        }
    }

    static IEnumerator HandleSmoke() //Enlarges and fades out the smoke cloud
    {
        GameObject smokeCloud = GameObject.Find("SmokeCloud");
        smokeCloud.GetComponent<CanvasGroup>().alpha = 1.0f; //Makes smoke visible
        Vector3 scaler = new Vector3(0.08f, 0.2f, 0.0f);
        bool endingFight = false; //The units are still "fighting" behind the smoke cloud. Becomes true once the smoke cloud is big enough to cover the units
        float timer = 0.7f;

        SoundMaster.AttackFighting();

        while (smokeCloud.GetComponent<CanvasGroup>().alpha > 0)
        {
            smokeCloud.transform.localScale += scaler; //Increases the size of the smoke cloud
            if (timer < 0) //If the smoke cloud is big enough
            {
                smokeCloud.GetComponent<CanvasGroup>().alpha -= Time.deltaTime / 2.5f; //Fade out smoke cloud

                if(!endingFight) //If we haven't started knocking down units
                {
                    KnockdownUnits(); 
                    endingFight = true;
                }
            }
            timer -= Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);
        fighting = false;
    }

    static void KnockdownUnits() //Moves killed units to the side and knocks them down
    {
        GameObject P1Side = GameObject.Find("Player1Units");
        GameObject P2Side = GameObject.Find("Player2Units");
        GameObject Winner = GameObject.Find("Winner");

        for(int i = 0; i < fighters.Count; ++i) //loops through units
        {
            if (toKill.Contains(fighters[i])) //If this particular unit dead
            {
                if (fighters[i].Player == 0) //If player 1's unit
                    UnitDict[fighters[i]].transform.Rotate(Vector3.forward * 90.0f); //Knocks the unit over
                else UnitDict[fighters[i]].transform.Rotate(Vector3.forward * -90.0f); //Knocked over the other way for player 2 units
            }

            else //If the unit lived
            {
                UnitDict[fighters[i]].transform.SetParent(Winner.transform); //Placed with the winners
            }
        }

        P1Side.GetComponent<RectTransform>().anchoredPosition = new Vector2(/*200*/ screenwidth/8, 0);
        P2Side.GetComponent<RectTransform>().anchoredPosition = new Vector2(/*-200*/ -screenwidth/8, 0);
    }

    public static void CleanUp() //Resets the FightScene for next time
    {
        GameObject smokeCloud = GameObject.Find("SmokeCloud");
        smokeCloud.transform.localScale = new Vector3(0.1f, 0.1f, 1); //Resets size of the smoke cloud
        GameObject p1Side = GameObject.Find("Player1Units");
        GameObject p2Side = GameObject.Find("Player2Units");
        GameObject winner = GameObject.Find("Winner");

        p1Side.GetComponent<RectTransform>().anchoredPosition = p1Start; //Resets the position of P1's units
        p2Side.GetComponent<RectTransform>().anchoredPosition = p2Start; //Similiar comment

        for (int i = p1Side.transform.childCount - 1; i >= 0; --i) //Removes pictures
            GameObject.Destroy(p1Side.transform.GetChild(i).gameObject);
        for (int i = p2Side.transform.childCount - 1; i >= 0; --i)
            GameObject.Destroy(p2Side.transform.GetChild(i).gameObject);
        for (int i = winner.transform.childCount - 1; i >= 0; --i)
            GameObject.Destroy(winner.transform.GetChild(i).gameObject);

        UnitDict.Clear(); //Clears dictionary]
        UIMaster.SetFightScenePanel(false); //Turns off the canvas for the FightScene
        UIMaster.SetPanelAlpha(false, (int)UIPannels.Fight);
    }

    public static bool Fighting
    {
        get { return fighting; }
    }
}
