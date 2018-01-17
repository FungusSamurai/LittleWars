using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public enum GamePhase { Deploy, Cannon, Move, Attack } // enum of the current phase

/// <summary>
/// Controls the Game State and "uses" the master scripts to adjucate user input and result.
/// </summary>
public class GameBrain : MonoBehaviour
{
    static GameBrain instance; // used so we can still attach the script to an object and make use of instance only monobehavior functions

   // static bool canSkip;

    static UIMaster uiM; // Controls AI functions
    static ScoreMaster sM; // Handles Scores
    static MapMaster mM; // Contains all methods related to map data and provides map array
    static PlayerMaster pM; // Holds player profiles and information
    static DeployMaster dM; // Handles the functions to the deploy phase
    static MoveMaster mvM;// Handles the functions for the move phase
    static AttackMaster aM; // Handles the functions for the attack phase

    public static GamePhase gP; // current phase
    public static string mapName = "LW_Tutorial"; // default map loaded
    private static string mapPath; // file path for the maps, currently tied to resources folders and text assets
    private int phase; 
    MapLoader mL; // hanbdles the IO of loading a map.

    public bool VS_AI;
    public static bool tutorial = true;
    static bool acceptInput;// if false, nothing will happen when the user clicks a button or presses a key
    public static int turnNum;

    //initialize
	void Start ()
    {
        instance = this;
       // canSkip = false;
        uiM = new UIMaster();
        sM = new ScoreMaster();
        mM = new MapMaster();
        pM = new PlayerMaster();
        dM = new DeployMaster();
        mvM = new MoveMaster();
        aM = new AttackMaster();

        acceptInput = true;
        turnNum = 0;

        CannonMaster.ResetCurrent();

        gP = GamePhase.Deploy;
        ScoreMaster.ResetScore();

        mL = new MapLoader();
        mL.LoadMapFromTextAsset(((TextAsset)Resources.Load("Maps/" + mapName)));
        PlayerMaster.SetBackLines(MapMaster.Map);
        TopDownCamera.ActivateMainCamera(MapMaster.Height, MapMaster.Width, MapMaster.MapRadius);

        UIMaster.SetGeneralUI();
        UIMaster.SetDeployUI();
        UIMaster.SetCannonUI();
        UIMaster.DisplayScore();

        UIMaster.SetPanelAlpha(false, (int)UIPannels.Cannon);
        UIMaster.SetPanelAlpha(false, (int)UIPannels.Phase);
        UIMaster.SetPanelAlpha(false, (int)UIPannels.View);
        UIMaster.SetPanelAlpha(false, (int)UIPannels.Fight);

        UIMaster.SetActionPanel(true);

        if (!tutorial)
            UIMaster.DisplayState(gP, PlayerMaster.CurrentTurn);

       // norton = VS_AI ? new AIBrain() : null; // must be called after UIMaster.SetGeneralUI to percive turns properly
        
    }
	
	// Update Game Loop
	void Update ()
    {
        if (acceptInput)
        {
            ViewMaster.CheckClick();
            ManageTurn();
        }
	}

    // Drives the current behavior based on what phase of the game is taking place.
    void ManageTurn()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        switch (gP)
        {
            case (GamePhase.Deploy):
                DeployMaster.SetTiles(); // let users set units
                break;
            case (GamePhase.Cannon):
                CannonMaster.HandleCannons(); // let users turn and fire cannons
                break;
            case (GamePhase.Move):
                MoveMaster.DecideMove(); // let users move available units
                break;
            case (GamePhase.Attack): // Attack phase currently resolves itself with no need for user input 
                break;
            default:
                break;
        }
    }

    //starts a coroutine to change the current turn. 
    //This lets us delay specific actions or code calls based on phase or desired "wait" while the rest of the game runs.
    public static void ChangeTurnParallel()
    {
        instance.StartCoroutine(ChangeTurn());
    }

    // Changes the phase and updates the UI as needed
    public static IEnumerator ChangeTurn()
    {
        UIMaster.SetActionPanel(false);

        yield return AlterState(); // pauses before executing the rest of the code based on phase.

        if (PlayerMaster.PriorTurn == 0)
        {
            yield return EndofRound();
        }

        if (gP != GamePhase.Attack)
        {
            PlayerMaster.SwitchPlayer();
            TopDownCamera.ChangePlayerCamera();
        }

        UIMaster.ChangeDisplayedPlayer();
        UIMaster.ChangePlayerDeploy(gP);
        UIMaster.DisplayState(gP, PlayerMaster.CurrentTurn);

        turnNum++;

        yield return null;
    }

    //Alters aspects of the game state (such as highlights or unit numbers) based on phase and other information
    private static IEnumerator AlterState()
    {
        switch (gP)
        {
            case (GamePhase.Deploy):

                if (DeployMaster.instance.uoList.Count != 0)
                {
                    yield return DeployMaster.DeployUnitsCoroutine();
                    yield return new WaitForSeconds(1.5f);
                }
                UIMaster.UpdateDeployAmount();
                break;
            case (GamePhase.Cannon):
                // CheckSkip();

          /*      if (PlayerMaster.CurrentTurn == 0)
                {
                    CheckSkip();
                } */

                CannonMaster.ExitCannon();
                HighlightMaster.UnhighlightCannons(PlayerMaster.CurrentPlayer);
                DeactivateCannonByCannon(PlayerMaster.OtherPlayer);

                if (PlayerMaster.CurrentTurn == 0)
                {
                    //DeactivateCannonByMove(PlayerMaster.OtherPlayer);
                    HighlightMaster.HighlightActiveCannons(PlayerMaster.OtherPlayer);
                }
                break;
            case (GamePhase.Move):

                if (MoveMaster.UnitsToMove != 0)
                {
                    HighlightMaster.UnhighlightMovable(PlayerMaster.CurrentPlayer);
                    MoveMaster.ClearMoveVariables();
                }
                AttackMaster.BuildCombatAreas();

                ActivateCannonByMove(PlayerMaster.CurrentPlayer);
                DeactivateCannonByMove(PlayerMaster.CurrentPlayer);

                if (AttackMaster.CombatCount == 0)
                {
                    CannonMaster.CheckCannonCapture();
                    yield return WinByMove();
                    if (PlayerMaster.CurrentTurn == 0)
                    {
                        MoveMaster.PrimeMoveVariables(PlayerMaster.OtherPlayer);
                        HighlightMaster.HighlightMovable(PlayerMaster.OtherPlayer);
                    }
                }

                else if (AttackMaster.CombatCount != 0)
                {
                  //  AttackMaster.BuildCombatAreas();
                    //gP = GamePhase.Attack;

                    UIMaster.SetActionPanel(false); //Deactivates off action panel
                    UIMaster.FadePhanel((int)UIPannels.Action);
                    acceptInput = false;

                    instance.StartCoroutine(Fight());
                    //yield return Fight();
                }
               break;
            case (GamePhase.Attack):
                break;
            default:
                yield return null;
                break;
        }
    }

    // changes the phase and "refreshes" variables as apropriate
    static IEnumerator EndofRound()
    {
        switch (gP)
        {
            case (GamePhase.Deploy):
                Debug.Log("Move from D");
                MoveMaster.PrimeMoveVariables(PlayerMaster.OtherPlayer);
                PlayerMaster.RefreshMovement();
                HighlightMaster.HighlightMovable(PlayerMaster.OtherPlayer);
                gP = GamePhase.Move;

                break;
            case (GamePhase.Cannon):
                Debug.Log("Move from C");
                PlayerMaster.RefreshMovement();
                if (PlayerMaster.CurrentTurn == 1)
                {
                    MoveMaster.PrimeMoveVariables(PlayerMaster.OtherPlayer);
                    HighlightMaster.HighlightMovable(PlayerMaster.OtherPlayer);
                }
                gP = GamePhase.Move;
                break;
            case (GamePhase.Move):
                Debug.Log("Cannon from M");
                HighlightMaster.HighlightActiveCannons(PlayerMaster.OtherPlayer);

                yield return WinBySize();
                gP = GamePhase.Cannon;
                //CheckSkip();
                break;
            case (GamePhase.Attack):
                Debug.Log("Move from A");
                break;
            default:
                yield return null;
                break;
        }
    }

 /*   static void CheckSkip()
    {
        foreach (Cannon c in PlayerMaster.OtherPlayer.Can)
        {
            if (c.Moved)
            {
                return;
            }
        }
        canSkip = true;

    } */

    // disables cannon usability if a unit moves away from or to it
    static void DeactivateCannonByMove(Player p)
    {
        foreach (Cannon c in p.Can)
        {
            if (!c.Moved)
            {
                CannonMaster.SetCannon(c);
            }
        }
    }

    // disables cannon usability if another cannon kills the controling units
    static void DeactivateCannonByCannon(Player p)
    {
        foreach (Cannon c in p.Can)
        {
            if (c.Manned && !c.Moved)
            {
                CannonMaster.SetCannon(c);
            }
        }
    }

    // enable cannon is a legal conditions met when a unit moves near it
    static void ActivateCannonByMove(Player p)
    {
        foreach (Cannon c in p.Can)
        {
            if (!c.manned)
            {
                CannonMaster.SetCannon(c);
            }
        }
    }

    static IEnumerator Fight() //Handles and manages the FightScene
    {
        gP = GamePhase.Attack;

        while (AttackMaster.CombatCount != 0)
        {
            float rotation;
            if (PlayerMaster.CurrentTurn == 0)
                rotation = 180;
            else rotation = 0;
            
            instance.StartCoroutine(TopDownCamera.LerpToPosition(AttackMaster.CenterOfCombat(), Quaternion.Euler(90, rotation, 0)));
            yield return new WaitForSeconds(2.0f); //Wait to give the player a chance to see stuff

            instance.StartCoroutine(UIMaster.MoveCurtains(true)); //lower curtains
            yield return null; //Wait at least one frame so that UIMaster.MovingCurtians can change value
            while (UIMaster.MovingCurtains)
                yield return null;

            List<Unit> currentCombat = AttackMaster.CurrentCombatArea;
            List<Unit> toKill = AttackMaster.FightResults();
            FightSceneMaster.SetUpFight(currentCombat, toKill); //Prepare the fightscene

            yield return new WaitForSeconds(1.0f); //Pause for dramatic effect
            instance.StartCoroutine(UIMaster.MoveCurtains(false)); //Raise Curtains
            yield return null;
            while (UIMaster.MovingCurtains)
                yield return null;

            SoundMaster.AttackCharge();
            instance.StartCoroutine(FightSceneMaster.ToBattle()); //Begins FightScene
            yield return null; //Wait at least one frame so that FightSceneMaster.Fighting can change value
            while (FightSceneMaster.Fighting)
                yield return null;

            //SoundMaster.AttackResolution();
            instance.StartCoroutine(UIMaster.MoveCurtains(true)); //Lower curtains again
            yield return null; //Wait at least one frame so that UIMaster.MovingCurtians can change value
            while (UIMaster.MovingCurtains)
                yield return null;

            FightSceneMaster.CleanUp(); //Resets FightScene

            yield return new WaitForSeconds(0.25f); //Pause

            instance.StartCoroutine(UIMaster.MoveCurtains(false)); //Raise curtains
            yield return new WaitForFixedUpdate(); //Wait at least one frame so that UIMaster.MovingCurtians can change value
            while (UIMaster.MovingCurtains)
                yield return null;

            yield return new WaitForSeconds(1.0f);
            foreach (Unit u in toKill)
                PlayerMaster.KillUnit(u); //Kills dead units

            yield return new WaitForSeconds(1.0f); //Pause to see aftermath
            
        }

        //return to player's position after the fight
        instance.StartCoroutine(TopDownCamera.LerpToPosition(PlayerMaster.CurrentPlayer.CameraPosistion, PlayerMaster.CurrentPlayer.CameraRotation));
        
        DeactivateCannonByMove(PlayerMaster.OtherPlayer);

        if (PlayerMaster.CurrentTurn == 0)
        {
            HighlightMaster.HighlightMovable(PlayerMaster.OtherPlayer);
        }

        UIMaster.DisplayScore();

       // WinByMove();

        UIMaster.SetActionPanel(true); //Turns action pannel back on
        UIMaster.FadePhanel((int)UIPannels.Action);

        CannonMaster.CheckCannonCapture();

        gP = GamePhase.Move;
        yield return ChangeTurn();
        acceptInput = true;
    }

    public static IEnumerator GameOver() // disables input and calles method that fades to black and changes scenes
    {
        acceptInput = false;
        UIMaster.SetPanelAlpha(false, (int)UIPannels.Action);
        UIMaster.SetPanelAlpha(false, (int)UIPannels.Action);
        GameObject gg = GameObject.Find("PhasePanel");
        if (gg != null)
        {
            gg.GetComponent<CanvasGroup>().alpha = 0;
        }

        //GameObject.Find("New Canvas").GetComponent<Canvas>().enabled = false;
        GameObject cam = TopDownCamera.Instance.gameObject;
        cam.transform.GetChild(0).gameObject.SetActive(true); //There's a black screen attached to the MainCamera that is used for fading
        SpriteRenderer blackScreen = cam.transform.GetChild(0).GetComponent<SpriteRenderer>();
        blackScreen.color = new Color(0, 0, 0, 0);
        instance.StartCoroutine(EndingGame(blackScreen));

        yield return null;
    }


    static IEnumerator EndingGame(SpriteRenderer blackScreen)
    {
        float alpha = 0;
        while (alpha < 1.0f)
        {
            alpha += .01f;
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        FinalScoreManager.Score1 = ScoreMaster.Score1;
        FinalScoreManager.Score2 = ScoreMaster.Score2;
        SceneManager.LoadScene("Gameover");
    }

    // checks for win condition by movement rules
    static IEnumerator WinByMove()
    {
        bool end = false;
        foreach (Unit u in PlayerMaster.CurrentPlayer.Cav)
        {
            if (end)
            {
                break;
            }
            end = BehindBackline(u);
        }
        foreach (Unit u in PlayerMaster.CurrentPlayer.Inf)
        {
            if (end)
            {
                break;
            }
            end = BehindBackline(u);
        }

        if (end)
        {
            yield return GameOver();
        }
        else
        {
            yield return null;
        }

    }

    // checks for win condition by placement rules
    static bool BehindBackline(Unit u)
    {
        int value = PlayerMaster.CurrentTurn == 0 ? (u.CurrentHex.R - PlayerMaster.CurrentPlayer.winZone) : PlayerMaster.CurrentPlayer.winZone - u.CurrentHex.R;

        if (value >= 0)
        {
            ScoreMaster.GivePoints(ScoreMaster.BreachBonus, u.Player);
            return true;
        }
        else
        {
            return false;
        }
    }

    // checks for win conditon by population rules
    static IEnumerator WinBySize()
    {
        if (TooSmall(PlayerMaster.CurrentPlayer) || TooSmall(PlayerMaster.OtherPlayer))
        {
            yield return GameOver();
        }
        else
        {
            yield return null;
        }
    }

    static bool TooSmall(Player p)
    {
        int count = 0;

        foreach (Unit u in p.Cav)
        {
            count++;
        }
        foreach (Unit u in p.Inf)
        {
            count++;
        }

        if (count < 3)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    //toogles accept input variables
    public static void ChangeAcceptInput(bool accept)
    {
        acceptInput = accept;
    }
}
