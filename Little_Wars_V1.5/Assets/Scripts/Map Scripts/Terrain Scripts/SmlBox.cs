using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmlBox : Unit, ITerrain {

    public override void Awake()
    {
        uDirection = 0;
        player = -1;
        moveSpeed = 0;
        moved = false;
        uName = "SmlBox";
    }

    // Update is called once per frame
    public override void Update()
    {

    }

    void OnCollisionEnter(Collision col)
    {
       //Do stuff
    }

    void ITerrain.SetTiles()
    {
        List<HexCell> cells = MapMaster.CellsWithinArea(currentHex, moveSpeed);

        foreach (HexCell h in cells)
        {
            h.Passable = false;
        }
    }
    void ITerrain.SetPosition()
    {
        gameObject.transform.position = currentHex.spawnPoint.transform.position + new Vector3(0, gameObject.transform.localScale.y / 2 + 0.01f, 0);
    }

    void ITerrain.SetCollider()
    {
        gameObject.GetComponent<BoxCollider>().enabled = true;
    }

    void ITerrain.ResetTiles()
    {
        List<HexCell> cells = MapMaster.CellsWithinArea(currentHex, moveSpeed);

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
