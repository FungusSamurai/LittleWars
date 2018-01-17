using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

enum UIPannels {Action, Cannon, Phase, View, Fight}

public class UIMaster
{
    static UIMaster instance;

    Button playerHead;

    Button[] deployButtons;

    Button[] cannonArrows;
    Button exitCannon;
    Button exitView;
    Button fireButton;
    Slider powerSlider;

    Text[] scoreTexts;
    Text[] phaseDisplayTexts;
    
    FadeUI[] pannelFaders;

    RectTransform curtainsPos;
    float topCurtainPos;
    bool movingCurtains;

    public UIMaster()
    {
        playerHead = GameObject.Find("PlayerPanel").GetComponent<Button>();

        deployButtons = new Button[] { GameObject.Find("InfantryButton").GetComponent<Button>(), GameObject.Find("CavalryButton").GetComponent<Button>(), GameObject.Find("CannonButton").GetComponent<Button>(), GameObject.Find("RemoveButton").GetComponent<Button>() };

        cannonArrows = new Button[] { GameObject.Find("RotateUpButton").GetComponent<Button>(), GameObject.Find("RotateRightButton").GetComponent<Button>(), GameObject.Find("RotateDownButton").GetComponent<Button>(), GameObject.Find("RotateLeftButton").GetComponent<Button>(), GameObject.Find("PowerUpButton").GetComponent<Button>(), GameObject.Find("PowerDownButton").GetComponent<Button>() };
        exitCannon = GameObject.Find("ExitCannonButton").GetComponent<Button>();
        exitView = GameObject.Find("ExitViewButton").GetComponent<Button>();
        fireButton = GameObject.Find("FireButton").GetComponent<Button>();
        powerSlider = GameObject.Find("PowerSlider").GetComponent<Slider>();

        scoreTexts = new Text[] { GameObject.Find("Player1ScoreText").GetComponent<Text>(), GameObject.Find("Player2ScoreText").GetComponent<Text>() };
        phaseDisplayTexts = new Text[] { GameObject.Find("PhaseText").GetComponent<Text>(), GameObject.Find("PlayerText").GetComponent<Text>() };

        pannelFaders = new FadeUI[] { GameObject.Find("ActionPanel").GetComponent<FadeUI>(), GameObject.Find("CannonPanelv1").GetComponent<FadeUI>(), GameObject.Find("PhasePanel").GetComponent<FadeUI>(), GameObject.Find("ExitViewButton").GetComponent<FadeUI>(), GameObject.Find("FightSceneCanvas").GetComponent<FadeUI>()};
        curtainsPos = GameObject.Find("Curtains").GetComponent<RectTransform>();
        movingCurtains = false;
        instance = this;

        topCurtainPos = curtainsPos.anchoredPosition.y;
    }

    public static void SetActionPanel(bool on)
    {
        instance.pannelFaders[(int)UIPannels.Action].GetComponent<CanvasGroup>().interactable = on;
    }

    public static void SetGeneralUI()
    {
        instance.playerHead.onClick.AddListener(GameBrain.ChangeTurnParallel);
    }

    public static void SetAIListner(UnityEngine.Events.UnityAction call)
    {
        instance.playerHead.onClick.AddListener(call);
    }

    public static void SetDeployUI()
    {
        SetDeployButtons();
    }

    public static void SetCannonUI()
    {
        EventTrigger t = instance.cannonArrows[0].GetComponent<EventTrigger>();
        AddEventTriggerListener(t, EventTriggerType.PointerDown, delegate {CannonMaster.CurrentUp(); });
        AddEventTriggerListener(t, EventTriggerType.PointerUp, delegate { CannonMaster.CurrentUp(); });
        t = instance.cannonArrows[1].GetComponent<EventTrigger>();
        AddEventTriggerListener(t, EventTriggerType.PointerDown, delegate { CannonMaster.CurrentRight(); });
        AddEventTriggerListener(t, EventTriggerType.PointerUp, delegate { CannonMaster.CurrentRight(); });
        t = instance.cannonArrows[2].GetComponent<EventTrigger>();
        AddEventTriggerListener(t, EventTriggerType.PointerDown, delegate { CannonMaster.CurrentDown(); });
        AddEventTriggerListener(t, EventTriggerType.PointerUp, delegate { CannonMaster.CurrentDown(); });
        t = instance.cannonArrows[3].GetComponent<EventTrigger>();
        AddEventTriggerListener(t, EventTriggerType.PointerDown, delegate { CannonMaster.CurrentLeft(); });
        AddEventTriggerListener(t, EventTriggerType.PointerUp, delegate { CannonMaster.CurrentLeft(); });

        instance.cannonArrows[4].onClick.AddListener(CannonMaster.CurrentUpPower);
        instance.cannonArrows[5].onClick.AddListener(CannonMaster.CurrentDownPower);
        instance.fireButton.onClick.AddListener(CannonMaster.CurrentFire);
        instance.exitCannon.onClick.AddListener(CannonMaster.ExitCannon);
        instance.exitView.onClick.AddListener(ViewMaster.ExitView);
    }

    public static void SetViewPanel(bool on)
    {
        instance.pannelFaders[(int)UIPannels.View].GetComponent<CanvasGroup>().interactable = on;
    }

    public static void SetFightScenePanel (bool on)
    {
        instance.pannelFaders[(int)UIPannels.Fight].GetComponent<CanvasGroup>().interactable = on;
    }

    public static void TogglePanelLock(bool on, int panel)
    {
        GameObject g = instance.pannelFaders[panel].gameObject;
        foreach (Button b in g.GetComponentsInChildren<Button>())
        {
            if (b != null)
            {
                b.interactable = on;
            }
        }
        foreach (EventTrigger e in g.GetComponentsInChildren<EventTrigger>())
        {
            if (e != null)
            {
                e.enabled = on;
            }
        }
    }

    public static void SetPanelAlpha(bool on, int panel)
    {
        int alph = on ? 1 : 0;

        instance.pannelFaders[panel].gameObject.GetComponent<CanvasGroup>().alpha = alph; 
    }

    public static void FadePhanel(int panel)
    {
        FadeUI fU = instance.pannelFaders[panel];
        fU.Fade();
    }

    public static void DisplayScore()
    {
        instance.scoreTexts[0].text = ScoreMaster.Score1.ToString();
        instance.scoreTexts[1].text = ScoreMaster.Score2.ToString();
    }

    public static void DisplayState(GamePhase phase, int player)
    {
        instance.phaseDisplayTexts[0].text = phase.ToString() + " Phase";
        instance.phaseDisplayTexts[1].text = "Player " + (player + 1);

        instance.pannelFaders[(int)UIPannels.Phase].StartCoroutine(FadePhase());

        SoundMaster.TurnTransition();
    }

    private static IEnumerator FadePhase()
    {
        while (TopDownCamera.IsLearping)
        {
            yield return null;
        }

        FadeUI fU = instance.pannelFaders[(int)UIPannels.Phase];

        yield return fU.DoFade();

        SetActionPanel(true);
    }

    public static void AddEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, System.Action<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener(new UnityEngine.Events.UnityAction<BaseEventData>(callback));
        trigger.triggers.Add(entry);
    }

    public static void SetDeployButtons()
    {
        for (int i = 0; i < instance.deployButtons.Length - 1; i++)
        {
            int j = i;
            instance.deployButtons[i].onClick.AddListener(() => DeployMaster.SetToSpawnUnit(j));
        }
        instance.deployButtons[instance.deployButtons.Length - 1].onClick.AddListener(() => DeployMaster.SetToSpawnUnit(-1));
        UpdateDeployAmount();
    }

    public static void UpdateDeployAmount()
    {
        for(int i = 0; i < instance.deployButtons.Length - 1; i++)
        {
            instance.deployButtons[i].gameObject.GetComponentInChildren<Text>().text = DeployMaster.UnitsUsed(i).ToString();
        }
    }

    public static void SetPlayerUnitInfo()
    {
        instance.deployButtons[0].gameObject.GetComponentInChildren<Text>().text = PlayerMaster.CurrentPlayer.Inf.Count.ToString();
        instance.deployButtons[1].gameObject.GetComponentInChildren<Text>().text = PlayerMaster.CurrentPlayer.Cav.Count.ToString();
        instance.deployButtons[2].gameObject.GetComponentInChildren<Text>().text = PlayerMaster.CurrentPlayer.Can.Count.ToString();
    }

    public static void ChangePlayerDeploy(GamePhase gP)
    {
        SpriteState tempState = new SpriteState();
        instance.deployButtons[0].image.sprite = Resources.Load<Sprite>("Pictures/DeployInf" + (PlayerMaster.CurrentTurn) + "Button");
        tempState.highlightedSprite = Resources.Load<Sprite>("Pictures/DeployInf" + (PlayerMaster.CurrentTurn) + "Button");
        tempState.pressedSprite = Resources.Load<Sprite>("Pictures/DeployInf" + (PlayerMaster.CurrentTurn) + "ButtonPressed");
        tempState.disabledSprite = Resources.Load<Sprite>("Pictures/DeployInf" + (PlayerMaster.CurrentTurn) + "ButtonDisabled");
        instance.deployButtons[0].spriteState = tempState;
        instance.deployButtons[1].image.sprite = Resources.Load<Sprite>("Pictures/DeployCav" + (PlayerMaster.CurrentTurn) + "Button");
        tempState.highlightedSprite = Resources.Load<Sprite>("Pictures/DeployCav" + (PlayerMaster.CurrentTurn) + "Button");
        tempState.pressedSprite = Resources.Load<Sprite>("Pictures/DeployCav" + (PlayerMaster.CurrentTurn) + "ButtonPressed");
        tempState.disabledSprite = Resources.Load<Sprite>("Pictures/DeployCav" + (PlayerMaster.CurrentTurn) + "ButtonDisabled");
        instance.deployButtons[1].spriteState = tempState;
        instance.deployButtons[2].image.sprite = Resources.Load<Sprite>("Pictures/DeployCannon" + (PlayerMaster.CurrentTurn) + "Button");
        tempState.highlightedSprite = Resources.Load<Sprite>("Pictures/DeployCannon" + (PlayerMaster.CurrentTurn) + "Button");
        tempState.pressedSprite = Resources.Load<Sprite>("Pictures/DeployCannon" + (PlayerMaster.CurrentTurn) + "ButtonPressed");
        tempState.disabledSprite = Resources.Load<Sprite>("Pictures/DeployCannon" + (PlayerMaster.CurrentTurn) + "ButtonDisabled");
        instance.deployButtons[2].spriteState = tempState;

        if (gP != GamePhase.Deploy)
        {
            SetPlayerUnitInfo();
        }
    }

    public static void ChangeDisplayedPlayer()
    {
        SpriteState tempState = new SpriteState();
        instance.playerHead.gameObject.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Pictures/DeployInf" + (PlayerMaster.CurrentTurn) + "Button");
        tempState.highlightedSprite = Resources.Load<Sprite>("Pictures/DeployInf" + (PlayerMaster.CurrentTurn) + "Button");
        tempState.pressedSprite = Resources.Load<Sprite>("Pictures/DeployInf" + (PlayerMaster.CurrentTurn) + "ButtonPressed");
        tempState.disabledSprite = Resources.Load<Sprite>("Pictures/DeployInf" + (PlayerMaster.CurrentTurn) + "ButtonDisabled");
        instance.playerHead.spriteState = tempState;
    }

    public static IEnumerator MoveCurtains(bool lower)
    {
        instance.movingCurtains = true;
        int increment = 0;
        Vector2 startPos = instance.curtainsPos.anchoredPosition;

        if (lower) //lower curtains
        {
            SoundMaster.CurtainDown();
            while (increment < 101)
            {
                instance.curtainsPos.anchoredPosition = Vector2.Lerp(startPos, new Vector2(0, 0), 0.01f * increment);
                increment += 1;
                yield return null;
            }
        }

        else //raise curtains
        {
            SoundMaster.CurtainUp();
            while (increment < 101)
            {
                instance.curtainsPos.anchoredPosition = Vector2.Lerp(startPos, new Vector2(0, instance.topCurtainPos), 0.01f * increment);
                increment += 1;
                yield return null;
            }
        }

        instance.movingCurtains = false;
    }

    public static bool MovingCurtains
    {
        get { return instance.movingCurtains; }
        set { instance.movingCurtains = value; }
    }
    public static Slider PowerSlider
    {
        get { return instance.powerSlider; }
        set { instance.powerSlider = value; }
    }
    public static Button PlayerHead
    {
        get { return instance.playerHead; }
    }

}
