using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TutorialUIMaster {

    public static bool c_bool;
    static bool raised;
    static Text textbox; //Textbox that will be changed throughout the tutorial
    static string[][] tutorialText; //Jagged array containing all the tutorial text
    static GameObject t_panel, c_button, r_button, c_reference, t_reference, e_reference;
    static Sprite[] pictures;
    static Vector2 startPos;

	public TutorialUIMaster()
    {
        raised = true;
        c_bool = true;
        textbox = GameObject.Find("TutorialText").GetComponent<Text>();
        t_panel = GameObject.Find("TextBox");
        c_button = GameObject.Find("Continue Button");
        r_button = GameObject.Find("Raise Button");

        c_reference = GameObject.Find("Cannon Reference");
        t_reference = GameObject.Find("Terrain Reference");
        e_reference = GameObject.Find("Elevation Reference");

        c_reference.SetActive(false);
        t_reference.SetActive(false);
        e_reference.SetActive(false);

        startPos = t_panel.GetComponent<RectTransform>().anchoredPosition;
        tutorialText = new string[6][];
        tutorialText[0] = new string[7]; //Intro
        tutorialText[1] = new string[13]; //Deploy
        tutorialText[2] = new string[19]; //Move
        tutorialText[3] = new string[14]; //Cannon
        tutorialText[4] = new string[8]; //Fight
        tutorialText[5] = new string[9]; //closing
        SetText();
    }
    
    public static IEnumerator MoveTextbox()
    {
        float lerp = 0;
        Vector2 begin = t_panel.GetComponent<RectTransform>().anchoredPosition;
        Vector2 end;
        if (raised)
        {
            t_panel.GetComponent<CanvasGroup>().interactable = false;
            end = new Vector2(startPos.x, -100);
        }

        else
        {
            end = startPos;
            SetRaiseButton(false);
        }

        while(lerp < 1)
        {
            lerp += .02f;
            t_panel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(begin, end, lerp);
            yield return null;
        }

        if (raised)
            SetRaiseButton(true);
        else
        {
            SetContinue(c_bool);
            c_bool = true;
            t_panel.GetComponent<CanvasGroup>().interactable = true;
        }
        raised = !raised;
    }

    static void SetRaiseButton(bool on)
    {
        r_button.GetComponent<CanvasGroup>().alpha = on ? 1 : 0;
        r_button.GetComponent<CanvasGroup>().interactable = on;
    }

    public static void SetContinue(bool on)
    {
        c_button.GetComponent<CanvasGroup>().alpha = on ? 1 : 0;
        c_button.GetComponent<CanvasGroup>().interactable = on;
    }

    public static void ChangeText(int phase, int num)
    {
            textbox.text = tutorialText[phase][num];
    }

    public static void SetCannonImage(bool on)
    {
        c_reference.SetActive(on);
    }

    public static void SetTerrainImage(bool on)
    {
        t_reference.SetActive(on);
    }

    public static void SetElevationImage(bool on)
    {
        e_reference.SetActive(on);
    }

    static void SetText()
    {
        SetIntroText();
        SetDeployText();
        SetMoveText();
        SetCannonText();
        SetFightText();
        SetClosingText();
    }

    static void SetIntroText()
    {
        tutorialText[0][0] = "Hello and welcome to a\nLittle Wars Tutorial. " +
            "Here\nwe will give you a \nquick run down of how to\nplay Little Wars.";
        tutorialText[0][1] = "Your goal in Little Wars is\nto gain as many points as\nyou can by " + 
            "killing enemy\nUnits and breaching your\nopponent's backline.";
        tutorialText[0][2] = "Points are calculated\nbased on Units, of which\nthere are 3 types, " +
            "Infantry,\nCavalry, and Cannons.";
        tutorialText[0][3] = "You get points for each\nUnit you own (Infantry = 1,\nCavalry = 2, Cannons = 10).";
        tutorialText[0][4] = "When a Unit is either\nkilled or taken those\npoints are transferred\nto the opponent.";
        tutorialText[0][5] = "Breaching the opponent's\nbackline is worth a\nwhopping 100 points! " +
            "So\nmake sure you keep your\nside of the map protected.";
        tutorialText[0][6] = "Little Wars is played by\nprogressing through\n4 phases: " +
            "Deploy, Move,\nCannon, and Attack. Let's\nstart with Deploy.";

    }

    static void SetDeployText()
    {
        tutorialText[1][0] = "Welcome to the Deploy\nPhase. See that Panel that\njust faded in to our left?" +
            "\nThat is the Action Panel.";
        tutorialText[1][1] = "On the right half of the\nAction Panel there are four\n" +
            "buttons that are used\nduring the Deploy Phase.";
        tutorialText[1][2] = "These four buttons are\n1) Infantry   2) Cavalry\n3) Cannon   4) Remove" +
            "\nThe first 3 are for choosing\nwhich unit to place.";
        tutorialText[1][3] = "The fourth is for removal.\nPressing this button then\nselecting placed Units\n" +
            "will remove any Unit you\npreviously chose.";
        tutorialText[1][4] = "See those black numbers\nunderneath the buttons?\nThat indicates how many" +
            "\nof that Unit you can still\nplace.";
        tutorialText[1][5] = "Remember, you get points\nfor each Unit you own. Try" +
            "\nto place as many as you\ncan fit on the board.";
        tutorialText[1][6] = "All deployment is done\nonce at the beginning of\nthe game so you should" +
            "\ncarefully consider where to\nplace each Unit.";
        tutorialText[1][7] = "Lastly, see that blue strip\nof hexagons? That's your\nbackline " +
            "(the line your\nopponent has to cross to\ngain the 100 point bonus.)";
        tutorialText[1][8] = "Units can ONLY be\ndeployed on your backline.";
        tutorialText[1][9] = "Now it's your turn. To place\na Unit, press that Unit's\ncorresponding button then" +
            "\ntouch one of the Hexagons\non your backline.";
        tutorialText[1][10] = "For the sake of the tutorial\nwe're going to keep things\nsimple and " +
            "limit how many\nunits we're going to\ndeploy.";
        tutorialText[1][11] = "We're also going to guide\nyou using a sample\ndeployment. Try to match" +
            "\nthe symbol of the\nupcoming white hexagons.";
        tutorialText[1][12] = "(I = Infantry, CV = Cavalry,\nCN = Cannon). Once you're\ndone placing units, press" +
            "\nthe giant face on the left\nside of the action panel.";
    }

    static void SetMoveText()
    {
        tutorialText[2][0] = "Good job! Now it's time to\nget moving. Literally.";
        tutorialText[2][1] = "Each Unit has its own\nspeed which determines\nhow many hexagons it can" +
            "\ntraverse. Infantry has 3\nwhereas Cavalry has 5.";
        tutorialText[2][2] = "Cannons are unique.\nFirst off, notice how during\ndeployment we had you" +
            "\nplace Units around the\ncannon?";
        tutorialText[2][3] = "That's because cannons\ncannot move or fire\nunless they have least 3" +
            "\nInfantry or Cavalry in\nadjacent spaces.";
        tutorialText[2][4] = "Its move speed depends\non its surrounding Units.\nWhere less than 3 cavalry" +
        "\ngive it a move of 3\n and more than that";
        tutorialText[2][5] = "Will grant that cannon a move\n speed of \n5.";
        tutorialText[2][6] = "One thing to note is that\nthere is unique terrain that\ncan limit move speed.";
        tutorialText[2][7] = "Water and mud limit your\nmovement by 1 when\nmoving through them." +
        "\nSo a space 3 spots away\nwould take 4 to get to.";
        tutorialText[2][8] = "There is also impassable\nterrain. These are any\nspaces with rubble or\nother objects on them.";
        tutorialText[2][9] = "Moving to higher ground\nreduces your movement by\n1. Moving back down from" +
            "\nan elevated area does not\naffect your movement.";
        tutorialText[2][10] = "While this makes climbing\ndifficult, the higher \nelevation is useful for" + 
            "\ncannons if you manage to\nget them there.";
        tutorialText[2][11] = "And now it's your turn. To\nmove a single Unit, simply\ntouch the unit you" +
            " want to\nmove then touch a\nhighlighted space.";
        tutorialText[2][12] = "To move a group of units,\ntouch and hold until a\nblack hexagon appears" +
            "\nthen touch a highlighted\nspace.";
        tutorialText[2][13] = "The longer you hold, the\nlarger the hex will grow and\nthe more units" +
            " you can\nmove at once.";
        tutorialText[2][14] = "(Note: if some of the units\nchosen via group move\nare not able to" +
            " reach their\ndestination...";
        tutorialText[2][15] = "those Units will be left\nbehind and you will have\nto move them seperately)";
        tutorialText[2][16] = "Try to move your cannons\nso that they are in range\nof the enemy" +
            " (roughly 8\nspaces).";
        tutorialText[2][17] = "Remember that you want\nyour cannons to have 3\ninfantry or Cavalry nearby\nat all times.";
        tutorialText[2][18] = "When you are done, press\nthe giant picture on the left\njust like last time.";
    }

    static void SetCannonText()
    {
        tutorialText[3][0] = "Another phase down.\nYou're on a roll. Ready to\nshoot stuff?!";
        tutorialText[3][1] = "The Cannon Phase is what\nsets Little Wars apart from\nother strategy games.";
        tutorialText[3][2] = "Instead of an automatated\nor chance based system," +
            "\nyou get to aim and shoot\nthe cannon yourself!";
        tutorialText[3][3] = "To fire a cannon, first click\non a highlighted cannon.\nThe camera will zoom to" +
            "\nthat cannon to give you a\nbetter field of view.";
        tutorialText[3][4] = "From there, use the arrows\non the right to adjust the\npower of your shot." +
            "\nUse the arrows on the left\nto adjust your aim.";
        tutorialText[3][5] = "The button in the middle of\nthe aim changing arrows\nis for firing and the button" +
            "\nto the upper left is to exit\nyour cannon's view.";
        tutorialText[3][6] = "Each cannon can only fire\ntwice so be sure to\ncarefully line up your shot.";
        tutorialText[3][7] = "Also (and very importantly)\nonly Cannons that were\nnot moved during move\n" +
            "phase can be fired.";
        tutorialText[3][8] = "Before we send you off\nthere is one last thing you\nneed to know. There are 2" +
            "\nways to kill an enemy unit\nduring cannon combat.";
        tutorialText[3][9] = "The first way is a head on\nshot. The first thing hit by\na cannonball will ALWAYS" +
            "\ndie. Even if the cannonball\nwas shot with little power.";
        tutorialText[3][10] = "The second way is to\nknock over an enemy unit,\nwhether it's caused by the" +
            "\ncannonball or by knocking\none unit into another.";
        tutorialText[3][11] = "(Note: for the sake of the\ntutorial, cannons can be\nfired regardless of nearby" +
            "\nunits or if they were\nmoved.)";
        tutorialText[3][12] ="Now that you know what to\ndo, give it a shot. Fire your\ncannons at the enemy" +
            "\nUnits and try to kill as\nmany as you can.";
        tutorialText[3][13] = "When you're finished, end\nyour turn by touching the\ngiant...." +
            " well, I'm sure you\nknow how to end your turn\nat this point.";
    }

    static void SetFightText()
    {
        tutorialText[4][0] = "Wow, you're a pro at this!\nHave you considered" +
            "\nplaying this game\ncompetitively?";
        tutorialText[4][1] = "Lets get ready to rumble.\nThe final phase is called\nthe Fight Phase.";
        tutorialText[4][2] = "This phase is automatically\ntriggered by ending Move\nPhase while Infantry or" +
        "\nCavalry from different\nteams are touching.";
        tutorialText[4][3] = "When this phase is\ntriggered the units form\n\"combat lines\". Each Unit" +
            "\ntouching a Unit in combat\nis added to this line.";
        tutorialText[4][4] = "This includes units who\nare only in combat\nbecause they were added" +
            "\nto the combat line earlier.";
        tutorialText[4][5] = "In other words, if you form\na straight line of 5 units\nand the enemy is adjacent" +
            "\nto only one, all 5 of those\nUnits will be in combat.";
        tutorialText[4][6] = "After combat begins, the\nfight will play out and the\nside with the most Units" +
        "\nwill be the winner. All Units\non the losing side die.";
        tutorialText[4][7] = "Simple right? Give it a try.\nEnd your move phase with\none of your units \n(Infantry or Cavalry) \nadjacent to an enemy Unit.";
    }

    static void SetClosingText()
    {
        tutorialText[5][0] = "I give up. You're simply too\ngood at this game. There is\nlittle more I can teach" +
            "\nsomeone of such grand\nprowess.";
        tutorialText[5][1] = "As a result, it's time to\nwrap things up by\nexplaining 2 final parts of\nthe game.";
        tutorialText[5][2] = "Have you noticed those\nspheres on either side of\nt hemap? Those are\nView Markers.";
        tutorialText[5][3] = "You can click on them at\nany time during any phase\nto get a different view of\nthe map.";
        tutorialText[5][4] = "Lastly, you might have\nbeen wondering, \"When\ndoes the game end?\"" +
            " The\ngame ends when one of\ntwo conditions are met.";
        tutorialText[5][5] = "1) A unit has crossed over\nthe opposing player's\nbackline. This will give the" +
            "\nplayer who breached\n100 points before ending.";
        tutorialText[5][6] = "2) If a player has less than\n3 units at the start of their\nturn (Move Phase) the" +
            "\ngame automatically ends.";
        tutorialText[5][7] = "After the game ends, the\nplayer with the most points\nis declared the winner.";
        tutorialText[5][8] = "And that's everything you\nneed to know about how to\nplay Little Wars. Get out" +
        "\nthere and have fun!";
    }

    public static string[][] TutorialText
    {
        get { return tutorialText; }
    }
}
