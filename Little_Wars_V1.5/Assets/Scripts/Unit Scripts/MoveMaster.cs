using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMaster
{
    private static MoveMaster instance;

   // private static HexCell source;
   // private static HexCell destination;
    private static Unit target;

    private List<Unit> uQue; // for group moving when its ready
    private int uSize;
    public static int UnitsToMove;

    public static int movingUnits;

    private static Material markerMat;
    private static string markerName = "HexRadius";
    private GameObject radiusMarker;
    private float timePressed;

    private static SoundMaster soundM;
    const int HOLD = 1;

    public MoveMaster()
    {
        markerMat = (Material)Resources.Load("Pictures/Materials/" + markerName);
        radiusMarker = null;
        timePressed = 0.0f;
        uQue = new List<Unit>();
        uSize = 0;
        instance = this;
        soundM = GameObject.Find("SoundMaster").GetComponent<SoundMaster>();
        movingUnits = 0;
    }

    public static void PrimeMoveVariables(Player p)
    {
        UnitsToMove = p.Inf.Count + p.Cav.Count + p.Can.Count;
       // source = null;
     //   destination = null;
        target = null;
        instance.uQue.Clear();
    }

    public static void ClearMoveVariables()
    {
        if (target != null)
        {
            HighlightMaster.ToggleTileHighlights(false, MapMaster.CellsWithinArea(target.CurrentHex, target.MoveSpeed));
        }

        target = null;
    }

    private void BuildMarker()
    {
        timePressed += Time.deltaTime;

        if (timePressed > HOLD)
        {
            timePressed = 0.0f;

            if (radiusMarker == null)
            {
                radiusMarker = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/UnitImagePlane"), target.CurrentHex.SpawnVector + Vector3.up, Quaternion.Euler(0.0f, 180.0f * PlayerMaster.CurrentTurn, 0.0f));

                radiusMarker.transform.localScale = new Vector3(0.4f, 0.0f, 0.4f);

                radiusMarker.GetComponent<Renderer>().material = markerMat;

                if (uQue.Count > 0)
                {
                    uQue.Clear();
                }
            }
            else
            {
                if (radiusMarker.transform.localScale.x > 1.2f)
                {
                    return;
                }

                timePressed = 0.0f;
                radiusMarker.transform.localScale += new Vector3(0.4f, 0.0f, 0.4f);
            }
            ++uSize;
        }
    }

    public static void DecideMove()
    {
        if (target != null)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                instance.BuildMarker();
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0) && instance.radiusMarker != null)
            {
                instance.timePressed = 0;

                foreach (HexCell h in MapMaster.CellsWithinArea(target.CurrentHex, instance.uSize))
                {
                    Unit u = h.Unit;
                    if (u != null)
                    {
                        if (!u.Moved && u.Player == PlayerMaster.CurrentTurn)
                        {
                            instance.uQue.Add(u);
                        }
                    }
                }

                GameObject.Destroy(instance.radiusMarker);
                instance.radiusMarker = null;
                instance.uSize = 0;
                // target = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) /*&& Events.GetComponent<Pause>().paused == false*/)
        {
            Ray ray;
            RaycastHit hit;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Unit u = hit.transform.gameObject.GetComponent<Unit>();
                HexCell h = hit.transform.gameObject.GetComponent<HexCell>();
                //Debug.Log(u == null ? "" : u.UName);

                if (u != null)
                {
                    SetTarget(u);
                }
                else if (h != null)
                {
                    if (instance.uQue.Count == 0)
                    {
                        EvaluateTile(h);
                    }
                    else
                    {
                        if (target != null)
                        {
                            HighlightMaster.ToggleTileHighlights(false, MapMaster.CellsWithinArea(target.CurrentHex, target.MoveSpeed));

                            Vector3 directionVector = HexCell.TravelDirection(target.CurrentHex.Cube, h.Cube);
                            Vector2 endpoint;
                            HexCell goal;

                            List<HexCell> newPath;

                            List<KeyValuePair<Unit, List<HexCell>>> paths = new List<KeyValuePair<Unit, List<HexCell>>>();

                            foreach (Unit un in instance.uQue)
                            {
                                un.CurrentHex.passable = true;
                            }
                            foreach (Unit un in instance.uQue)
                            {
                                endpoint = HexCell.NewLocationInOffset(un.CurrentHex.Cube, directionVector);
                                if (!MapMaster.IsCellOnMap((int)endpoint.y, (int)endpoint.x))
                                {
                                    continue;
                                }
                                goal = MapMaster.Map[(int)endpoint.y, (int)endpoint.x];
                                newPath = HexStar.GetPath(un.CurrentHex, goal, un.MoveSpeed);
                                if (newPath.Count > 0)
                                {
                                    paths.Add(new KeyValuePair<Unit, List<HexCell>>(un, newPath));
                                }
                                else
                                {
                                    un.CurrentHex.passable = false;
                                }
                            }

                            EvaluateTileGroup(paths);
                            instance.uQue.Clear();
                            if (!target.Moved && target.UName == "Infantry")
                            {
                                target.GetComponentInChildren<Animator>().Play("GoToIdle");
                            }
                            target = null;
                        }
                    }
                }
            }
        }
    }

    public static void SetTarget(Unit u)
    {
        if (!u.Moved && u.Player == PlayerMaster.CurrentTurn)
        {
            if (target != null)
            {
                HighlightMaster.ToggleTileHighlights(false, MapMaster.CellsWithinArea(target.CurrentHex, target.MoveSpeed));
                if (target.UName != "Cannon" && !target.Equals(u))
                {
                    target.GetComponentInChildren<Animator>().Play("GoToIdle");
                }
            }

            target = u;
            instance.timePressed = 0;
            SoundMaster.SelectUnit();

            if (u.UName != "Cannon")
            {
                u.GetComponentInChildren<Animator>().Play("AtenHut");
            }

            foreach (HexCell h in MapMaster.CellsWithinArea(u.CurrentHex, u.MoveSpeed))
            {
                if (HexStar.GetPath(u.CurrentHex, h, u.MoveSpeed).Count > 0)
                {
                    HighlightMaster.HighlightTile(h);
                }
            }

            if (instance.uQue.Count != 0)
            {
                instance.uQue.Clear();
            }
            //playthatfunkysound(h.Unit.UName);
        }
    }

    public static void EvaluateTile(HexCell h)
    {
        if (target != null && h.passable)
        {
            List<HexCell> path = HexStar.GetPath(target.CurrentHex, h, target.MoveSpeed);

            HighlightMaster.ToggleTileHighlights(false, MapMaster.CellsWithinArea(target.CurrentHex, target.MoveSpeed));

            if (path.Count > 0 )
            {
                target.StartCoroutine(FollowPath(path, target));
                SoundMaster.SelectUnit();
            }

            target = null;
        }
    }

    public static bool AIMove(Unit u, HexCell h)
    {
        if (u != null && h.passable)
        {
            List<HexCell> path = HexStar.GetPath(u.CurrentHex, h, u.MoveSpeed);

            if (path.Count > 0)
            {
                u.StartCoroutine(FollowPath(path, u));
                SoundMaster.SelectUnit();
                return true;
            }
        }
        return false;
    }

    public static void EvaluateTileGroup(List<KeyValuePair<Unit, List<HexCell>>> paths)
    {
        foreach (KeyValuePair<Unit, List<HexCell>> pair in paths)
        {
            if (pair.Value.Count > 0)
            {
                target.StartCoroutine(FollowPath(pair.Value, pair.Key));
                SoundMaster.SelectUnit();
            }
        }
    }

    private static IEnumerator FollowPath(List<HexCell> path, Unit u)
    {
        u.Moved = true;
        u.CurrentHex.Passable = true;
        u.CurrentHex.unit = null;
        path[path.Count - 1].passable = false;

        Animator a = null;
        bool infOrCav = u.UName == "Cannon" ? false : true;

        if(infOrCav)
            a = u.gameObject.GetComponentInChildren<Animator>();

        yield return new WaitForFixedUpdate();
        ++movingUnits;

        UIMaster.SetActionPanel(false);
        //  path[path.Count - 1].Unit = u;

        if (infOrCav)
            a.Play("MoveStart");

        Rigidbody r = u.gameObject.GetComponent<Rigidbody>();
        if (r != null)
        {
            r.detectCollisions = false;
            r.useGravity = false;
        }

        yield return new WaitForSeconds(0.25f);

        SoundMaster.StartUnitMove();

        while (path.Count > 0)
        {
            yield return new WaitForEndOfFrame();
            yield return LearpToTile(u, path[0]);
            path.RemoveAt(0);
        }

        SoundMaster.StopUnitMove();

        yield return new WaitForFixedUpdate();
        --movingUnits;

        if (!(movingUnits > 0))
        {
            UIMaster.SetActionPanel(true);
        }

        if (r != null)
        {
            r.detectCollisions = true;
            r.useGravity = true;
        }

        if (infOrCav)
            a.Play("MoveEnd");

        u.CurrentHex.Unit = u;
        u.transform.rotation = u.URotation();
        HighlightMaster.HighlightUnitToggle(false, u);
        AttackMaster.BuildCombatPoints(u.CurrentHex);
    }

    private static IEnumerator LearpToTile(Unit u, HexCell next)
    {
        float rate = 0;
        float delta = 0.25f;
        float limit = 1 + delta;

        HexCell current = u.CurrentHex;

        Vector3 startPos = u.transform.position;
        Vector3 endy = new Vector3(current.SpawnVector.x, next.SpawnVector.y, current.SpawnVector.z);
        Vector3 endPos = next.SpawnVector;

        u.transform.eulerAngles = HexCell.TravelAngle(current.spawnPoint.transform.position, next.spawnPoint.transform.position);

        float timer = 0.0f;

        while (next.Traversing)
        {
            yield return new WaitForFixedUpdate();

            timer += Time.deltaTime;
            if (timer > 2)
            {
                break;
            }
        }

        if (!next.Passable)
        {
            if (next.Unit != null && (next.Unit.Player == PlayerMaster.CurrentTurn))
            {
                yield return new WaitForFixedUpdate();
                endPos += Vector3.up * 2;
                //yield break; // NEEDS TO BE IMPROVED TO HANDLE UNITS ENDING IT ITS PATH INSTEAD OF JUST SKIPPING THE TILE
            }
        }

        next.Traversing = true;

        if (endy.y > startPos.y)
        {
            while (rate < limit)
            {
                yield return new WaitForFixedUpdate();
                u.transform.position = Vector3.Lerp(startPos, endy, rate);
                rate += delta;
            }
            rate = 0;
            startPos = u.transform.position;
        }

        while (rate < limit)
        {
            yield return new WaitForSeconds(Time.deltaTime*2);
            u.transform.position = Vector3.Lerp(startPos, endPos, rate);
            rate += delta;
        }

        next.Traversing = false;
        u.CurrentHex = next;

    }

    private static void MoveToTile(HexCell h, ref Unit u)
    {
        HighlightMaster.ToggleTileHighlights(false, MapMaster.CellsWithinArea(u.CurrentHex, u.MoveSpeed));

        MapMaster.Map[u.CurrentHex.R, u.CurrentHex.Q].Passable = true;
        MapMaster.Map[u.CurrentHex.R, u.CurrentHex.Q].Unit = null;

        u.gameObject.transform.position = h.SpawnVector;

        MapMaster.Map[h.R, h.Q].Unit = u;
        MapMaster.Map[h.R, h.Q].Passable = false;
        u.Moved = true;

        u.CurrentHex = MapMaster.Map[h.R, h.Q];

        HighlightMaster.HighlightUnitToggle(false, u);

        AttackMaster.BuildCombatPoints(u.CurrentHex);

        u = null;

        UnitsToMove--;
    }
}
