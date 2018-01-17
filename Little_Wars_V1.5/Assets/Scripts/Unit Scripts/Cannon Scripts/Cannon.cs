using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Unit
{
    public static int PointValue = 10;
    public static int Clip = 2;
    public static int ActiveThreshold = 3;
    public bool manned;
    public int shots;

    // Use this for initialization
    public override void Awake()
    {
        moveSpeed = 0;
        moved = true;
        uName = "Cannon";
        manned = false;
        uDirection = (int)UnitRotations.none;
        currentHex = null;
    }

    // Update is called once per frame
    public override void Update()
    {

    }

    public override int MoveSpeed
    {
        get { return moveSpeed; }
    }

    public void SetMove(int s)
    {
        moveSpeed = s;
    }

    public override string UName { get { return uName; } }
    public override bool Moved { get { return moved; } set { moved = value; } }
    public bool Manned { get { return manned; } set { manned = value; } }
    public int Shots { get { return shots; } set { shots = value; } }
}
