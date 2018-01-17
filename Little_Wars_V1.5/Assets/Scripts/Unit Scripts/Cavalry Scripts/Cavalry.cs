using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cavalry : Unit
{
    public static int MaxMoveSpeed = 5;
    public static int PointValue = 2;
    public bool dead = false;
    public bool dying = false;
    CannonCombat cc;
    GameObject sfx;

	public override void Awake ()
    {
        player = -1;
        moveSpeed = MaxMoveSpeed;
        moved = false;
        uName = "Cavalry";
        uDirection = (int)UnitRotations.none;
        cc = GameObject.FindGameObjectWithTag("MapControl").GetComponent<CannonCombat>();
    }
	
	// Update is called once per frame
	public override void Update ()
    {

	}

    public override int MoveSpeed
    {
        get { return moveSpeed; }
    }

    void OnCollisionEnter(Collision col)
    {
        if (!dying)
        {
            if (col.gameObject.tag == "Cavalry" || col.gameObject.tag == "Infantry")
            {
                dying = true;
                StartCoroutine("Falling");
            }
            if (col.gameObject.tag == "Cannonball")
            {
                int num = Random.Range(0, 3); //the max is exclusive meaning it picks from 0 - 2

                if (num == 0) //Picks which one to load based off of th random number
                    sfx = (GameObject)Instantiate(Resources.Load("Prefabs/Pow"), transform.position, transform.rotation);
                else if (num == 1)
                    sfx = (GameObject)Instantiate(Resources.Load("Prefabs/Ka-Pow"), transform.position, transform.rotation);
                else
                    sfx = (GameObject)Instantiate(Resources.Load("Prefabs/Bam"), transform.position, transform.rotation);

                sfx.transform.position += new Vector3(0, 2.5f, 0); //moves object up
                sfx.transform.position += transform.right * Random.Range(-5, 6); //and slightly to the side

                if (col.gameObject.GetComponent<FirstKill>().firstKill == false)
                {
                    dead = true;
                    col.gameObject.GetComponent<FirstKill>().firstKill = true;
                }
                dying = true;
                StartCoroutine("Falling");
            }
        }
    }

    IEnumerator Falling()
    {
        cc.unitsFalling += 1; //Lets the CannonCombat script know one more unit is falling
        cc.checkUnits = true; //Let's the CannonCombat script know to check for units
        cc.unitList.Add(gameObject); //Adds itself to a list of falling units

        Rigidbody rigid = gameObject.GetComponent<Rigidbody>();
        float waitForIt = 1.5f; //Object must be still for three seconds before we check if it's dead or alive

        while (waitForIt > 0)
        {
            if (rigid.velocity.sqrMagnitude > 1f)
                waitForIt = 1.5f; //If the object is still moving we keep waiting
            else waitForIt -= Time.deltaTime; //If the unit has stopped moving we decrease waitForIt by the time since the last frame
            yield return null;
        }

        float x = transform.eulerAngles.x;
        float z = transform.eulerAngles.z;

        if (x > 180) //Euler angles don't become negative so I have to wrap them back around (Brennan)
            x-=360;

        if(z > 180)
            z-= 360;
        if (Mathf.Abs(x) > 45 || Mathf.Abs(z) > 45) //If an object is tilted more than 45 degrees it's considered dead
        {
            dead = true;
        }
        else dying = false;
        cc.unitsFalling -= 1;
    }

    public override bool Moved { get { return moved; } set { moved = value; } }
}
