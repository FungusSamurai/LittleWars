using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployMaster
{
    public static DeployMaster instance;

    //int iLim, cLim, cnLim; //for unit limits
    int toSpawnUnit;
    int[] unitsUsed, p1List, p2List;

    public List<UnitOrder> uoList = new List<UnitOrder>();

    public DeployMaster()
    {
        //iLim = 20;
        //cLim = 12;
        //cnLim = 4;
        p1List = new int[4];
        p2List = new int[4];
        UnitDistributionMaster.P1Dist.CopyTo(p1List, 0);
        UnitDistributionMaster.P2Dist.CopyTo(p2List, 0);
        toSpawnUnit = -1;
        uoList = new List<UnitOrder>();
        unitsUsed = new int[] { p1List[0], p1List[1], p1List[2] };
        instance = this;
    }

    public static void RefreshUnitsUsed()
    {
        instance.toSpawnUnit = -1;
        if(PlayerMaster.CurrentTurn < 0)
            instance.unitsUsed = new int[] { instance.p1List[0], instance.p1List[1], instance.p1List[2] };
        else if(PlayerMaster.CurrentTurn >= 0)
            instance.unitsUsed = new int[] { instance.p2List[0], instance.p2List[1], instance.p2List[2] };
    }

    public static void SetTiles()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) /*&& Events.GetComponent<Pause>().paused == false*/)
        {
            Ray ray;
            RaycastHit hit;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "HexCell")
                {
                    HexCell h = hit.transform.gameObject.GetComponent<HexCell>();
                    if (PlayerMaster.CurrentPlayer.backLine.Contains(h))
                    {
                        int tSU = instance.toSpawnUnit;

                        if (MapMaster.Map[h.R, h.Q].Passable && tSU != -1)
                        {
                            if (instance.unitsUsed[tSU] > 0)
                            {
                                instance.unitsUsed[tSU]--;
                                instance.uoList.Add(new UnitOrder(h.r, h.q, tSU, (GameObject)GameObject.Instantiate(Resources.Load("prefabs/UnitImagePlane"), MapMaster.Map[h.R, h.Q].spawnPoint.transform.position + new Vector3(0.0f, 0.05f, 0.0f), Quaternion.Euler(0.0f, 180.0f*PlayerMaster.CurrentTurn, 0.0f))));
                                //aud.GetComponent<Audiomanager>().soundeffectplay(5);
                                SoundMaster.Deploy();

                                MapMaster.Map[h.R, h.Q].Passable = false;

                                instance.uoList[instance.uoList.Count - 1].icon.GetComponent<ImagePlaneTest>().SetUnitPicture(((UnitTypes)tSU).ToString(), PlayerMaster.CurrentTurn);
                            }
                        }
                        else if (!(MapMaster.Map[h.R, h.Q].Passable) && tSU == -1)
                        {
                            UnitOrder uO;
                            for (int i = 0; i < instance.uoList.Count; i++)
                            {
                                uO = instance.uoList[i];
                                if (h.r == uO.R && h.q == uO.Q)
                                {
                                    GameObject.Destroy(uO.icon);
                                    instance.unitsUsed[uO.unitType]++;
                                    h.Passable = true;
                                    instance.uoList.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                        UIMaster.UpdateDeployAmount();
                    }
                }
            }
        }
    }

    public static void SetTutorialTiles()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) /*&& Events.GetComponent<Pause>().paused == false*/)
        {
            Ray ray;
            RaycastHit hit;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                string[] index;
                if ((hit.transform.tag == "I" && instance.toSpawnUnit == 0) ||
                    (hit.transform.tag == "CV" && instance.toSpawnUnit == 1) ||
                    (hit.transform.tag == "CN" && instance.toSpawnUnit == 2)) //toSpawnUnit is the same unit as the required unit of the hexcell they clicked on
                {
                    index = hit.transform.gameObject.name.Split('_');
                    HexCell h = MapMaster.Map[int.Parse(index[0]), int.Parse(index[1])];
                    int tSU = instance.toSpawnUnit;
                    instance.unitsUsed[tSU]--;
                    instance.uoList.Add(new UnitOrder(h.r, h.q, tSU, (GameObject)GameObject.Instantiate(Resources.Load("prefabs/UnitImagePlane"), MapMaster.Map[h.R, h.Q].spawnPoint.transform.position + new Vector3(0.0f, 0.05f, 0.0f), Quaternion.Euler(0.0f, 180.0f * PlayerMaster.CurrentTurn, 0.0f))));

                    SoundMaster.Deploy();

                    MapMaster.Map[h.R, h.Q].Passable = false;

                    instance.uoList[instance.uoList.Count - 1].icon.GetComponent<ImagePlaneTest>().SetUnitPicture(((UnitTypes)tSU).ToString(), PlayerMaster.CurrentTurn);
                    
                    UIMaster.UpdateDeployAmount();
                    GameObject.Destroy(hit.transform.parent.gameObject);
                }
            }
        }
    }


    public static void DeployUnits()
    {
        string prefabpath = "Prefabs/";
        string prefabName;
        HexCell h;
        foreach (UnitOrder uO in instance.uoList)
        {
            h = MapMaster.Map[uO.R, uO.Q];
            prefabName = ((UnitTypes)uO.unitType).ToString();
            prefabName += prefabName == "Cannon" ? "" : PlayerMaster.CurrentTurn.ToString();

            Unit u = ((GameObject)GameObject.Instantiate(Resources.Load(prefabpath + prefabName), h.SpawnVector, Quaternion.identity)).GetComponent<Unit>();

            u.UDirection = (int)(PlayerMaster.CurrentTurn == 0 ? UnitRotations.half : UnitRotations.none);
            u.Player = PlayerMaster.CurrentTurn;
            u.gameObject.transform.rotation = u.URotation();

            MapMaster.Map[uO.R, uO.Q].Unit = u;
            MapMaster.Map[uO.R, uO.Q].Unit.CurrentHex = MapMaster.Map[uO.R, uO.Q];
            

            switch (uO.unitType)
            {
                case (0):
                    PlayerMaster.CurrentPlayer.Inf.Add(u);
                    break;
                case (1):
                    PlayerMaster.CurrentPlayer.Cav.Add(u);
                    break;
                case (2):
                    Cannon c = (Cannon)u;
                    c.shots = 0;
                    PlayerMaster.CurrentPlayer.Can.Add(c);
                    break;
            }

            GameObject.Destroy(uO.icon);

            ScoreMaster.GivePoints(ScoreMaster.PointValues[uO.unitType], u.Player);
        }

        instance.uoList.Clear();
        RefreshUnitsUsed();
    }

    public static IEnumerator DeployUnitsCoroutine()
    {
        GameBrain.ChangeAcceptInput(false);
        string prefabpath = "Prefabs/";
        string prefabName;
        HexCell h;
        foreach (UnitOrder uO in instance.uoList)
        {
            yield return new WaitForSeconds(0.1f);
            h = MapMaster.Map[uO.R, uO.Q];
            prefabName = ((UnitTypes)uO.unitType).ToString();
            prefabName += prefabName == "Cannon" ? "" : PlayerMaster.CurrentTurn.ToString();

            Unit u = ((GameObject)GameObject.Instantiate(Resources.Load(prefabpath + prefabName), h.SpawnVector, Quaternion.identity)).GetComponent<Unit>();

            u.UDirection = (int)(PlayerMaster.CurrentTurn == 0 ? UnitRotations.half : UnitRotations.none);
            u.Player = PlayerMaster.CurrentTurn;
            u.gameObject.transform.rotation = u.URotation();

            MapMaster.Map[uO.R, uO.Q].Unit = u;
            MapMaster.Map[uO.R, uO.Q].Unit.CurrentHex = MapMaster.Map[uO.R, uO.Q];


            switch (uO.unitType)
            {
                case (0):
                    PlayerMaster.CurrentPlayer.Inf.Add(u);
                    break;
                case (1):
                    PlayerMaster.CurrentPlayer.Cav.Add(u);
                    break;
                case (2):
                    Cannon c = (Cannon)u;
                    c.shots = 0;
                    PlayerMaster.CurrentPlayer.Can.Add(c);
                    break;
            }

            GameObject.Destroy(uO.icon);

            ScoreMaster.GivePoints(ScoreMaster.PointValues[uO.unitType], u.Player);

            UIMaster.DisplayScore();
        }
        
        instance.uoList.Clear();
        RefreshUnitsUsed();
        GameBrain.ChangeAcceptInput(true);
    }

    public static void SetToSpawnUnit(int u)
    {
        instance.toSpawnUnit = u;
    }

    public static int UnitsUsed(int i)
    {
        return instance.unitsUsed[i];
    }
}

public struct UnitOrder
{
    public int R;
    public int Q;
    public int unitType;
    public GameObject icon;

    public UnitOrder(int r = 0, int q = 0, int ut = 0, GameObject i = null)
    {
        R = r;
        Q = q;
        unitType = ut;
        icon = i;
    }
}
