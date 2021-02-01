using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpManager : MonoBehaviour
{
    public GameObject[] warpPoints = new GameObject[5];

    public GameObject player;
    
    public void WarpPlayerTo(string map)
    {
        if (map == "oldtown")
        {
            player.transform.position = warpPoints[0].transform.position;
        }
        else if(map == "")
        {

        }
    }
}
