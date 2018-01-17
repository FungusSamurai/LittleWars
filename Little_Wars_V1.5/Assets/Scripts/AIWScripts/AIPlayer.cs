using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour {
    //as
    static Unit target;
    static bool moving = false;
    static float distance;
    static int playernum = 1;
    static int moves;
    static List<Unit> otherlist;
    static List<List<Unit>> SquadMaster;
    static HexCell moveTo;
    static List<HexCell> openSpaces;
    static AICannon aic;
    static string[] deployment;

	static void FillDeployment () {

        aic = GameObject.FindGameObjectWithTag("MapControl").GetComponent<AICannon>();
        SquadMaster = new List<List<Unit>>();
        deployment = new string[] 
        {
            "Infantry", "Infantry", "Infantry", "Cavalry", "Cavalry", "Cavalry", "Cavalry", "Cavalry", "Infantry", "Infantry", "Infantry", "Cavalry", "Cavalry", "Cavalry", "Cavalry",
            "Cavalry","Cannon", "Infantry", "Infantry", "Infantry", "Cannon", "Infantry", "Cavalry", "Infantry", "Cannon", "Infantry", "Infantry", "Infantry", "Cannon", "Cavalry"
        };
        
	}

    //AI's current goal is destroying all enemies

    public static IEnumerator Deploy()
    {
        FillDeployment();

        //For the purposes of the alpha the AI only needs a single, reasonable deployment phase that it will use on each instance of the game.
        //Units should be reasonably spaced out. Cannons should spawn with enough units to carry them.
        //Some cannons will spawn with infantry. A small few will spawn with Cavalry.
        //In the future, deployment may be based on how the opponent has set up their deployment
        //Question: Are we going to use a point based system of deployment in the future, replacing the current X-amount-of-each-unit implementation

        for (int i = 0; i < 30; ++i)
        {
            GameObject g = (GameObject)GameObject.Instantiate(Resources.Load("prefabs/" + deployment[i] + (deployment[i] == "Cannon" ? "" : playernum.ToString())), PlayerMaster.CurrentPlayer.backLine[i].SpawnVector, Quaternion.identity);
            MapMaster.Map[PlayerMaster.CurrentPlayer.backLine[i].R, PlayerMaster.CurrentPlayer.backLine[i].Q].Unit = g.GetComponent<Unit>();
            MapMaster.Map[PlayerMaster.CurrentPlayer.backLine[i].R, PlayerMaster.CurrentPlayer.backLine[i].Q].Unit.Player = playernum;
            MapMaster.Map[PlayerMaster.CurrentPlayer.backLine[i].R, PlayerMaster.CurrentPlayer.backLine[i].Q].Unit.CurrentHex = PlayerMaster.CurrentPlayer.backLine[i];

            switch (deployment[i])
            {
                case ("Infantry"):
                    Unit inf = g.GetComponent<Infantry>();
                    PlayerMaster.CurrentPlayer.Inf.Add(inf);
                    break;
                case ("Cavalry"):
                    Unit cl = g.GetComponent<Cavalry>();
                    PlayerMaster.CurrentPlayer.Cav.Add(cl);
                    break;
                case ("Cannon"):
                    Cannon c = g.GetComponent<Cannon>();
                    c.Moved = false;
                    c.CurrentHex = PlayerMaster.CurrentPlayer.backLine[i];
                    PlayerMaster.CurrentPlayer.Can.Add(c);
                    break;
            }
            yield return new WaitForSeconds(0.1f);
        }

        foreach (Cannon c in PlayerMaster.CurrentPlayer.Can)
        {
            Cluster(c);
        }
        foreach (Cavalry cv in PlayerMaster.CurrentPlayer.Cav)
            Cluster(cv);
        foreach (Infantry i in PlayerMaster.CurrentPlayer.Inf)
            Cluster(i);

        GameBrain.ChangeTurn();
    }

    static void Cluster(Unit u)//Clusters Units together into squads. Pushes squads to list for storage and management. 
    {
        if (!u.InSquad) //if unit is not in a squad
        {
            //Make a list to store the squad into and a list to hold the hexes to be searched for units for said squad.
            List<Unit> squad = new List<Unit>(); //single squad
            List<HexCell> cellsToSearch; //area looked at for more squad members

            squad.Add(u); //adds unit to squad
            u.InSquad = true;
            //Debug.Log("Position of first thing is " + u.transform.position);
            //Use CellsWithinArea to obtain first three units within one move of current unit. Push said units to a new List. Push said List to master list of all current squads.
            for (int i = 1; squad.Count <= 3 && i < 4; ++i)
            {
                cellsToSearch = MapMaster.CellsWithinArea(u.CurrentHex, i);
                foreach (HexCell cell in cellsToSearch)
                {
                    if (cell.Unit != null && cell.Unit.InSquad == false && squad.Count < 4)
                    {
                        squad.Add(cell.Unit);
                        cell.Unit.inSquad = true;
                    }
                }
            }

            SquadMaster.Add(squad);
        }

#region   
        /*if (u.tag == "cannon")
            {
                switch (count)
                {
                    case 1:
                        list1 = squad;
                        break;
                    case 2:
                        list2 = squad;
                        break;
                    case 3:
                        list3 = squad;
                        break;
                    case 4:
                        list4 = squad;
                        break;
                }
                ++count;
            }
            
        
        //Take the average of all the positions of the units in the squad list and set a cluster goal for them to move to. 
        int avgQ = 0, avgR = 0; //Holds the total for the coordinates of all the hexcells in the squad. Used for calculating the goal position

        foreach(Unit n in squad)
        {
            avgQ += n.CurrentHex.Q;
            avgR += n.CurrentHex.R;
        }
        avgQ /= squad.Capacity;
        avgR /= squad.Capacity;

        if(MapMaster.Map[avgR, avgQ] != null && MapMaster.Map[avgR, avgQ].Unit == null)
        {
            //pass cell (MapMaster.Map[avgR, avgQ]) as goal to Move() or MoveMaster methods
        }
        else
        {
            //Use BestFirst to find a goal cell. H = manhattan distance to middle of enemies DZ.
            cellsToSearch.Clear();
            cellsToSearch = MapMaster.CellsWithinArea(MapMaster.Map[avgR, avgQ], 10);

            foreach(HexCell c in cellsToSearch)
            {

            }
        }
        */


        //Use Move() with the cluster goal.

#endregion

    }

    public static IEnumerator Move() //WIP AI movement. Currently kept simple for the purpose of making it into the alpha. Made a coroutine so the player can see each step
    {

        //Cannons move first and prioritize getting in range of an enemy but do not move if already in range of an enemy

        //yield return new WaitForSeconds(2.0f);

        //Cavalry and Infantry will move after and prioritize geting in range of the closest enemy
        //Closest enemy is relative to each unit. Units will not move if already adjacent to an enemy

        for (int s = 0; s< SquadMaster.Count; ++s)
        {
            otherlist = SquadMaster[s];
            bool noCannons = true;

            for(int i = 0; i < otherlist.Count; ++i)
            {
                if (otherlist[i].gameObject.tag == "cannon" && otherlist[i] != null)
                {
                    noCannons = false;
                    //Debug.Log("Found cannon");
                }
            }

            if(noCannons)
            {
                FindClosestEnemy();
                yield return new WaitForSeconds(.2f);

                MoveLeadSoldier();
                otherlist[0].StartCoroutine(MoveFollowers());
                while (moving) //boolean that is changed via MoveFollowers
                    yield return null;
            }
            else //Move cannons forward about 2 spaces, then move the units nearby 2 spaces as well
            {
                MoveCannon();
                yield return new WaitForSeconds(.2f);

                otherlist[0].StartCoroutine(MoveCannonFollowers());
                while (moving) //boolean that is changed via MoveCannonFollowers
                    yield return null;
            }
        }
        GameBrain.ChangeTurn();
    }

    public static IEnumerator Fight()
    {
        yield return new WaitForSeconds(2); //Gives the player a chance to see which units are about to fight
        GameBrain.ChangeTurn();
    }

    static void FindClosestEnemy()
    {
        if (PlayerMaster.OtherPlayer.Inf.Count > 0)
        {
            //distance between unit being moved and first enemy
            distance = Vector3.Distance(PlayerMaster.OtherPlayer.Inf[0].transform.position, otherlist[0].transform.position);
            //sets target as first enemy
            target = PlayerMaster.OtherPlayer.Inf[0];
        }

        else
        {
            //distance between unit being moved and first enemy
            distance = Vector3.Distance(PlayerMaster.OtherPlayer.Cav[0].transform.position, otherlist[0].transform.position);
            //sets target as first enemy
            target = PlayerMaster.OtherPlayer.Cav[0];
        }
        //loops through enemies and finds which one is the closest and sets target and distance based on closest enemy
        for (int i = 1; i < PlayerMaster.OtherPlayer.Inf.Count; ++i)
        {
            if (distance > Vector3.Distance(PlayerMaster.OtherPlayer.Inf[i].transform.position, otherlist[0].transform.position))
            {
                distance = Vector3.Distance(PlayerMaster.OtherPlayer.Inf[i].transform.position, otherlist[0].transform.position);
                target = PlayerMaster.OtherPlayer.Inf[i];
            }
        }

        //loops through cavalry
        for (int i = 0; i < PlayerMaster.OtherPlayer.Cav.Count; ++i)
        {
            if (distance > Vector3.Distance(PlayerMaster.OtherPlayer.Cav[i].transform.position, otherlist[0].transform.position))
            {
                distance = Vector3.Distance(PlayerMaster.OtherPlayer.Cav[i].transform.position, otherlist[0].transform.position);
                target = PlayerMaster.OtherPlayer.Cav[i];
            }
        }
    }

    static void MoveLeadSoldier()
    {
        MoveMaster.SetTarget(otherlist[0]); //Which unit it's about to move
        //moveTo is hexcell unit is trying to move to. Starts as the cell the Unit is already on
        moveTo = otherlist[0].CurrentHex;
        moves = otherlist[0].MoveSpeed;

        openSpaces = MapMaster.CellsWithinArea(moveTo, moves); //finds nearby hexcells
        distance = Vector3.Distance(target.CurrentHex.transform.position, otherlist[0].CurrentHex.transform.position); //distance between current hex and target hex.

        for (int i = 1; i < openSpaces.Count; ++i) //loops through the rest of the cells
            if (openSpaces[i].Unit == null && distance > Vector3.Distance(target.CurrentHex.transform.position, openSpaces[i].transform.position))
            {
                moveTo = openSpaces[i];
                distance = Vector3.Distance(target.CurrentHex.transform.position, openSpaces[i].transform.position);
            }

        openSpaces.Clear();

        MoveMaster.EvaluateTile(moveTo); //Actually moves unit
    }

    static IEnumerator MoveFollowers()
    {
        moving = true;
        target = otherlist[0];

        for (int n = 1; n < otherlist.Count; ++n)
        {
            MoveMaster.SetTarget(otherlist[n]);

            moveTo = otherlist[n].CurrentHex;
            moves = otherlist[n].MoveSpeed;

            openSpaces = MapMaster.CellsWithinArea(moveTo, moves); //finds nearby hexcells
            distance = Vector3.Distance(target.CurrentHex.transform.position, otherlist[n].CurrentHex.transform.position); //distance between current hex and target hex.

            for (int i = 1; i < openSpaces.Count; ++i) //loops through the rest of the cells
                if (openSpaces[i].Unit == null && distance > Vector3.Distance(target.CurrentHex.transform.position, openSpaces[i].transform.position))
                {
                    moveTo = openSpaces[i];
                    distance = Vector3.Distance(target.CurrentHex.transform.position, openSpaces[i].transform.position);
                }
            yield return new WaitForSeconds(.2f);
            MoveMaster.EvaluateTile(moveTo); //Actually moves unit
        }
        Debug.Log("1");
        moving = false;
    }

    static void MoveCannon()
    {
        Debug.Log("Move Cannons");
        HexCell moveTo;
        List<Unit> inRangeTargets = new List<Unit>();

        for (int i = 0; i < otherlist.Count; ++i) //searches squad list for the cannon.
        {
            if (otherlist[i].gameObject.tag == "cannon")
            {
                Debug.Log("Found Cannon");
                if (GameBrain.turnNum < 4)
                {
                    otherlist[i].Moved = false;
                    otherlist[i].MoveSpeed = 2;
                }

                HexCell cannonCell = otherlist[i].CurrentHex;
                List<HexCell> inRangeCells = MapMaster.CellsWithinArea(cannonCell, 11); //Gets all HexCells within "range" of the current cannon
                for (int j = 0; j < inRangeCells.Count; ++j)
                {
                    //If the cell has an enemy unit on it that is not a cannon it is added to the list
                    if (inRangeCells[j].Unit != null && inRangeCells[j].Unit.Player != PlayerMaster.CurrentTurn && inRangeCells[j].Unit.gameObject.tag != "Cannon")
                        inRangeTargets.Add(inRangeCells[j].Unit);
                }


                //if(inRangeTargets.Count < 0)
                // {
                MoveMaster.SetTarget(otherlist[i]);
                target = otherlist[i];
                moveTo = MapMaster.Map[target.CurrentHex.R - 2, target.CurrentHex.Q];
                MoveMaster.EvaluateTile(moveTo); //Actually moves unit
                // }
            }
        }
    }

    static IEnumerator MoveCannonFollowers()
    {
        moving = true;
        for (int i = 0; i < otherlist.Count; ++i) // loops through squad list to find everything else
            {
                if (otherlist[i].gameObject.tag != "cannon")
                   {
                       MoveMaster.SetTarget(otherlist[i]);
                       target = otherlist[i];

                       moveTo = MapMaster.Map[target.CurrentHex.R - 2, target.CurrentHex.Q];
                       MoveMaster.EvaluateTile(moveTo); //Actually moves unit   
                       yield return new WaitForSeconds(.2f);
                   }
            }
        Debug.Log("2");
        moving = false;
    }
}
