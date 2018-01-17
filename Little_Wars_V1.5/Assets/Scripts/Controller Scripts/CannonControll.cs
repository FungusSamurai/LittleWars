using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CannonControll : MonoBehaviour
{
    public Vector3 targetPos;
    public Quaternion targetRot;

    public GameObject CannonballPrefab, ShotSpawn, CannonCam;
    public GameObject[] CameraPos = new GameObject[2];
    public float turnSpeed, cannon_power, percent;

    public bool CanShoot;

    int cI, buffer;

    public float time, Dx, Dy, Vx, Vy, height, actual;

    bool left = false;
    bool right = false;
    bool up = false;
    bool down = false;
    bool overLeft;
    public bool changed, selected = false;
    bool canControl;

    GameObject IndicatorP;
    GameObject Indicator;
    GameObject cannonball;
    List<GameObject> indicators;

    Vector3 turnLeft, turnRight, turnUp, turnDownForWhat, defaultCameraPos;

    //public Text cPowerDisplay;
    private Cannon c;

    Transform leftwheel, rightwheel;

    private void Awake()
    {
        canControl = true;
        CannonCam.GetComponent<Camera>().enabled = false;
        c = gameObject.GetComponent<Cannon>();

        CannonCam.transform.position = CameraPos[cI].transform.position;
        CannonCam.transform.rotation = CameraPos[cI].transform.rotation;
        targetPos = CannonCam.transform.position;
        targetRot = CannonCam.transform.rotation;

        CanShoot = true;
    }

    // Use this for initialization
    void Start()
    {
        IndicatorP = (GameObject)Resources.Load("prefabs/Indicator");
        Indicator = null;
        indicators = new List<GameObject>();
        c = gameObject.GetComponentInParent<Cannon>();
        
        turnUp = new Vector3(-turnSpeed, 0, 0);
        turnDownForWhat = new Vector3(turnSpeed, 0, 0);
        turnLeft = new Vector3(0, -turnSpeed, 0);
        turnRight = new Vector3(0, turnSpeed, 0);
        changed = false;
        //LandingZone.transform.position = new Vector3(transform.position.x, transform.position.y - 1.25f, transform.position.z);
        //cPowerDisplay = FindObjectOfType<Text>();

        //Going to use this later for the camera buffer - Brennan
        defaultCameraPos = new Vector3(transform.localPosition.x - 1.0f, transform.localPosition.y + 20f, transform.localPosition.z);
        percent = .5f;
        overLeft = true; //Currently looking over the left side
        cI = 0;
        buffer = 20;
 //       sinceLastShot = 5.0f; //Just a value above .5f so that the first shot will always work
        leftwheel = transform.parent.transform.GetChild(1);
        rightwheel = transform.parent.transform.GetChild(2);

        height = 10.0f;
        time = 0.2f;
    }

    void FixedUpdate()
    {
        if (selected == true)
        {
            Move();
            if (changed)
                Aim();

            ShoulderSwap();
            TrackCannon();
            CannonCamFollow();
  //          if (sinceLastShot < .5f)
 //               sinceLastShot += Time.deltaTime; //Adds how much time has passed in seconds to sinceLastShot (Brennan)
        }

        else if (!selected)
        {
          //  cPowerDisplay.text = " ";
            targetPos = CameraPos[cI].transform.position;
            targetRot = CameraPos[cI].transform.rotation;
            CannonCam.transform.position = CameraPos[cI].transform.position;
            CannonCam.transform.rotation = CameraPos[cI].transform.rotation;
        }

        //Debug.Log(CameraPos[cI].transform.position + "ping");
        //Debug.Log(CannonCam.transform.position + "pong");
    }

    void Move()
    {
        if (canControl)
        {
            UpdatePosistion();
        }
    }

    void UpdatePosistion()
    {
        if (up == true && transform.localEulerAngles.x > 15.0f)
        {
            transform.localEulerAngles = new Vector3((transform.localEulerAngles.x - 1), transform.localEulerAngles.y, transform.localEulerAngles.z);
            changed = true;

        }
        else if (down == true && transform.localEulerAngles.x < 88.0f)
        {
            transform.localEulerAngles = new Vector3((transform.localEulerAngles.x + 1), transform.localEulerAngles.y, transform.localEulerAngles.z);
            changed = true;
        }
        else if (left == true) //turns the parent to the side and the cannon moves with it.
        {
            transform.parent.transform.localEulerAngles += turnLeft;
            leftwheel.Rotate(Vector3.down * -2f);
            rightwheel.Rotate(Vector3.up * 2f);
            //leftwheel.localRotation = new Quaternion(leftwheel.rotation.x + 5, leftwheel.rotation.y, leftwheel.rotation.z, leftwheel.rotation.z);
            changed = true;
        }
        else if (right == true)
        {
            transform.parent.transform.localEulerAngles += turnRight;
            leftwheel.Rotate(Vector3.down * 2f);
            rightwheel.Rotate(Vector3.up * -2f);
            changed = true;
        }
    }


    public void lefts()
    {
        left = !left;
        if (left)
            SoundMaster.TurnCannonOn();
        else if (!left)
            SoundMaster.TurnCannonOff();
    }
    public void rights()
    {
        right = !right;
        if (right)
            SoundMaster.TurnCannonOn();
        else if (!right)
            SoundMaster.TurnCannonOff();
    }
    public void ups()
    {
        up = !up;
        if (up)
            SoundMaster.TurnCannonOn();
        else if (!up)
            SoundMaster.TurnCannonOff();
    }
    public void downs()
    {
        down = !down;
        if (down)
            SoundMaster.TurnCannonOn();
        else if (!down)
            SoundMaster.TurnCannonOff();
    }

    public void firecannon()
    {
        actual = cannon_power * percent;

        DestroyIndicator();

        if (c.Shots == Cannon.Clip)
        {
            MoveMaster.UnitsToMove--;
            c.Moved = true;
        }
        if (selected == true && c.Shots > 0)
        {
            if (CanShoot) //Half a second has to have passed before the next shot can fire (Brennan)
            {
                //Destroy code here

                cannonball = (GameObject)Instantiate(CannonballPrefab, ShotSpawn.transform.position, ShotSpawn.transform.rotation);
                cannonball.GetComponent<Rigidbody>().AddForce(ShotSpawn.transform.forward * actual);
                CanShoot = false;
                c.Shots--;

                ParticleSystem ps = transform.GetChild(1).gameObject.GetComponent<ParticleSystem>();
                    //shader = Shader.Find("Mobile/Particles/Multiply");
                ps.Play();
                SoundMaster.FireCannon();
                StartCoroutine(ShakeCannon());
            }
        }
    }


    IEnumerator ShakeCannon()
    {
        canControl = false;

        Vector3 op = gameObject.transform.localPosition;
        Quaternion or = gameObject.transform.rotation;
        Vector3 tp = gameObject.transform.localPosition - Vector3.Normalize(new Vector3(Mathf.Sin(gameObject.transform.rotation.x * Mathf.PI / 180.0f), 0.0f, Mathf.Cos(gameObject.transform.rotation.x * Mathf.PI / 180.0f))) *0.3f;
        Quaternion tr =  Quaternion.Euler(gameObject.transform.rotation.eulerAngles.x - 10.0f, gameObject.transform.rotation.eulerAngles.y, gameObject.transform.rotation.eulerAngles.z);

        Quaternion olwr = leftwheel.transform.rotation;
        Quaternion orwr = rightwheel.transform.rotation;

        Vector3 olwp = leftwheel.transform.localPosition;
        Vector3 orwp = rightwheel.transform.localPosition;
        Vector3 lwp = leftwheel.transform.localPosition - Vector3.Normalize(new Vector3(Mathf.Sin(leftwheel.transform.rotation.x * Mathf.PI / 180.0f), 0.0f, Mathf.Cos(leftwheel.transform.rotation.x * Mathf.PI / 180.0f))) * 0.1f;
        Vector3 rwp = rightwheel.transform.localPosition - Vector3.Normalize(new Vector3(Mathf.Sin(rightwheel.transform.rotation.x * Mathf.PI / 180.0f), 0.0f, Mathf.Cos(rightwheel.transform.rotation.x * Mathf.PI / 180.0f))) * 0.1f;

        Quaternion lwr = Quaternion.Euler(leftwheel.transform.rotation.eulerAngles.x - 15.0f, leftwheel.transform.rotation.eulerAngles.y, leftwheel.transform.rotation.eulerAngles.z);
        Quaternion rwr = Quaternion.Euler(rightwheel.transform.rotation.eulerAngles.x - 15.0f, rightwheel.transform.rotation.eulerAngles.y, rightwheel.transform.rotation.eulerAngles.z);

        float t = 10.0f;

        while (Vector3.Distance(tp, gameObject.transform.localPosition) >= 0.001f)
        {
            float dt = t * Time.deltaTime;
            gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, tp, dt);
            gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, tr, dt);

            leftwheel.transform.rotation = Quaternion.Lerp(leftwheel.transform.rotation, lwr, dt);
            rightwheel.transform.rotation = Quaternion.Lerp(rightwheel.transform.rotation, rwr, dt);

            leftwheel.transform.localPosition = Vector3.Lerp(leftwheel.transform.localPosition, lwp, dt);
            rightwheel.transform.localPosition = Vector3.Lerp(rightwheel.transform.localPosition, rwp, dt);
            yield return null;
        }

        while (Vector3.Distance(op, gameObject.transform.localPosition) >= 0.001f)
        {
            float dt = t * Time.deltaTime;
            gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, op, dt);
            gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, or, dt);

            leftwheel.transform.rotation = Quaternion.Lerp(leftwheel.transform.rotation, olwr, dt);
            rightwheel.transform.rotation = Quaternion.Lerp(rightwheel.transform.rotation, orwr, dt);

            leftwheel.transform.localPosition = Vector3.Lerp(leftwheel.transform.localPosition, olwp, dt);
            rightwheel.transform.localPosition = Vector3.Lerp(rightwheel.transform.localPosition, orwp, dt);
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        CanShoot = true;
        canControl = true;
    }


    void Aim() //Creates an invisible cannonball that leaves a trail behind it. You can change the color of the trail through it's material TrialMat
    {
        actual = cannon_power * percent;
        //Debug.Log("Actual is " + actual);
        DestroyIndicator(); //Destroy previous indicator if it exist
        Vx = Time.fixedDeltaTime * actual/3 * Mathf.Sin(transform.eulerAngles.x * Mathf.Deg2Rad); //Forward velocity
        Vy = Time.fixedDeltaTime * actual/3 * Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad); //Upward velocity
        while (height > 1) //height of indicators. The while loop stops when one is made below 0 (which is roughly where the ground is)
        {
            //time starts as 0.2f
            Dx = Vx * time; //How far the ball will move forward after traveling for "time" amount of time
            Dy = (Vy * time) + (0.5f * -9.81f * time * time); //Same but for the upwards direction
            Indicator = Instantiate(IndicatorP, ShotSpawn.transform.position + (transform.parent.forward * Dx) + (transform.parent.up * Dy), IndicatorP.transform.rotation);
            height = Indicator.transform.position.y;
            indicators.Add(Indicator); //Adds the indicator just made to a list so they can later be deleted
            time += 0.2f;
        }
        //Sets values back to default values
        time = 0.2f;
        height = 10.0f;
        changed = false;
    }

    public void SetAim(float angle, float per)
    {
        transform.localEulerAngles = new Vector3(angle, 0, 0);
        percent = per *0.01f;
        changed = true;
    }

    public float Powerup()
    {
        if (percent < 1)
        {
            percent += .1f;
            changed = true;
        }
        return percent;
    }

    public float powershow()
    {
        changed = true;
        return percent;
    }

    public float powerdown()
    {
        if (percent > 0.3)
        {
            percent -= .1f;
            changed = true;
        }
        return percent;

    }

    void ShoulderSwap() //I know how much you guys like putting things in functions so here you go. This does the shoulder swap when you press shift (Brennan)
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            overLeft = !overLeft;

            if (overLeft)
                cI = 0;
            else
                cI = 1;

            setTargets();
        }
    }

    public void ReturnToDefault()
    {
        gameObject.transform.localEulerAngles = new Vector3(75.0f, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    void TrackCannon()
    {
        float angle = transform.eulerAngles.y - CannonCam.transform.eulerAngles.y; //The difference between the direction the camera and the cannon is facing
        if (angle < 0)
            angle += 360; //Negatives make things annoying

        //The angle doesn't become negative (it goes to down from 360 if you subtract from 0) so I added angle < buffer + 10 to keep things within a boundary
        if (right == true && angle >= buffer && angle < buffer + 10)
        {
            setTargets();
        }

        else if (left == true && angle <= 360 - buffer && angle > 360 - buffer - 10)
        {
            setTargets();
        }
    }

    private void CannonCamFollow()
    {
        CannonCam.transform.position = Vector3.Lerp(CannonCam.transform.position, targetPos, .05f);
        CannonCam.transform.rotation = Quaternion.Lerp(CannonCam.transform.rotation, targetRot, .05f);
    }

    public void DestroyIndicator()
    {
        if (indicators.Count != 0)
            foreach (GameObject g in indicators.ToArray()) //Created an enumeration problem without the ToArray part
            {
                indicators.Remove(g);
                Destroy(g);
            }
    }

    public int RayCastEnemies(out RaycastHit h) //Tells if there is an object along the predicted shot for the cannon
    {                                          //Returns which player own's the object found (-1 if neither) and the object that was hit
        RaycastHit hit;
        for(int i = 0; i < indicators.Count - 1; ++i)
        {
            
            Vector3 dir = indicators[i+1].transform.position - indicators[i].transform.position;
            float dist = Vector3.Distance(indicators[i+1].transform.position, indicators[i].transform.position);
            Physics.Raycast(indicators[i].transform.position, dir, out hit, dist);
            Debug.DrawRay(indicators[i].transform.position, dir, Color.green, 1);
            if (hit.transform != null && (hit.transform.tag == "Cavalry" || hit.transform.tag == "Infantry"))
            {
                h = hit;
                return hit.transform.gameObject.GetComponent<Unit>().Player;
            }
            
        } //Needs to start raycast from cannon

        h = new RaycastHit(); //Default value incase for loop did not find an enemy
        return -1;
    }
    private void setTargets()
    {
        targetPos = CameraPos[cI].transform.position;
        targetRot = CameraPos[cI].transform.rotation;
    }

    public bool Selected
    {
        get { return selected; }
        set { selected = value; }
    }

    public Vector3 DefaultCameraPos
    {
        get { return defaultCameraPos; }
        set { defaultCameraPos = value; }
    }

    public Cannon C
    {
        get { return c; }
    }
}
