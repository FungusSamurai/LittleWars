using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonCombat : MonoBehaviour {

    public int unitsFalling = 0; //How many units are currently falling (Brennan)
    public List<GameObject> unitList; //List of which units are falling (Brennan)
    public bool checkUnits = false; //Do we need to start removing units

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(checkUnits && unitsFalling == 0) //If we need to check the units and all of said units have finished falling (Brennan)
        {

            for (int i = unitList.Count -1; i > -1; --i) //You have to move backwards through a list if you remove from it while iterating through it (Brennan)
            {
                if (unitList[i] != null)
                {
                    if (unitList[i].tag == "Infantry") //If it's an Infantry we get the Infantry script
                    {
                        if (unitList[i].GetComponent<Infantry>().dead)
                        {
                            //Unit tR = unitList[i].GetComponent<Unit>();
                            //ScoreMaster.UpdateScore(tR);
                            //DeleteUnitFromPlay(tR);
                            //Destroy(unitList[i]);
                            //unitList.Remove(unitList[i]);
                            PlayerMaster.KillUnit(unitList[i].GetComponent<Unit>());
                        }
                    }
                    else
                    {
                        if (unitList[i].GetComponent<Cavalry>().dead)
                        {
                           /* Unit tR = unitList[i].GetComponent<Unit>();
                            ScoreMaster.UpdateScore(tR);
                            DeleteUnitFromPlay(tR);
                            Destroy(unitList[i]);
                            unitList.Remove(unitList[i]);*/
                            PlayerMaster.KillUnit(unitList[i].GetComponent<Unit>());
                        }
                    }
                }
            }

            for(int i = 0; i < unitList.Count; ++i)
            {
                if (unitList[i] != null)
                {
                        unitList[i].transform.position = unitList[i].GetComponent<Unit>().CurrentHex.SpawnVector;
                        unitList[i].transform.rotation = unitList[i].GetComponent<Unit>().URotation();
                        unitList[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
                        unitList[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                }
            }

            unitList.Clear();
            checkUnits = false;
        }
	}

    public static void DeleteUnitFromPlay(Unit u)
    {
        PlayerMaster.UnitsPlayer(u.Player).DeleteUnit(u);
        MapMaster.Map[u.CurrentHex.R, u.CurrentHex.Q].Unit = null;
        MapMaster.Map[u.CurrentHex.R, u.CurrentHex.Q].Passable = true;
    }
}
