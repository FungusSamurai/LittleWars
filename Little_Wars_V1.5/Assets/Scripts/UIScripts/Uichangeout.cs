using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Uichangeout : MonoBehaviour
{

    public Button playr1image;
    public Button playr2image;
    
    
    
    public bool notfinishedwithmove = true;
    
    
    public bool movetobot;
    public bool movecanoncontrol;
    public bool rockoutwithyourstatsout;
    
    public bool deploydone= false;
    public bool cannontime;
    float bottomscreenuiplaces;
    float bottomscreenuidestinaions;
    float cannonbuttonplaces;
    float cannonbuttondestinations;
    float endphasebuttonplace;
    float endphasebuttongoal;
    float endphasebuttonhalf;
    RectTransform endphasetransform;

    //final attempt
    public GameObject deploybuttons;
    GameObject deployfinalize;
    GameObject endphasebutton;
    //public GameObject bottomscreenUI; //includes
    GameObject characterholder1;
    GameObject characterholder2;
    GameObject image1;
    GameObject image2;
    // end of bottom ui
    public GameObject statbox;

    //public GameObject[] cannonbuttons; includes
    GameObject firebutton;
    GameObject cannonleft;
    GameObject cannonright;
    GameObject cannonup;
    GameObject cannonsdown;
    GameObject powerup;
    GameObject powerdown;
    GameObject slider;
    GameObject cannonexit;
    // Use this for initialization
    public void deployfin()
    {
        deploydone = true;
    }
    void Start()
    {
        cannonexit = GameObject.Find("cannonexit");
        deployfinalize = GameObject.Find("Deployborder");
        endphasebutton = GameObject.Find("endphaseborder");
        characterholder1 = GameObject.Find("Characterholderborder");
        characterholder2 = GameObject.Find("Characterholder2border");
        image1 = GameObject.Find("Image1border");
        image2 = GameObject.Find("Image2border");
        firebutton = GameObject.Find("FireButtonBorder");
        cannonleft = GameObject.Find("cannonleft");
        cannonright = GameObject.Find("cannonright");
        cannonsdown = GameObject.Find("cannondown");
        cannonup = GameObject.Find("cannonup");
        powerdown = GameObject.Find("Powerdown");
        powerup = GameObject.Find("Powerup");
        slider = GameObject.Find("Slider");
    }

    // Update is called once per frame
    void Update()
    {
        if (movetobot == true)
        {
            characterholder1.transform.position = Vector2.Lerp(characterholder1.transform.position, new Vector2(characterholder1.transform.position.x, -100), Time.deltaTime * 1.5f);
            characterholder2.transform.position = Vector2.Lerp(characterholder2.transform.position, new Vector2(characterholder2.transform.position.x, -100), Time.deltaTime * 1.5f);
            image1.transform.position = Vector2.Lerp(image1.transform.position, new Vector2(image1.transform.position.x, -100), Time.deltaTime * 1.5f);
            image2.transform.position = Vector2.Lerp(image2.transform.position, new Vector2(image2.transform.position.x, -100), Time.deltaTime * 1.5f);
        }
        if (movetobot == false)
        {
             characterholder1.transform.position = Vector2.Lerp(characterholder1.transform.position, new Vector2(characterholder1.transform.position.x,Screen.height/18),Time.deltaTime*1.5f);
            characterholder2.transform.position = Vector2.Lerp(characterholder2.transform.position, new Vector2(characterholder2.transform.position.x, Screen.height / 18), Time.deltaTime * 1.5f);
            image1.transform.position = Vector2.Lerp(image1.transform.position, new Vector2(image1.transform.position.x, Screen.height / 6), Time.deltaTime * 1.5f);
            image2.transform.position = Vector2.Lerp(image2.transform.position, new Vector2(image2.transform.position.x, Screen.height / 6), Time.deltaTime * 1.5f);
        }
        if (movecanoncontrol == false)
        {
            slider.transform.position = Vector2.Lerp(slider.transform.position, new Vector2(slider.transform.position.x, Screen.height/4.25f), Time.deltaTime * 1.5f);
            powerdown.transform.position = Vector2.Lerp(powerdown.transform.position, new Vector2(powerdown.transform.position.x, Screen.height/7), Time.deltaTime * 1.5f);
            powerup.transform.position = Vector2.Lerp(powerup.transform.position, new Vector2(powerup.transform.position.x, Screen.height/3), Time.deltaTime * 1.5f);
            firebutton.transform.position = Vector2.Lerp(firebutton.transform.position, new Vector2(firebutton.transform.position.x, Screen.height/4.25f), Time.deltaTime * 1.5f);
            cannonleft.transform.position = Vector2.Lerp(cannonleft.transform.position, new Vector2(cannonleft.transform.position.x, Screen.height/4.25f), Time.deltaTime * 1.5f);
            cannonsdown.transform.position = Vector2.Lerp(cannonsdown.transform.position, new Vector2(cannonsdown.transform.position.x, Screen.height/10), Time.deltaTime * 1.5f);
            cannonup.transform.position = Vector2.Lerp(cannonup.transform.position, new Vector2(cannonup.transform.position.x, Screen.height/2.65f), Time.deltaTime * 1.5f);
            cannonright.transform.position = Vector2.Lerp(cannonright.transform.position, new Vector2(cannonright.transform.position.x, Screen.height/4.25f), Time.deltaTime * 1.5f);
            cannonexit.transform.position = Vector2.Lerp(cannonexit.transform.position, new Vector2(cannonexit.transform.position.x, Screen.height / 2.65f), Time.deltaTime * 1.5f);
        }
        if (movecanoncontrol == true)
        {
            slider.transform.position = Vector2.Lerp(slider.transform.position, new Vector2(slider.transform.position.x,-100), Time.deltaTime * 1.5f);
            powerdown.transform.position = Vector2.Lerp(powerdown.transform.position, new Vector2(powerdown.transform.position.x, -100), Time.deltaTime * 1.5f);
            powerup.transform.position = Vector2.Lerp(powerup.transform.position, new Vector2(powerup.transform.position.x, -100), Time.deltaTime * 1.5f);
            firebutton.transform.position = Vector2.Lerp(firebutton.transform.position, new Vector2(firebutton.transform.position.x, -100), Time.deltaTime * 1.5f);
            cannonleft.transform.position = Vector2.Lerp(cannonleft.transform.position, new Vector2(cannonleft.transform.position.x, -100), Time.deltaTime * 1.5f);
            cannonsdown.transform.position = Vector2.Lerp(cannonsdown.transform.position, new Vector2(cannonsdown.transform.position.x, -100), Time.deltaTime * 1.5f);
            cannonup.transform.position = Vector2.Lerp(cannonup.transform.position, new Vector2(cannonup.transform.position.x, -100), Time.deltaTime * 1.5f);
            cannonright.transform.position = Vector2.Lerp(cannonright.transform.position, new Vector2(cannonright.transform.position.x, -100), Time.deltaTime * 1.5f);
            cannonexit.transform.position = Vector2.Lerp(cannonexit.transform.position, new Vector2(cannonexit.transform.position.x, -100), Time.deltaTime * 1.5f);
        }
        if (rockoutwithyourstatsout == false && notfinishedwithmove == false && deployfinalize.transform.position.y <= -30.5f)
        {
            endphasebutton.transform.position = Vector2.Lerp(endphasebutton.transform.position, new Vector2(endphasebutton.transform.position.x, Screen.height/4.2f),Time.deltaTime*1.5f);
            
        }
        if (rockoutwithyourstatsout == true && cannontime == false)
        {
            
            endphasebutton.transform.position = Vector2.Lerp(endphasebutton.transform.position, new Vector2(endphasebutton.transform.position.x, Screen.height/4.2f), Time.deltaTime * 1.5f);
        }
        if (deploydone == true)
        {
            deploybuttons.transform.position = Vector2.Lerp(deploybuttons.transform.position, new Vector2(deploybuttons.transform.position.x, -100), Time.deltaTime * 1.5f);
            deployfinalize.transform.position = Vector2.Lerp(deployfinalize.transform.position, new Vector2(deployfinalize.transform.position.x,-50), Time.deltaTime * 2.5f);
            
        }
        if(cannontime == true)
        {
            endphasebutton.transform.position = Vector2.Lerp(endphasebutton.transform.position, new Vector2(endphasebutton.transform.position.x, -100), Time.deltaTime * 1.5f);
           
        }
    }
    /// <summary>
    /// highlights button
    /// </summary>
    /// <param name="playernumber"></param>
    public void buttonhighlight(int playernumber)
    {
        if(playernumber == 0)
        {
            playr1image.GetComponent<Outline>().enabled = true;
            playr2image.GetComponent<Outline>().enabled = false;
            

        }
        else if(playernumber == 1 )
        {
            playr1image.GetComponent<Outline>().enabled = false;
            playr2image.GetComponent<Outline>().enabled = true;
        }
    }
    public void statsout()
    {
        rockoutwithyourstatsout = false;
    }
    public void statsin()
    {
        rockoutwithyourstatsout = true;
    }
    public void Movebottomscreen()
    {

        movetobot = true;
        movecanoncontrol = false;
    }



    public void cannondown()
    {
        movetobot = false;
        movecanoncontrol = true;
    }
}
