using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

enum T_Phases { Introduction, Deploy, Move, Cannon, Fight, Closing } //Text about how to win and the view cubes are included in Closing

public class TutorialBrain : MonoBehaviour  {

    static TutorialUIMaster tm;
    static TutorialBrain instance;
    static TutorialAI t_ai;
    static int whichText, phaselength;
    static T_Phases phase;
    static bool acceptInput, ai_turn, image_on;
    //public List<HexCell> CV_list, CN_list, I_list;
    static Button turn_button;
    static GameObject d_markers;

	void Start()
    {
        GameBrain.ChangeAcceptInput(false);
        TopDownCamera.AllowMovement(false);
        instance = this;
        tm = new TutorialUIMaster();
        t_ai = new TutorialAI();
        acceptInput = false;
        ai_turn = false;
        image_on = false;
        turn_button = GameObject.Find("PlayerPanel").GetComponent<Button>();
        d_markers = GameObject.Find("DeployMarkers");
        d_markers.SetActive(false);

        UIMaster.SetActionPanel(false);

        phase = T_Phases.Introduction;
        whichText = 0;
        phaselength = TutorialUIMaster.TutorialText[(int)phase].Length;
        TutorialUIMaster.ChangeText((int)phase, whichText);

        GameObject.Find("Back Button").GetComponent<Button>().onClick.AddListener(BackText);
        GameObject.Find("Forward Button").GetComponent<Button>().onClick.AddListener(ForwardText);
        GameObject.Find("Continue Button").GetComponent<Button>().onClick.AddListener(PlayersTurn);
        GameObject.Find("Raise Button").GetComponent<Button>().onClick.AddListener(RaiseBox);
    }

    void Update()
    {
        Debug.Log(PlayerMaster.PriorTurn);

        if(acceptInput)
        {
            switch(phase)
            {
                case T_Phases.Deploy:
                    DeployMaster.SetTutorialTiles();
                    if(d_markers.transform.childCount == 0)
                        turn_button.interactable = true;
                    if(PlayerMaster.CurrentTurn == 1)
                    {
                        acceptInput = false;
                        ai_turn = true;
                        instance.StartCoroutine(TutorialAI.Deploy());
                    }
                    break;

                case T_Phases.Move:
                    MoveMaster.DecideMove();
                    if(PlayerMaster.CurrentTurn == 1)
                    {
                        acceptInput = false;
                        ai_turn = true;
                        instance.StartCoroutine(TutorialAI.Move());
                    }
                    break;

                case T_Phases.Cannon:
                    CannonMaster.HandleCannons();
                    if (PlayerMaster.CurrentTurn == 1)
                    {
                        acceptInput = false;
                        ai_turn = true;
                        instance.StartCoroutine(TutorialAI.IDontHaveCannons());
                    }
                    break;

                case T_Phases.Fight:
                    MoveMaster.DecideMove();
                    if(PlayerMaster.CurrentTurn == 1)
                    {
                        acceptInput = false;
                        ai_turn = true;
                        instance.StartCoroutine(TutorialAI.Move());
                    }
                    break;

                default:
                Debug.Log("Default");
                    break;
            }
        }

        if(ai_turn)
        {
            if(PlayerMaster.CurrentTurn == 0)
            {
                ai_turn = false;
                TutorialUIMaster.c_bool = false;
                TutorialUIMaster.SetContinue(false);
                NextPhase();
                instance.StartCoroutine(TutorialUIMaster.MoveTextbox());
            }
        }
    }

    static void BackText()
    {
        if (whichText > 0) //If we aren't at the first text
        {
            TutorialUIMaster.ChangeText((int)phase, --whichText); //Go to the text before it
            CheckForImage();
        }
    }

    static void ForwardText()
    {
        if (whichText + 1 < phaselength)
        {
            TutorialUIMaster.ChangeText((int)phase, ++whichText);
            if (whichText + 1 == phaselength)
                TutorialUIMaster.SetContinue(true);
            CheckForImage();
        }
    }

    static void PlayersTurn()
    {
        TutorialUIMaster.SetContinue(false);

        if ((int) phase > 0) //If we are past the introduction when this method is called
        {
            if ((int) phase > 4)
                SceneManager.LoadScene("TitleScreen");
            else
            {
                if (phase == T_Phases.Deploy)
                    d_markers.SetActive(true);
                else turn_button.interactable = true;


                acceptInput = true;
                instance.StartCoroutine(TutorialUIMaster.MoveTextbox());
            }

            CheckForImage();
        }
        else NextPhase(); //If we just finished the introduction (phase == 0), go to the next phase (Deploy).
    }

    static void NextPhase()
    {
        whichText = 0;
        ++phase;
        switch(phase)
        {
            case T_Phases.Deploy:
                UIMaster.DisplayState(GamePhase.Deploy, PlayerMaster.CurrentTurn);
                UIMaster.SetActionPanel(true);
                UIMaster.FadePhanel(0);
                break;

            case T_Phases.Cannon:
            for (int i = 0; i < PlayerMaster.CurrentPlayer.Can.Count; ++i)
                    {
                        PlayerMaster.CurrentPlayer.Can[i].Moved = false;
                    }
                    HighlightMaster.HighlightActiveCannons(PlayerMaster.CurrentPlayer);
                break;

            default:
                break;
        }

        turn_button.interactable = false;
        phaselength = TutorialUIMaster.TutorialText[(int)phase].Length;
        TutorialUIMaster.ChangeText((int)phase, whichText);
        
    }

    static void RaiseBox()
    {
        acceptInput = false;
        instance.StartCoroutine(TutorialUIMaster.MoveTextbox());
        CheckForImage();
    }

    static void CheckForImage()
    {
        if (!acceptInput)
        {
            if (phase == T_Phases.Cannon && (whichText == 4 || whichText == 5))
            {
                TutorialUIMaster.SetCannonImage(true);
                image_on = true;
            }

            else if(phase == T_Phases.Move)
            {
                if(whichText == 7 || whichText == 8)
                {
                    TutorialUIMaster.SetElevationImage(false);
                    TutorialUIMaster.SetTerrainImage(true);
                    image_on = true;
                }

                else if(whichText == 9 || whichText == 10)
                {
                    TutorialUIMaster.SetTerrainImage(false);
                    TutorialUIMaster.SetElevationImage(true);
                    image_on = true;
                }

                else
                {
                    TutorialUIMaster.SetTerrainImage(false);
                    TutorialUIMaster.SetElevationImage(false);
                    image_on = false;
                }
            }

            else
            {
                TutorialUIMaster.SetCannonImage(false);
                TutorialUIMaster.SetTerrainImage(false);
                TutorialUIMaster.SetTerrainImage(false);
                image_on = false;
            }
           
        }

        else
        {
            if (image_on)
            {
                TutorialUIMaster.SetCannonImage(false);
                TutorialUIMaster.SetTerrainImage(false);
                TutorialUIMaster.SetElevationImage(false);
                image_on = false;
            }
        }
    }
}
