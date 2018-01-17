using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMaster
{
    static AttackMaster instance;

    public int combatCount;
    private List<List<Unit>> combatAreas;
    private List<HexCell> combatPoints;

    public AttackMaster()
    {
        combatCount = 0;
        combatAreas = new List<List<Unit>>();
        combatPoints = new List<HexCell>();
        instance = this;
    }

    public static void BuildCombatPoints(HexCell checkCell)
    {
        if (checkCell.Unit != null && checkCell.Unit.UName == "Cannon")
        {
            return;
        }

        foreach (HexCell h in MapMaster.CellsWithinArea(checkCell, 1))
        {
            if (h.Passable)
            {
                continue;
            }
            else if (h.Unit != null)
            {
                if (h.Unit.Player == PlayerMaster.PriorTurn && h.Unit.UName != "Cannon")
                {
                    Debug.Log("PointAdded" + h.Unit.Player);
                    instance.combatPoints.Add(checkCell);
                    break;
                }
            }
        }

      //  foreach(Unit u in PlayerMaster.CurrentPlayer)
    }
    public static void BuildCombatAreas()
    {
        if (instance.combatPoints.Count == 0)
        {
            return;
        }

        while (instance.combatPoints.Count != 0)
        {
            Unit origin = instance.combatPoints[0].Unit;
            instance.combatPoints.RemoveAt(0);

            List<Unit> combatArea = BuildCombatArea(origin);
            instance.combatAreas.Add(combatArea);
            foreach (Unit u in combatArea)
            {
                HighlightMaster.HighlightUnitToggle(true, u);
            }
        }
        instance.combatCount = instance.combatAreas.Count;
    }

    public static List<Unit> FightResults()
    {
        List<Unit> toKill = new List<Unit>();
        int p1 = 0;
        int p2 = 0;

        if (instance.combatAreas.Count != 0)
        {
            foreach (Unit u in instance.combatAreas[0])
            {
                if (u.Player == 0)
                {
                    p1++;
                }
                else
                {
                    p2++;
                }
                HighlightMaster.HighlightUnitToggle(true, u);
            }
        }

        if (p1 == p2)
        {
            foreach (Unit u in instance.combatAreas[0])
                if (u != null)
                    toKill.Add(u);
        }
        else
        {
            if (p1 > p2)
            {
                p1 = p2;
            }

            else
            {
                p2 = p1;
            }

            foreach (Unit u in instance.combatAreas[0])
            {
                if (u != null)
                {


                    if (u.Player == 0 && p1 != 0)
                    {
                        toKill.Add(u);
                        p1--;
                    }

                    else if (u.Player == 1 && p2 != 0)
                    {
                        toKill.Add(u);
                        p2--;
                    }
                }
            }
        }

        foreach (Unit u in instance.combatAreas[0])
        {
            if (u != null)
            {
                HighlightMaster.HighlightUnitToggle(false, u);
            }
        }

        instance.combatAreas.Remove(instance.combatAreas[0]);
        instance.combatCount--;
        return toKill;
    }

    public static void ResolveFight()
    {
        int p1 = 0;
        int p2 = 0;

        if (instance.combatAreas.Count != 0)
        {
            foreach (Unit u in instance.combatAreas[0])
            {
                if (u.Player == 0)
                {
                    p1++;
                }
                else
                {
                    p2++;
                }
                HighlightMaster.HighlightUnitToggle(true, u);
            }
        }

        if (p1 == p2)
        {
            foreach (Unit u in instance.combatAreas[0])
                if (u != null)
                    PlayerMaster.KillUnit(u);
        }
        else
        {
            if (p1 > p2)
            {
                p1 = p2;
            }

            else
            {
                p2 = p1;
            }

            foreach (Unit u in instance.combatAreas[0])
            {
                if (u != null)
                {
                    

                    if (u.Player == 0 && p1 != 0)
                    {
                        PlayerMaster.KillUnit(u);
                        p1--;
                    }

                    else if (u.Player == 1 && p2 != 0)
                    {
                        PlayerMaster.KillUnit(u);
                        p2--;
                    }
                }
            }
        }

        foreach (Unit u in instance.combatAreas[0])
        {
            if (u != null)
            {
                HighlightMaster.HighlightUnitToggle(false, u);
            }
        }

        instance.combatAreas.Remove(instance.combatAreas[0]);
        instance.combatCount--;
    }

    private static List<Unit> BuildCombatArea(Unit o)
    {
        List<Unit> clusterPoints = new List<Unit>();
        List<Unit> combatCluster = new List<Unit>();
        List<Unit> combatArea = new List<Unit>();
        //List<Unit> result = new List<Unit>();

        clusterPoints.Add(o);

        //Find all conected combat points
        FeelForPoints(ref clusterPoints);
        //build combat cluster
        BuildCluster(ref combatCluster, ref clusterPoints);
        //build combat area from cluster
        ExpandCluster(ref combatArea, ref combatCluster);

        return combatArea;
    }


    private static void FeelForPoints(ref List<Unit> points)
    {
        List<Unit> open = new List<Unit>() {points[0]};
        List<Unit> closed = new List<Unit>() { points[0] };

        while (open.Count != 0)
        {
            Unit center = open[0];
            HexCell origin = points[0].CurrentHex;
            foreach (HexCell h in MapMaster.CellsWithinArea(center.CurrentHex, 1))
            {
                if (h.Passable)
                {
                    continue;
                }
                if (closed.Contains(h.Unit))
                {
                    continue;
                }
                if (h.Unit != null)
                {
                    if (h.Unit.Player != -1)
                    {

                        open.Add(h.Unit);
                        closed.Add(h.Unit);
                        if (instance.combatPoints.Contains(h))
                        {
                            points.Add(h.Unit);
                        }
                    }
                }
            }

            open.RemoveAt(0);
        }

    }

    private static void BuildCluster(ref List<Unit> cluster, ref List<Unit> points)
    {
        while (points.Count != 0)
        {
            instance.combatPoints.Remove(points[0].CurrentHex);
            cluster.Add(points[0]);

            Unit center = points[0];
            foreach (HexCell h in MapMaster.CellsWithinArea(center.CurrentHex, 1))
            {
                if (h.Passable)
                {
                    continue;
                }
                if (cluster.Contains(h.Unit))
                {
                    continue;
                }
                if (h.Unit != null)
                {
                    if (h.Unit.UName != "Cannon")
                    {
                        if (h.Unit.Player == PlayerMaster.PriorTurn)
                        {
                            cluster.Add(h.Unit);
                        }
                    }
                }
            }
            points.RemoveAt(0);
        }
    }

    private static void ExpandCluster(ref List<Unit> area, ref List<Unit> cluster)
    {
        List<Unit> open = new List<Unit>(cluster);
        area = new List<Unit>(cluster);
        while (open.Count != 0)
        {
            Unit center = open[0];
            foreach (HexCell h in MapMaster.CellsWithinArea(center.CurrentHex, 1))
            {
                if (h.Passable)
                {
                    continue;
                }
                if (area.Contains(h.Unit))
                {
                    continue;
                }
                if (h.Unit != null)
                {
                    if (h.Unit.UName != "Cannon")
                    {
                        if (h.Unit.Player != -1)
                        {
                            if (WithinClusterRange(ref cluster, h, 1))
                            {
                                open.Add(h.Unit);
                                area.Add(h.Unit);
                            }
                        }
                    }
                }
            }

            open.RemoveAt(0);
        }
    }

    private static bool WithinClusterRange(ref List<Unit> cluster, HexCell h, int range)
    {

        foreach (Unit u in cluster)
        {
            if (HexCell.CubeDistance(h, u.CurrentHex) <= range)
            {
                return true;
            }
        }

        return false;
    }

    public static Vector3 CenterOfCombat()
    {
        List<Unit> currentCombat = instance.combatAreas[0];

        float bigX = currentCombat[0].transform.position.x;
        float lilX = currentCombat[0].transform.position.x;
        float bigZ = currentCombat[0].transform.position.z;
        float lilZ = currentCombat[0].transform.position.z;

        for (int i = 1; i < currentCombat.Count; ++i)
            if (currentCombat[i] != null && currentCombat[i].transform.position.x < lilX)
                lilX = currentCombat[i].transform.position.x;

        for (int i = 1; i < currentCombat.Count; ++i)
            if (currentCombat[i] != null && currentCombat[i].transform.position.x > bigX)
                bigX = currentCombat[i].transform.position.x;

        for (int i = 1; i < currentCombat.Count; ++i)
            if (currentCombat[i] != null && currentCombat[i].transform.position.z > bigZ)
                bigZ = currentCombat[i].transform.position.z;

        for (int i = 1; i < currentCombat.Count; ++i)
            if (currentCombat[i] != null && currentCombat[i].transform.position.z < lilZ)
                lilZ = currentCombat[i].transform.position.z;

        return new Vector3((bigX + lilX) / 2, 12, (bigZ + lilZ) / 2);
    }

    public static int CombatCount
    {
        get { return instance.combatCount; }
    }

    public static List<Unit> CurrentCombatArea
    {
        get { return instance.combatAreas[0]; }
    }
}
