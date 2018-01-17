using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour {    
	
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(transform.forward * 5 * Time.deltaTime);
        if (transform.position.z >= 400)
            transform.position = new Vector3(transform.position.x, transform.position.y, -400);
	}
}
