using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstKill : MonoBehaviour {

    public bool firstKill = false; //Has this cannonball instakilled something yet? (Brennan)

    ParticleSystem ps;
    SphereCollider sc;
    MeshRenderer mr;
    Rigidbody rb;

    float counter = 0;

    void Start()
    {
        ps = gameObject.GetComponent<ParticleSystem>();
        sc = gameObject.GetComponent<SphereCollider>();
        rb = gameObject.GetComponent<Rigidbody>();
        mr = gameObject.GetComponent<MeshRenderer>();
        StartCoroutine("WhenToDie");
    }

    IEnumerator WhenToDie()
    {
        while (counter < 3)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        mr.enabled = false;
        sc.enabled = false;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.rotation = new Quaternion(-90, 0, 0, transform.rotation.w);
        ps.Play();
        SoundMaster.CannonballPop();
        Destroy(gameObject, 1.0f);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "HexCell" || col.gameObject.name == "Plane")
            if (counter < 2)
                counter = 2;

        if (col.gameObject.tag == "Infantry" && !col.gameObject.GetComponent<Infantry>().dying)
        {
            SoundMaster.CannonballHit();
        }

        if (col.gameObject.tag == "Cavalry" && !col.gameObject.GetComponent<Cavalry>().dying)
        {
            SoundMaster.CannonballHit();
        }
    }
}
