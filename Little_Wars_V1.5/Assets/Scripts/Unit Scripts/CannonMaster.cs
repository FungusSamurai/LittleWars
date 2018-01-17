using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CannonMaster
{
    private static CannonControll currentCannon = null;

    private static SoundMaster soundM = GameObject.Find("SoundMaster").GetComponent<SoundMaster>();

    public static void ResetCurrent()
    {
        currentCannon = null;
    }

    public static void HandleCannons()
    {
        if (currentCannon == null)
        {
            ScanForCannon();
        }
    }

    private static void ScanForCannon()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) /*&& Events.GetComponent<Pause>().paused == false*/)
        {
            Ray ray;
            RaycastHit hit;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Cannon c = hit.transform.gameObject.GetComponent<Cannon>();
                if (c != null)
                {
                    if (c.player == PlayerMaster.CurrentTurn && !c.Moved && c.shots != 0)
                    {
                        currentCannon = hit.transform.GetComponentInChildren<CannonControll>();
                        CurrentPowerShow();
                        currentCannon.Selected = true;
                        currentCannon.StartCoroutine(CannonMaster.MoveToCannon());
                    }
                }
            }
        }
    }

    private static IEnumerator MoveToCannon()
    {
        if (!currentCannon.C.Moved)
        {
            HighlightMaster.HighlightUnitToggle(false, currentCannon.C);
        }

        yield return TopDownCamera.LerpToPosition(currentCannon.CameraPos[0].transform.position, currentCannon.CameraPos[0].transform.rotation);
        currentCannon.CannonCam.GetComponent<Camera>().enabled = true;
        TopDownCamera.Instance.GetComponent<Camera>().enabled = false;

        UIMaster.FadePhanel((int)UIPannels.Cannon);
       // UIMaster.TogglePanelLock(false, (int)UIPannels.Action);
        UIMaster.FadePhanel((int)UIPannels.Action);
    }

    public static void SetAICannon(Cannon can)
    {
        currentCannon = can.GetComponentInChildren<CannonControll>();
        currentCannon.Selected = true;
        currentCannon.StartCoroutine(CannonMaster.MoveToCannon());
    }

    public static IEnumerator LeaveCannon()
    {
        TopDownCamera.Instance.transform.position = currentCannon.CannonCam.transform.position;
        TopDownCamera.Instance.transform.rotation = currentCannon.CannonCam.transform.rotation;
        TopDownCamera.Instance.GetComponent<Camera>().enabled = true;
        currentCannon.CannonCam.GetComponent<Camera>().enabled = false;
        UIMaster.FadePhanel((int)UIPannels.Cannon);
        //UIMaster.TogglePanelLock(true, (int)UIPannels.Action);
        UIMaster.FadePhanel((int)UIPannels.Action);

        yield return TopDownCamera.LerpToCurrentPlayer();
    }

    public static void SetCannon(Cannon c)
    {
        int fI = 0;
        int fC = 0;
        int speed;

        foreach (HexCell h in MapMaster.CellsWithinArea(c.CurrentHex, 1))
        {
            if (h.passable)
            {
                continue;
            }
            if (h.Unit != null)
            {
                if (h.Unit.Player == c.player)
                {
                    string name = h.Unit.UName;
                    if (name == "Infantry")
                    {
                        fI++;
                    }
                    else if (name == "Cavalry")
                    {
                        fC++;
                    }
                }
            }
        }

        c.Manned = DecideManned(fI, fC, out speed);
        c.shots = Cannon.Clip;
        c.SetMove(speed);
        c.Moved = speed == 0 ? true : false;
    }

    private static bool DecideManned(int infCount, int cavCount, out int speed)
    {
        speed = 0;
        if (infCount + cavCount >= Cannon.ActiveThreshold)
        {
            if (cavCount >= Cannon.ActiveThreshold)
            {
                speed = Cavalry.MaxMoveSpeed;
            }
            else
            {
                speed = Infantry.MaxMoveSpeed;
            }
            return true;
        }

        return false;
    }

    public static void CurrentUp()
    {
        if (currentCannon != null)
        {
            currentCannon.ups();
        }
    }

    public static void CurrentDown()
    {
        if (currentCannon != null)
        {
            currentCannon.downs();
        }
    }

    public static void CurrentLeft()
    {
        if (currentCannon != null)
        {
            currentCannon.lefts();
        }
    }

    public static void CurrentRight()
    {
        if (currentCannon != null)
        {
            currentCannon.rights();
        }
    }

    public static void CurrentFire()
    {
        if (currentCannon != null)
        {
            currentCannon.firecannon();
            Debug.Log("pewpew");
        }
    }

    private static void CurrentPowerShow()
    {
        if (currentCannon != null)
        {
            UIMaster.PowerSlider.value = currentCannon.powershow();
        }
    }

    public static void CurrentUpPower()
    {
        if (currentCannon != null)
        {
            UIMaster.PowerSlider.value = currentCannon.Powerup();
            soundM.CannonPower();
        }
    }

    public static void CurrentDownPower()
    {
        if (currentCannon != null)
        {
            UIMaster.PowerSlider.value = currentCannon.powerdown();
            soundM.CannonPower();
        }
    }

    public static void ExitCannon()
    {
        if (currentCannon != null)
        {
            currentCannon.StartCoroutine(LeaveCannon());
            currentCannon.Selected = false;
            currentCannon.DestroyIndicator();
            currentCannon.ReturnToDefault();

            if (!currentCannon.C.Moved)
            {
                HighlightMaster.HighlightUnitToggle(true, currentCannon.C);
            }

            currentCannon = null;

            soundM.ButtonPress();
        }
    }

    public static CannonControll CurrentCannon
    { get { return currentCannon; } }

    public static void CheckCannonCapture()
    {
        for (int i = 0; i < PlayerMaster.CurrentPlayer.Can.Count; i++)
        {
            List<HexCell> adj = MapMaster.CellsWithinArea(PlayerMaster.CurrentPlayer.Can[i].CurrentHex, 1);
            int finfC = 0;
            int fcalC = 0;
            int einfC = 0;
            int ecalC = 0;
            foreach (HexCell h in adj)
            {
                if (h.Unit != null)
                {
                    if (h.Unit.UName == "Infantry")
                    {
                        if (h.Unit.Player == PlayerMaster.CurrentTurn)
                        {
                            finfC++;
                        }

                        if (h.Unit.Player != PlayerMaster.CurrentTurn)
                        {
                            einfC++;
                        }
                    }

                    else if (h.Unit.UName == "Cavalry")
                    {
                        if (h.Unit.Player == PlayerMaster.CurrentTurn)
                        {
                            fcalC++;
                        }

                        if (h.Unit.Player != PlayerMaster.CurrentTurn)
                        {
                            ecalC++;
                        }
                    }
                }
            }

            if (finfC + fcalC == 0 && einfC + ecalC >= Cannon.ActiveThreshold)
            {
                PlayerMaster.CurrentPlayer.Can[i].Shots = Cannon.Clip;
                PlayerMaster.CurrentPlayer.Can[i].Moved = false;
                PlayerMaster.CurrentPlayer.Can[i].Manned = true;

                if (ecalC >= Cannon.ActiveThreshold)
                {
                    PlayerMaster.CurrentPlayer.Can[i].SetMove(Cavalry.MaxMoveSpeed);
                }
                else
                {
                    PlayerMaster.CurrentPlayer.Can[i].SetMove(Infantry.MaxMoveSpeed);
                }
                PlayerMaster.CurrentPlayer.Can[i].Player = PlayerMaster.PriorTurn;
                PlayerMaster.OtherPlayer.Can.Add(PlayerMaster.CurrentPlayer.Can[i]);
                ScoreMaster.SwapPoints(UnitTypes.Cannon, PlayerMaster.CurrentTurn);
                PlayerMaster.CurrentPlayer.Can.Remove(PlayerMaster.CurrentPlayer.Can[i]);
                i--;
            }
        }
    }
}
