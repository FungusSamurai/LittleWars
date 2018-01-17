using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour {
    private int hits;
    public List<Infantry> InfHit;
    public List<Cavalry> CavHit;
	// Use this for initialization
	void Start () {
        hits = 0;
        InfHit = new List<Infantry>();
        CavHit = new List<Cavalry>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Infantry")
        {
            if(hits == 0)
            {
                DestroyImmediate(col.gameObject);
            }
        }
        else if (col.gameObject.tag == "Cavalry")
        {
            if (hits == 0)
            {
                DestroyImmediate(col.gameObject);
            }

        }
    }
}
