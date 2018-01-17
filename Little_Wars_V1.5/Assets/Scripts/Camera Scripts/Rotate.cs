using UnityEngine;
using System.Collections;


/// <summary>
/// Script to Rotate Main Cam in the Title Screen
/// </summary>
public class Rotate : MonoBehaviour {
    public float speed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    /// <summary>
    /// main menu rotation
    /// </summary>
	void Update () {
        transform.Rotate(Vector3.up * Time.deltaTime* speed);
    }
}
