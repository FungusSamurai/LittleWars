using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundMaster : MonoBehaviour {

    List<string> SFX_Names = new List<string>
    {
        //General SFX
        "Phase_Trans",
        "Button_Press",
        "Deploy",
        "Select_Infantry",
        "Select_Cavalry",
        "Select_Cannon",
        "Unit_Death",
        "Rotate_Cannon",
        "Fire_Cannon",
        "Cannon_Power",
        "Cannon_Ball_Hit",
        "Cannon_Ball_Pop",
        "Unit_Move",
        "Place_Unit",
        "End_Turn_Button",
        "Turn_Transition",
        "Turn_Cannon",
        //Fight Scene SFX
        "Attack_Charge",
        "Attack_Fighting",
        "Curtain_Up",
        "Curtain_Down",
        "Attack_Resolution"
    };

    public enum SFXEnum
    {
        //General SFX
        Phase_Trans,
        Button_Press,
        Deploy,
        Select_Infantry,
        Select_Cavalry,
        Select_Cannon,
        Unit_Death,
        Rotate_Cannon,
        Fire_Cannon,
        Cannon_Power,
        Cannon_Ball_Hit,
        Cannon_Ball_Pop,
        Unit_Move,
        Place_Unit,
        End_Turn_Button,
        Turn_Transition,
        Turn_Cannon,
        //Fight Scene SFX
        Attack_Charge,
        Attack_Fighting,
        Curtain_Up,
        Curtain_Down,
        Attack_Resolution
    }

    List<string> Music_Names = new List<string> { "Title_Music", "Map_Music", "GameOver_Music" };
    public enum MusicEnum { Title_Music, Map_Music, GameOver_Music }

    static List<AudioClip> SFX_List = new List<AudioClip>();

    static List<AudioClip> Music_List = new List<AudioClip>();

    GameObject AudioSourceGet;
    static AudioSource BGM;
    static AudioSource SFX;
    static AudioSource SFX2;

    void Awake()
    {
        AudioClip clip;

        for (int i = 0; i < SFX_Names.Count; i++)
        {
            clip = (AudioClip)Resources.Load("SFX/" + SFX_Names[i]);
            SFX_List.Add(clip);
        }

        for (int i = 0; i < Music_Names.Count; i++)
        {
            clip = (AudioClip)Resources.Load("Music/" + Music_Names[i]);
            Music_List.Add(clip);
        }

        AudioSourceGet = GameObject.Find("BGM");
        BGM = AudioSourceGet.GetComponent<AudioSource>();
        AudioSourceGet = GameObject.Find("SFX");
        SFX = AudioSourceGet.GetComponent<AudioSource>();
        AudioSourceGet = GameObject.Find("SFX2");
        SFX2 = AudioSourceGet.GetComponent<AudioSource>();

        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "TitleScreen")
        {
            BGM.clip = Music_List[(int)MusicEnum.Title_Music];
            BGM.Play();
        }
        else if (currentScene.name == "A Little War Converted" || currentScene.name == "A Little Tutorial")
        {
            BGM.clip = Music_List[(int)MusicEnum.Map_Music];
            BGM.volume = 0.15f;
            BGM.Play();
        }
        else if (currentScene.name == "Gameover")
        {
            BGM.clip = Music_List[(int)MusicEnum.GameOver_Music];
            BGM.Play();
        }
    }

    public void ButtonPress()
    {
        SFX.clip = SFX_List[(int)SFXEnum.Button_Press];
        SFX.Play();
    }

    public static void Deploy()
    {
        SFX.clip = SFX_List[(int)SFXEnum.Deploy];
        SFX.Play();
    }

    public static void SelectUnit()
    {
        SFX.clip = SFX_List[(int)SFXEnum.Select_Cannon];
        SFX.Play();
    }

    public void UnitDeath()
    {
        SFX.clip = SFX_List[(int)SFXEnum.Unit_Death];
        SFX.Play();
    }

    public static void FireCannon()
    {
        SFX.clip = SFX_List[(int)SFXEnum.Fire_Cannon];
        SFX.Play();
    }

    public void CannonPower()
    {
        SFX.clip = SFX_List[(int)SFXEnum.Cannon_Power];
        SFX.Play();
    }

    public static void CannonballHit()
    {
        SFX.clip = SFX_List[(int)SFXEnum.Cannon_Ball_Hit];
        SFX.Play();
    }

    public static void CannonballPop()
    {
        SFX.clip = SFX_List[(int)SFXEnum.Cannon_Ball_Pop];
        SFX.Play();
    }

    public static void AttackCharge()
    {
        SFX.clip = SFX_List[(int)SFXEnum.Attack_Charge];
        SFX.Play();
    }

    public static void AttackFighting()
    {
        SFX.clip = SFX_List[(int)SFXEnum.Attack_Fighting];
        SFX.Play();
    }

    public static void CurtainUp()
    {
        SFX.clip = SFX_List[(int)SFXEnum.Curtain_Up];
        SFX.Play();
    }

    public static void CurtainDown()
    {
        SFX.clip = SFX_List[(int)SFXEnum.Curtain_Down];
        SFX.Play();
    }

    public static void AttackResolution()
    {
        SFX2.clip = SFX_List[(int)SFXEnum.Attack_Resolution];
        SFX2.Play();
    }

    public void EndTurnButtonPress()
    {
        SFX2.clip = SFX_List[(int)SFXEnum.End_Turn_Button];
        SFX2.Play();
    }

    public static void TurnTransition()
    {
        SFX.clip = SFX_List[(int)SFXEnum.Turn_Transition];
        SFX.Play();
    }

    public static void TurnCannonOn()
    {
        SFX2.clip = SFX_List[(int)SFXEnum.Turn_Cannon];
        SFX2.loop = true;
        SFX2.Play();
    }

    public static void TurnCannonOff()
    {
        SFX2.Stop();
        SFX2.loop = false;
    }

    public static void StartUnitMove()
    {
        SFX2.clip = SFX_List[(int)SFXEnum.Unit_Move];
        SFX2.loop = true;
        SFX2.Play();
    }

    public static void StopUnitMove()
    {
        SFX2.Stop();
        SFX2.loop = false;
    }

    public static void FadeMusic(float t)
    {
        BGM.volume = Mathf.Lerp(-1, 1, t);
    }
}
