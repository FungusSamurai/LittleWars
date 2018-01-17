using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectDriver : MonoBehaviour
{
    public static bool mapChosen;

    Button current;

    public Button[] maps;

    public void Start()
    {
        current = null;
        mapChosen = false;
    }

    public void SetChosenMap(string map)
    {
        Debug.Log(UnitDistributionMaster.correctDist);
        if(UnitDistributionMaster.correctDist)
            GameObject.Find("Start").GetComponent<Button>().interactable = true;
        GameBrain.mapName = map;
    }

    public void SetButton(int i)
    {
        if (current != null)
        {
            current.interactable = true;

            current = maps[i];

            current.interactable = false;
        }
        else
        {
            current = maps[i];
            current.interactable = false;
            mapChosen = true;
        }
    }

}
