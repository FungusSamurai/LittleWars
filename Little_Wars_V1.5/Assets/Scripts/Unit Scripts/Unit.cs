using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum UnitRotations //for easy reference to what each "rotation" int value means IE: currentRotation = (int)TerrainRotation.half means the terrain should have a y rotation of 90 degrees. 
{ none, quarter, half, threeQ, full }

public enum UnitTypes
{ Infantry, Cavalry, Cannon }

/// <summary>
/// Abstract class for all units in the game world, used to give the code a parent to put all child scripts into and help with organization, while being able to then refer to specific child instances. When need be.
/// </summary>
public abstract class Unit : MonoBehaviour
{
    public int player; // number corelating to the player that owns this unit 
    protected int moveSpeed; // the number of hex times a unit may move
    protected bool moved; // if the unit has been or can be moved / selected by the player
    protected string uName; // the name of the unit, used to not have to check a units game object name or tag to know that type it is. 
    protected HexCell currentHex; // the current hex the unit is on.
    public int uDirection;
    public bool inSquad = false;

    // The abstracted Awake and Update methods to be overrode by the needs of the child scripts.
    public abstract void Awake(); 
    public abstract void Update();

    // Accessor Methods
    public virtual int MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    public virtual bool Moved { get { return moved; } set { moved = value; } }
    public virtual string UName { get { return uName; } }
    public virtual HexCell CurrentHex { get { return currentHex; } set { currentHex = value; } }
    public virtual int Player { get { return player; } set { player = value; } }
    public virtual bool InSquad { get { return inSquad; } set { inSquad = value; } }
    public virtual int UDirection
    { get { return uDirection; }  set { uDirection = value; } }

    // Returns a rotation for units based on if they belong to player one or two. 
    public virtual Quaternion URotation()
    { return Quaternion.Euler(new Vector3(0.0f, 90*uDirection, 0.0f)); }
}
