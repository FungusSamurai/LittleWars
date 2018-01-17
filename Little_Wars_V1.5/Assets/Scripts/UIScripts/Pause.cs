using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour {
    public AudioSource soundeffects;
    public GameObject[] buttonstofreeze;
    public GameObject unpausebutton;
    public bool paused;
    public GameObject returntomenubutton;
    public GameObject unpausereal;
    public GameObject quitbutton;
    float alpha = 0;
    SpriteRenderer blackScreen;
	// Use this for initialization
	void Start () {
       // buttonstofreeze = GameObject.FindGameObjectsWithTag("uinew");
        //returntomenubutton = GameObject.Find("returntomenu");
       // quitbutton = GameObject.Find("quitgameb");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void quitgame()
    {
        Application.Quit();
    }
    public void replay()
    {
        FinalScoreManager.ResetScore();
        SceneManager.LoadScene("Map Workshop");
    }
    public void returntomenu()
    {
        Time.timeScale = 1.0f;
        FinalScoreManager.ResetScore();
        SceneManager.LoadScene("TitleScreen");
        
    }
    public void Pausebutton()
    {
        paused = true;
        soundeffects.GetComponent<Audiomanager>().soundeffectplay(0);
        foreach (GameObject i in buttonstofreeze)
        {
           
            i.GetComponent<Button>().interactable = false;
        }

        

        unpausebutton.SetActive(true);
        returntomenubutton.SetActive(true);
        unpausereal.SetActive(true);
        quitbutton.SetActive(true);
        Time.timeScale = 0;
    
    }
    public void unpause()
    {
        paused = false;
        soundeffects.GetComponent<Audiomanager>().soundeffectplay(0);
        Time.timeScale = 1;
        foreach(GameObject i in buttonstofreeze)
        {

            i.GetComponent<Button>().interactable = true;

        }

        unpausebutton.SetActive(false);
        returntomenubutton.SetActive(false);
        unpausereal.SetActive(false);
        quitbutton.SetActive(false);

    }

    public void GameOver()
    {
        paused = true;
        foreach (GameObject i in buttonstofreeze)
            i.GetComponent<Button>().interactable = true;

        //Jacob, place the code for pulling the UI back here

        GameObject cam = GameObject.Find("MainCamera");
        cam.transform.GetChild(0).gameObject.SetActive(true); //There's a black screen attached to the MainCamera that is used for fading
        blackScreen = cam.transform.GetChild(0).GetComponent<SpriteRenderer>();
        blackScreen.color = new Color(0, 0, 0, 0);
        StartCoroutine("EndingGame");
    }

    
    IEnumerator EndingGame()
    {
        while (alpha < 1.0f)
        {
            alpha += .01f;
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        SceneManager.LoadScene("GameOver");
    }
}
