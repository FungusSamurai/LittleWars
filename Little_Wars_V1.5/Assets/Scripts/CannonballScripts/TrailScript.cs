using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailScript : MonoBehaviour {

    float wait = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        wait += Time.deltaTime;
	}

    void OnTriggerEnter(Collider col) //if the cannonball collides with anything it stops it's position.
    {
        //The wait is because the cannonball is able to collide with the collider above the cannon (the one we use to click on the cannon)
        //This would cause the ghostcannonball to stop too earlier and the rest of the line wouldn't draw
        if (col.gameObject.name == "Plane" || wait > 1f) 
        {
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
