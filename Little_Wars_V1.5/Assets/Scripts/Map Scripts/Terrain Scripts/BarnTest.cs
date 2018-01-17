using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnTest : Unit, ITerrain
{

    public override void Awake()
    {
        uDirection = 0;
        player = -1;
        moveSpeed = 1;
        moved = false;
        uName = "BarnTest";
    }

    // Update is called once per frame
    public override void Update()
    {

    }

    void OnCollisionEnter(Collision col)
    {
        //Do stuff.
    }

    void ITerrain.SetTiles()
    {
        List<HexCell> cells = new List<HexCell>();
        //Left side of Barn cells
        cells.Add(MapMaster.Map[currentHex.R-2, currentHex.Q - 2]);
        cells.Add(MapMaster.Map[currentHex.R-1, currentHex.Q - 2]);
        cells.Add(MapMaster.Map[currentHex.R, currentHex.Q - 2]);
        cells.Add(MapMaster.Map[currentHex.R+1, currentHex.Q - 2]);
        cells.Add(MapMaster.Map[currentHex.R+2, currentHex.Q - 2]);
        //Right side of Barn cells
        cells.Add(MapMaster.Map[currentHex.R - 2, currentHex.Q + 2]);
        cells.Add(MapMaster.Map[currentHex.R - 1, currentHex.Q + 2]);
        cells.Add(MapMaster.Map[currentHex.R, currentHex.Q + 2]);
        cells.Add(MapMaster.Map[currentHex.R + 1, currentHex.Q + 2]);
        cells.Add(MapMaster.Map[currentHex.R + 2, currentHex.Q + 2]);
        //Smaller doorway Cells
        cells.Add(MapMaster.Map[currentHex.R - 3, currentHex.Q - 1]);
        cells.Add(MapMaster.Map[currentHex.R - 3, currentHex.Q + 1]);


        foreach (HexCell h in cells)
        {
            h.Passable = false;
        }
    }
    void ITerrain.SetPosition()
    {
        gameObject.transform.position = currentHex.spawnPoint.transform.position;
    }

    void ITerrain.SetCollider()
    {
        foreach (Collider c in GetComponents<Collider>())
        {
            c.enabled = true;
        }
    }

    void ITerrain.ResetTiles()
    {
        List<HexCell> cells = new List<HexCell>();
        //Left side of Barn cells
        cells.Add(MapMaster.Map[currentHex.R - 2, currentHex.Q - 2]);
        cells.Add(MapMaster.Map[currentHex.R - 1, currentHex.Q - 2]);
        cells.Add(MapMaster.Map[currentHex.R, currentHex.Q - 2]);
        cells.Add(MapMaster.Map[currentHex.R + 1, currentHex.Q - 2]);
        cells.Add(MapMaster.Map[currentHex.R + 2, currentHex.Q - 2]);
        //Right side of Barn cells
        cells.Add(MapMaster.Map[currentHex.R - 2, currentHex.Q + 2]);
        cells.Add(MapMaster.Map[currentHex.R - 1, currentHex.Q + 2]);
        cells.Add(MapMaster.Map[currentHex.R, currentHex.Q + 2]);
        cells.Add(MapMaster.Map[currentHex.R + 1, currentHex.Q + 2]);
        cells.Add(MapMaster.Map[currentHex.R + 2, currentHex.Q + 2]);
        //Smaller doorway Cells
        cells.Add(MapMaster.Map[currentHex.R - 3, currentHex.Q - 1]);
        cells.Add(MapMaster.Map[currentHex.R - 3, currentHex.Q + 1]);

        foreach (HexCell h in cells)
        {
            h.Passable = true;
        }
    }

    void ITerrain.DestroySelf()
    {
        Destroy(gameObject);
    }
}
