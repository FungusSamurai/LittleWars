using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    static TopDownCamera instance;

    private static float leftLimit, rightLimit, topLimit, bottomLimit, currentZoom, priorZoom, zoomLimit;
    private static bool movable;

    private static Camera cam;
    private static InputControl iC;
    private static Transform t;

    private bool lerping;

    // Use this for initialization
    void Start ()
    {
        lerping = false;
        cam = GetComponent<Camera>();
        movable = false;
        iC = new InputControl();
        zoomLimit = 20;
        instance = this;
        t = instance.transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (movable)
        {
            iC.HandleInput();
        }
	}

    public static void ActivateMainCamera(float h, float w, int r)
    {
        GameObject g = MapMaster.gMap;

        topLimit = g.transform.position.z - (h * (r - 1)) - 5;
        bottomLimit = g.transform.position.z + 5;
        leftLimit = g.gameObject.transform.position.x;
        rightLimit = g.transform.position.x + (w * (r - 1) * 0.75f);

        PlayerMaster.CurrentPlayer.CameraPosistion = new Vector3((leftLimit + rightLimit) / 2, 15, bottomLimit);
        PlayerMaster.OtherPlayer.CameraPosistion = new Vector3((leftLimit + rightLimit) / 2, 15, topLimit);

        t.position = PlayerMaster.CurrentPlayer.CameraPosistion;
        cam.fieldOfView = PlayerMaster.CurrentPlayer.CameraZoom;

        iC = new InputControl(leftLimit, rightLimit, topLimit, bottomLimit, zoomLimit, cam, t);
        movable = true;
    }

    public static void ChangePlayerCamera()
    {
        instance.StartCoroutine(UpdateCameraPosistion());
        iC.ChangeTargets();
        iC.Reverse();
    }

    private static IEnumerator UpdateCameraPosistion()
    {
        while (instance.lerping)
        {
            yield return null;
        }
        PlayerMaster.OtherPlayer.CameraPosistion = t.position;
        PlayerMaster.OtherPlayer.CameraZoom = cam.fieldOfView;

        yield return LerpToCurrentPlayer();
    }

    public static IEnumerator LerpToCurrentPlayer() //Changes the camera to the other player's camera position when the turn has changed
    {
        while (instance.lerping)
        {
            yield return null;
        }
        movable = false;
        instance.lerping = true;
        int increment = 1;
        Vector3 start = t.position;
        Vector3 goal = PlayerMaster.CurrentPlayer.CameraPosistion;
        Quaternion sRot = t.rotation;
        Quaternion gRot = PlayerMaster.CurrentPlayer.CameraRotation;
        float zStart = cam.fieldOfView;
        float zGoal = PlayerMaster.CurrentPlayer.CameraZoom;
        while (Vector3.Distance(t.position, goal) > 0.05f)
        {
            yield return new WaitForSeconds(0.03f);
            t.position = Vector3.Lerp(start, goal, 0.1f * increment);
            t.rotation = Quaternion.Lerp(sRot, gRot, 0.1f * increment);
            cam.fieldOfView = Mathf.Lerp(zStart, zGoal, 0.1f * increment);
            ++increment;
        }
        instance.lerping = false;
        movable = true;
        yield return null;
    }

    public static IEnumerator LerpToPosition(Vector3 targetP, Quaternion targetR)
    {
        while (instance.lerping)
        {
            yield return null;
        }
        movable = false;
        instance.lerping = true;
        int increment = 1;
        Vector3 start = t.position;
        Vector3 goal = targetP;
        Quaternion sRot = t.rotation;
        float zStart = cam.fieldOfView;
        float zGoal = 60;
        while (Vector3.Distance(t.position, goal) > 0.05f)
        {
            yield return new WaitForSeconds(0.03f);
            t.position = Vector3.Lerp(start, goal, 0.1f * increment);
            t.rotation = Quaternion.Lerp(sRot, targetR, 0.1f * increment);
            cam.fieldOfView = Mathf.Lerp(zStart, zGoal, 0.1f * increment);
            ++increment;
        }
        instance.lerping = false;
        yield return null;
    }

    public static void AllowMovement(bool allow)
    {
        movable = allow;
    }

    public static TopDownCamera Instance
    {
        get { return instance; }
    }

    public static bool IsLearping
    {
        get { return instance.lerping; }
    }

}
