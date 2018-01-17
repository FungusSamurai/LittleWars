using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMaster {

    static GameObject cube; //Place holder object for the thing that gets clicked to change the view

    static ViewMaster()
    {
        cube = (GameObject)Resources.Load("Prefabs/Cube");
    }

    public static void PlaceCubes() //Places two cubes at the halfway point on both sides of the map and then rotates them to face the map
    {
        GameObject.Instantiate(cube, MapMaster.Map[(MapMaster.MapRadius - 1) / 2, 0].transform.position
                - new Vector3(MapMaster.Width * 2, -2, 0), Quaternion.Euler(new Vector3(0, 90, 0)));
        GameObject.Instantiate(cube, MapMaster.Map[(MapMaster.MapRadius - 1) / 2, MapMaster.MapRadius - 1].transform.position
                + new Vector3(MapMaster.Width * 2, 2, 0), Quaternion.Euler(new Vector3(0, -90, 0)));
    }

    public static void LerpToView(RaycastHit hit) //Called by CheckClick below. Changes UI pannels and lerps to the position of the clicked cube
    {
        GameBrain.ChangeAcceptInput(false);
        UIMaster.FadePhanel((int)UIPannels.Action);
        UIMaster.FadePhanel((int)UIPannels.View);
        TopDownCamera.Instance.StartCoroutine(TopDownCamera.LerpToPosition(hit.transform.position, hit.transform.rotation));
    }

    public static void ExitView() //Returns to the camera to the position is=t was at before clicking on the cube and switches the UI back
    {
        TopDownCamera.Instance.StartCoroutine(TopDownCamera.LerpToPosition(PlayerMaster.CurrentPlayer.CameraPosistion, PlayerMaster.CurrentPlayer.CameraRotation));
        TopDownCamera.AllowMovement(true);
        UIMaster.FadePhanel((int)UIPannels.Action);
        UIMaster.FadePhanel((int)UIPannels.View);
        GameBrain.ChangeAcceptInput(true);
    }

    public static void CheckClick() //Called in GameBrain under Update(). Checks if a cube has been clicked and calls LerpToView if so
    {
        if (Camera.main != null && Input.GetKeyDown(KeyCode.Mouse0) /*&& Events.GetComponent<Pause>().paused == false*/)
        {
            Ray ray;
            RaycastHit hit;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "MoveCamera")
                {
                    LerpToView(hit);
                }
            }
        }
    }

}
