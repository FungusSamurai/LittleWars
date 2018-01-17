using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audiomanager : MonoBehaviour {
    public AudioClip[] soundeffects;
    public AudioSource gameaudio;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    //takes an int and plays sound effect according to that int
    public void soundeffectplay(int soundeffectnumber)
    {
        gameaudio.PlayOneShot(soundeffects[soundeffectnumber]);
    }
}
