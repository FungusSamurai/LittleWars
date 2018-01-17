using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMakerCamera : MonoBehaviour
{
    private static readonly float PanSpeed = 40f;
    private static readonly float ScrollSpeed = 300f;

    private Vector3 startPos;
    private Quaternion startRot;

    private float rotationX = 0.0f;
    private float rotationY = -90.0f;
    private float maxYAngle = 90f;
    private float sensitivity = 5f;

    // Use this for initialization
    void Start ()
    {
        startPos = transform.position;
        startRot = transform.rotation;

        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
    }
	
	// Update is called once per frame
	void Update ()
    {
        //On right mouse button down, rotate camera based on its movement.
        if (Input.GetMouseButton(1))
        {
            RollCamera();
        }

        PanCamera(Input.mousePosition);

        //Reset Camera
        if (Input.GetKeyDown("backspace"))
        {
            transform.position = startPos;
            transform.rotation = startRot;
        }
    }

    void RollCamera()
    {
        rotationX += Input.GetAxis("Mouse X") * sensitivity;
        rotationY += Input.GetAxis("Mouse Y") * sensitivity;
        rotationY = Mathf.Clamp(rotationY, -maxYAngle, maxYAngle);

        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
    }

    void PanCamera(Vector3 newPanPosition)
    {
        transform.position += transform.forward * PanSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        transform.position += transform.right * PanSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
        if(!(Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
            transform.position += transform.up * ScrollSpeed * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;
    }
}
