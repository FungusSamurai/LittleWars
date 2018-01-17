using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagePlaneTest : MonoBehaviour
{
    public void SetUnitPicture(string name, int player)
    {
        string path = "Pictures/Materials/";
        string prefix = "Deploy";
        string p = player.ToString();
        Material m;

        if (name == "Cannon")
        {
            p = "";
        }

        m = (Material)Resources.Load(path + prefix + name + p);

        if (m != null)
        {
            gameObject.GetComponent<Renderer>().material = m;
        }
    }
}