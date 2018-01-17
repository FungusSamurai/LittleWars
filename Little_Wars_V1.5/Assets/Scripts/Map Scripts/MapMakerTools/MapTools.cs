using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Abstrace class for all map making tools, using this structure will make creating, running, and editing said tools much less of a pain, and more simple to implement for the users when we get around to it.
public abstract class MapTools // See MapToolbox for more on how this is used.
{
    //needs of this abstract class and its methods can be expanded on as better understanding of what every tool needs is figured out
    protected Ray ray;
    protected RaycastHit rayHit;

    protected Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

    public abstract void Behavior(); // this of this as each tool's own custom update function.
}
