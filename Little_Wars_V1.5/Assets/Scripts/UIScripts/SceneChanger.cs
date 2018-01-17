using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    //Changes Scene
    public void ChangeScene(string targetScene)
    {
        if (targetScene == "A Little Tutorial")
        {
            GameBrain.mapName = "LW_Tutorial";
            GameBrain.tutorial = true;
        }

        //ChangeLevel();
        SceneManager.LoadScene(targetScene);
    }

    IEnumerator ChangeLevel()
    {
        float fadeTime = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Fading>().BeginFade(1);
        //SoundMaster.FadeMusic(fadeTime);
        yield return new WaitForSeconds(fadeTime);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void DisableBoolAnimator(Animator anim)
    {
        anim.SetBool("IsDisplayed", false);
    }
    public void EnableBoolAnimator(Animator anim)
    {
        anim.SetBool("IsDisplayed", true);
    }

}
