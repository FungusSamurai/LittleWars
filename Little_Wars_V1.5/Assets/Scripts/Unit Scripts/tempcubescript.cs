using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempcubescript : MonoBehaviour {

    //bool hit = false;
    GameObject sfx;
    
	void Start () {
		
	}
	
	void Update () {
		
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Cannonball")
        {
           // if (!hit)
          //  {
                int num = Random.Range(0, 3);

                if(num == 0)
                    sfx = (GameObject)Instantiate(Resources.Load("Prefabs/Pow"), transform.position, transform.rotation);
                else if(num == 1)
                    sfx = (GameObject)Instantiate(Resources.Load("Prefabs/Ka-Pow"), transform.position, transform.rotation);
                else
                    sfx = (GameObject)Instantiate(Resources.Load("Prefabs/Bam"), transform.position, transform.rotation);

                sfx.transform.position += new Vector3(0, 2.5f, 0);
                sfx.transform.position += transform.right * Random.Range(-5, 6);
           //     hit = true;
           // }
        }
    }
}
