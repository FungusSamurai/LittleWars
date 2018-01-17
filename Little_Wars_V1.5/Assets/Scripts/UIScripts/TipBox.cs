using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipBox : MonoBehaviour {
    public GameObject Tipbox;
    public Text text;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void goawaytipbox()
    {
        Tipbox.SetActive(false);
    }
    public void heresatip(string whatsitsay)
    {
        Tipbox.SetActive(true);
        text.text = whatsitsay;
    }
}
