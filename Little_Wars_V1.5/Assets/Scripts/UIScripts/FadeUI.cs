using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeUI : MonoBehaviour {
    public enum fadeType
    {
        regularFade,
        phaseFade
    };
    public fadeType currType = fadeType.regularFade;
    CanvasGroup canvasGroup; // The Component of the panel that holds its alpha value and interactibility
	// Use this for initialization
	void Start ()
    {
        canvasGroup = GetComponent<CanvasGroup>();
	}
	
	public void Fade () // Used when you want to fade-in a menu(Temporary Method)
    {
        StartCoroutine(DoFade());
    }

    public IEnumerator DoFade()
    {
        if(currType == fadeType.regularFade)
        {
            if(canvasGroup.alpha == 0)
            {
                while (canvasGroup.alpha < 1)
                {
                    canvasGroup.alpha += Time.deltaTime * 2;
                    yield return null;
                }
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                yield return null;
            }
            else if(canvasGroup.alpha == 1)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                while (canvasGroup.alpha > 0)
                {
                    canvasGroup.alpha -= Time.deltaTime * 2;
                    yield return null;
                }
                yield return null;
            }
        }
        else if(currType == fadeType.phaseFade)
        {
            canvasGroup.alpha = 0;
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime * 2;
                yield return null;
            }
            yield return new WaitForSeconds(0.25f);
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.deltaTime * 2;
                yield return null;
            }
        }
    }
}
