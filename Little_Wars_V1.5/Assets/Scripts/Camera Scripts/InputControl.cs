using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControl
{
    private static float PanSpeed;
    private static readonly float ZoomSpeedTouch = 0.05f;
    private static readonly float ZoomSpeedMouse = 100f;

    private float[] BoundsX;
    private float[] BoundsZ;
    private float[] rBoundsX;
    //private float[] rBoundsY;
    private float[] ZoomBounds;

    private Camera cam;
    private Transform transform;

    private Vector3 startPos;
    private Quaternion startRot;

    private Vector3 lastPanPosition;
    private int panFingerId; // Touch mode only

    private bool wasZoomingLastFrame; // Touch mode only
    private Vector2[] lastZoomPositions; // Touch mode only


    public InputControl()
    {
        PanSpeed = -20.0f;
        BoundsX = new float[] { -5f, 56f };
        BoundsZ = new float[] { -63f, -5f };
        rBoundsX = new float[] { 0f, 95f };
      //  rBoundsY = new float[] { -90, 90f };
        ZoomBounds = new float[] { 15f, 60f };
        cam = null;
        transform = null;
    }

    public InputControl( float lb, float rb, float ub, float bb, float miz, Camera c, Transform t)
    {
        BoundsX = new float[] { lb, rb };
        BoundsZ = new float[] { ub, bb };
        ZoomBounds = new float[] { miz, 60f };
        cam = c;
        transform = t;
    }

    public void SetCnT(Camera c, Transform t)
    {
        Debug.Log(c);
        cam = c;
        transform = t;
        startPos = t.position;
        startRot = t.rotation;
    }

    public void Reverse()
    {
        PanSpeed *= -1;
    }

    public void HandleInput()
    {
        if (Input.touchSupported && Application.platform != RuntimePlatform.WebGLPlayer)
        {
            HandleTouch();
        }
        else
        {
            HandleMouse();
        }
    }

    void HandleTouch()
    {
        switch (Input.touchCount)
        {

            case 1: // Panning
                wasZoomingLastFrame = false;

                // If the touch began, capture its position and its finger ID.
                // Otherwise, if the finger ID of the touch doesn't match, skip it.
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    lastPanPosition = touch.position;
                    panFingerId = touch.fingerId;
                }
                else if (touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved)
                {
                    PanCamera(touch.position);
                }
                break;

            case 2: // Zooming
                Vector2[] newPositions = new Vector2[] { Input.GetTouch(0).position, Input.GetTouch(1).position };
                if (!wasZoomingLastFrame)
                {
                    lastZoomPositions = newPositions;
                    wasZoomingLastFrame = true;
                }
                else
                {
                    // Zoom based on the distance between the new positions compared to the 
                    // distance between the previous positions.
                    float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
                    float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
                    float offset = newDistance - oldDistance;

                    ZoomCamera(offset, ZoomSpeedTouch);

                    lastZoomPositions = newPositions;
                }
                break;

            default:
                wasZoomingLastFrame = false;
                break;
        }
    }

    void HandleMouse()
    {
        // On mouse down, capture it's position.
        // Otherwise, if the mouse is still down, pan the camera.
        if (Input.GetMouseButtonDown(0))
        {
            lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            PanCamera(Input.mousePosition);
        }

        // Check for scrolling to zoom the camera
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        ZoomCamera(scroll, ZoomSpeedMouse);

        //Reset Camera
        if(Input.GetKeyDown("backspace"))
        {
            transform.position = startPos;
            transform.rotation = startRot;
        }
    }

    void PanCamera(Vector3 newPanPosition)
    {
        // Determine how much to move the camera
        Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
        Vector3 move = new Vector3(offset.x * PanSpeed, 0, offset.y * PanSpeed);

        // Perform the movement
        transform.Translate(move, Space.World);

        // Ensure the camera remains within bounds.
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, BoundsX[0], BoundsX[1]);
        pos.z = Mathf.Clamp(transform.position.z, BoundsZ[0], BoundsZ[1]);
        transform.position = pos;

        // Cache the position
        lastPanPosition = newPanPosition;
    }

    void RollCamera(Vector3 newPanPosition)
    {
        // Determine how much to move the camera
        Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
        Quaternion rotate = new Quaternion();
        rotate.eulerAngles = new Vector3(offset.y * PanSpeed, offset.x * PanSpeed, 0);

        transform.Rotate(rotate.eulerAngles);

        // Ensure the camera remains within bounds.
        Quaternion rot = transform.rotation;
        rot.eulerAngles = new Vector3(Mathf.Clamp(transform.rotation.eulerAngles.x, rBoundsX[0], rBoundsX[1]), 0.0f, 0.0f);
        transform.rotation = rot;

        // Cache the position
        lastPanPosition = newPanPosition;
    }

    void ZoomCamera(float offset, float speed)
    {
        if (offset == 0)
        {
            return;
        }

        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - (offset * speed), ZoomBounds[0], ZoomBounds[1]);
    }

    public void ChangeTargets()
    {
        lastPanPosition = transform.position;
    }
}