using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sfxScript : MonoBehaviour
{
    bool disappear = false;
    Color color;
    float alpha = 1.0f;
    GameObject sfx_camera;
    Vector3 scale;
	// Use this for initialization
	void Start ()
    {
        color = gameObject.GetComponent<SpriteRenderer>().color;
        sfx_camera = GameObject.Find("MainCamera");
        scale = transform.localScale;
        transform.localScale = scale * .2f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.LookAt(sfx_camera.transform); //It always faces the MainCamera
        if (!disappear)
        {
            transform.localScale += scale * Time.deltaTime/.2f; //returning to full scale takes .2 seconds
            if (transform.localScale.x >= scale.x)
                disappear = true;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, alpha);
            alpha -= Time.deltaTime / 2; //reduces the transparancy. Dividing by 2 makes it take 2 seconds
            if (alpha <= 0)
                Destroy(gameObject);
        }
	}
}