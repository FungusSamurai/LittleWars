using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//as
public class AICannon : MonoBehaviour {

    public int range = 11; //How far the cannon looks for targets in HexCells
    int target = 0; //After the first shot, this increments to target a different enemy
    int player = 1; //AI is always the second player right now
    int currentCannon = 0; //Which cannon from the list of cannons we are shooting with. First index is 0
    float angle = 0.0f; //The horizontal difference in where the cannon in currently facing and where the target (at position aimAt) is
    Vector3 aimAt; //Location of the current Target

   // bool firstSet = true;
  //  bool increasingpower = true;
   // int num = 0;
   // RaycastHit hit;
   // float missDist, canDist;

    List<Unit> inRangeTargets; //List of enemy Infantry and Cavalry within "range"
    List<Unit> currentCluster;
    public List<Unit> biggestCluster;

    CannonControll canc;
    CannonCombat combat;

    public GameObject cube;

    //public GameObject cube;

	void Start () {
        //cc = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControl>();
        combat = GameObject.FindGameObjectWithTag("MapControl").GetComponent<CannonCombat>();
        //mh = GameObject.FindGameObjectWithTag("MapControl").GetComponent<MapHandler>();
        inRangeTargets = new List<Unit>();
        currentCluster = new List<Unit>();
        biggestCluster = new List<Unit>();

        cube = Instantiate(cube, Vector3.zero, Quaternion.identity);
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GeneticCannon.SetCannonParam(CannonMaster.CurrentCannon.ShotSpawn.transform.position, CannonMaster.CurrentCannon.transform.parent.forward, CannonMaster.CurrentCannon.ShotSpawn.transform.position);
            //GeneticCannon.EndPoint(CannonMaster.CurrentCannon.transform.localEulerAngles.x, CannonMaster.CurrentCannon.actual);
            FindTargets(CannonMaster.CurrentCannon.C);
        }
    }

    //Finds all nearby enemies and figures out where to aim. Called from Camera Control during Cannon phase and after a single cannon shot
    public void FindTargets(Cannon can)
    {
        if (can == null)
        {
            return;
        }

        HexCell cannonCell = can.CurrentHex;
        List<HexCell> inRangeCells = MapMaster.CellsWithinArea(cannonCell, range); //Gets all HexCells within "range" of the current cannon
   //     int l = 0;
        for (int i = 0; i < inRangeCells.Count; ++i)
        {
            //If the cell has an enemy unit on it that is not a cannon it is added to the list
            if (inRangeCells[i].Unit != null && inRangeCells[i].Unit.Player != 1 && inRangeCells[i].Unit.gameObject.tag != "cannon")
                inRangeTargets.Add(inRangeCells[i].Unit);
        }

        //For loop for inrange
        
        while(inRangeTargets.Count != 0) //runs through all aquired nearby targets
        {
            Debug.Log("Looping");
            currentCluster.Add(inRangeTargets[0]); //adds first one to current cluster
            inRangeTargets.RemoveAt(0); //removes that same one from inRangeTargets to prevent overlap
            inRangeCells = MapMaster.CellsWithinArea(currentCluster[0].CurrentHex, 1); //gets all cells that are 1 cell within the first target

            for (int i = 0; i < inRangeCells.Count; ++i) //loops through previously aquired list of cells
            {
                if (inRangeCells[i].Unit != null &&inRangeCells[i].Unit.gameObject.tag != "cannon" && !currentCluster.Contains(inRangeCells[i].Unit)) //if that cell has a unit that is not in the cluster
                {
                    currentCluster.Add(inRangeCells[i].Unit); //adds it too the current cluster
                    if (inRangeTargets.Contains(inRangeCells[i].Unit)) //if that unit was in the inRangeTarget list
                        inRangeTargets.Remove(inRangeCells[i].Unit); //remove that unit
                }
                
                //This loop is similiar to the one above. After it gets one enemy, it checks the cells around it for more
                //Then, after adding a new enemy, it checks the cells around THAT one for even more enemies, countinously adding them to the cluster
                for(int j = 0; j < currentCluster.Count; ++j)
                {
                    List<HexCell> inRangeCellsTwo = MapMaster.CellsWithinArea(currentCluster[j].CurrentHex, 1); //Cells surrounding the unit

                    for (int k = 0; k < inRangeCellsTwo.Count; ++k)
                        if (inRangeCellsTwo[k].Unit != null && !currentCluster.Contains(inRangeCellsTwo[k].Unit) && inRangeCellsTwo[k].Unit.gameObject.tag != "cannon") //if that cell has a unit that is not in the cluster
                        {
                            currentCluster.Add(inRangeCellsTwo[k].Unit); //adds it too the current cluster
                            if (inRangeTargets.Contains(inRangeCellsTwo[k].Unit)) //if that unit was in the inRangeTarget list
                                inRangeTargets.Remove(inRangeCellsTwo[k].Unit); //remove that unit
                        }
                }
            }
            
            //When the cluster is finished, we compare it to the biggest cluster we've found so far
            if (biggestCluster.Count < currentCluster.Count) //If the current cluster has more enemies than the biggest cluster, we make that cluster the new biggest
            {
                biggestCluster.Clear();
                for (int i = 0; i < currentCluster.Count; ++i)
                    biggestCluster.Add(currentCluster[i]);
            }
                    currentCluster.Clear();
        }

        if (biggestCluster.Count == 0)
        {
            Debug.Log("reached 1");
            inRangeTargets.Clear();
            currentCluster.Clear();
            biggestCluster.Clear();

            while (currentCannon < PlayerMaster.CurrentPlayer.Can.Count && PlayerMaster.CurrentPlayer.Can[currentCannon].shots == 0) //Moves through cannons until we are out of available cannons or we find one with enough shots
                ++currentCannon;
            Debug.Log("reached 2");
            if (currentCannon < PlayerMaster.CurrentPlayer.Can.Count && (PlayerMaster.OtherPlayer.Inf.Count > 0 || PlayerMaster.OtherPlayer.Cav.Count > 0)) //If we found a cannon that can be shot
                FindTargets(PlayerMaster.CurrentPlayer.Can[currentCannon]); //Find the targets within it's range and continue from there
            else
            {
                Debug.Log("Reached 3");
                //StartCoroutine(CannonMaster.LeaveCannon()); //If we are out of cannons then zoom back out
               // GameBrain.ChangeTurn();
            }
        }

        else
        {

            StartCoroutine(HandleCannons(can));
        }
        //AimAndShoot(can);
    }

    IEnumerator HandleCannons(Cannon can)
    {
        Debug.Log("handling " + can.gameObject.name);
        CannonMaster.SetAICannon(can);
        canc = can.transform.GetChild(0).transform.GetChild(0).GetComponent<CannonControll>();
        aimAt = CenterOfCluster(); //asigns aimAt as the center of the cluster
        cube.transform.position = aimAt;
        Vector3 dir = aimAt - can.transform.position;
        angle = Vector3.SignedAngle(dir, can.transform.GetChild(0).transform.forward, Vector3.up); //math to figure out the difference between the target's position and where the cannon is currently looking
        
        if(angle > 0)
            while (angle > 2.0f)
            {
                //moves cannon left until it is lined up with target
                canc.lefts();
                angle = Vector3.SignedAngle(dir, can.transform.GetChild(0).transform.forward, Vector3.up);
                yield return null;
            }
        else
            while(angle < -2.0f)
            {
                //moves cannon right until it is lined up with target
                canc.rights();
                angle = Vector3.SignedAngle(dir, can.transform.GetChild(0).transform.forward, Vector3.up);
                yield return null;
            }
       for (int i = 17; i >= 0; --i)
        {
            //Moves cannon's aim downward so that later we only have to move it up to line up shots
            canc.downs();
            yield return null;
        }
        for (int i = 30; i >= 0; --i)
        {
            //Sets cannon at max power to more easily line up shots. Will more than likely be changed later to make lob shots possible
            canc.Powerup();
            yield return null;
        }
        RaycastHit ray;
        int num = canc.RayCastEnemies(out ray); //function created in CannonControl. Checks if a unit is within the current cannon's path

        RaycastHit hit;
        Physics.Raycast(new Ray(cube.gameObject.transform.position + Vector3.up, Vector3.down), out hit, 2.0f , 1 << 8);

        if (hit.transform != null)
        {
            Debug.Log(hit.transform);
            if (hit.transform.gameObject.GetComponent<HexCell>() != null)
            {
                GeneticCannon.goal = hit.transform.gameObject.GetComponent<HexCell>();
                Debug.Log(GeneticCannon.goal.ToString());
                yield return GeneticCannon.SolveCannon();
            }
            else
            {
                GeneticCannon.goal = currentCluster[0].CurrentHex;
            }
        }

        while (num != -1)
        {
            //Move aim up until a unit is found
            //Needs to be modified to confirm the current target and avoid allies
            canc.ups();
            num = canc.RayCastEnemies(out ray);
            yield return null;
        }

        canc.firecannon();


        //StartCoroutine(AimAndShoot(can));

        while (!combat.checkUnits)
            yield return null;
        //Check units becomes true when a unit has been touched by a cannonball and becomes false once all "dead" units are removed from play
        while (combat.checkUnits)
            yield return null;

        //if (inRangeTargets[target] == null)
           // ++target; //If target is dead, move to the next one
        //need proper target reprioritizing

        if (can.shots != 0) //if there is ammo left in the cannon, shoot again with the same cannon
        {
            inRangeTargets.Clear();
            currentCluster.Clear();
            biggestCluster.Clear();
            //yield return new WaitForSeconds(10f);
            FindTargets(can);
        }
        else
        {
            while (currentCannon < PlayerMaster.CurrentPlayer.Can.Count && PlayerMaster.CurrentPlayer.Can[currentCannon].shots == 0) //Moves through cannons until we are out of available cannons or we find one with enough shots
                ++currentCannon;

            if (currentCannon < PlayerMaster.CurrentPlayer.Can.Count && PlayerMaster.OtherPlayer.Inf.Count > 0) //If we found a cannon that can be shot
            {
                //yield return new WaitForSeconds(10f);
                inRangeTargets.Clear();
                currentCluster.Clear();
                biggestCluster.Clear();
                FindTargets(PlayerMaster.CurrentPlayer.Can[currentCannon]); //Find the targets within it's range and continue from there
            }
            else
            {
                Debug.Log("Out of cannons");
                StartCoroutine(CannonMaster.LeaveCannon()); //If we are out of cannons then zoom back out
                GameBrain.ChangeTurn();
            }
        } 
    }

    Vector3 CenterOfCluster() //find center of cluster of enemies
    {
        //Future change: if only 2 enemies, just shoot one
        //Check accuracy at longer ranges
        float bigX = biggestCluster[0].transform.position.x;
        float lilX = biggestCluster[0].transform.position.x;
        float bigZ = biggestCluster[0].transform.position.z;
        float lilZ = biggestCluster[0].transform.position.z;

        for (int i = 1; i < biggestCluster.Count; ++i)
            if (biggestCluster[i] != null && biggestCluster[i].transform.position.x < lilX)
                lilX  = biggestCluster[i].transform.position.x;

        for (int i = 1; i < biggestCluster.Count; ++i)
            if (biggestCluster[i] != null && biggestCluster[i].transform.position.x > bigX)
                bigX = biggestCluster[i].transform.position.x;

        for (int i = 1; i < biggestCluster.Count; ++i)
            if (biggestCluster[i] != null && biggestCluster[i].transform.position.z > bigZ)
                bigZ = biggestCluster[i].transform.position.z;

        for (int i = 1; i < biggestCluster.Count; ++i)
            if (biggestCluster[i] != null && biggestCluster[i].transform.position.z < lilZ)
                lilZ = biggestCluster[i].transform.position.z;

        return new Vector3((bigX + lilX) / 2, 1, (bigZ + lilZ) / 2);

    }
   /* IEnumerator AimAndShoot(Cannon can)
    {
        while (true)
        {
            num = canc.RayCastEnemies(out hit);
            if (num != -1 && num != player)
            {
                if (hit.transform.gameObject == inRangeTargets[target].gameObject)
                {
                    canc.firecannon();
                    break;
                }

            }
            if (num == player)
                break; //Coding that later
            if (num != -1)
            {
                missDist = Vector3.Distance(hit.transform.position, can.transform.position);
                canDist = Vector3.Distance(inRangeTargets[target].transform.position, can.transform.position);
                if (missDist > canDist)
                {
                    if (firstSet)
                    {
                        increasingpower = false;
                        firstSet = false;
                        canc.powerdown();
                        yield return null;
                    }

                    else
                    {
                        if (increasingpower == false)
                        {
                            canc.ups();
                            increasingpower = true;
                            firstSet = false;
                            yield return null;
                        }

                        else
                        {
                            canc.powerdown();
                            yield return null;
                        }
                    }

                }
                else
                {
                    if (firstSet)
                    {
                        increasingpower = true;
                        firstSet = false;
                        canc.powerdown();
                        yield return null;
                    }

                    else
                    {
                        if (increasingpower == true)
                        {
                            canc.downs();
                            increasingpower = false;
                            firstSet = true;
                            yield return null;
                        }
                    }
                }
            }
        }
    } */
}
