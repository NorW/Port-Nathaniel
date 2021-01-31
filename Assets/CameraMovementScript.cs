using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementScript : MonoBehaviour
{
    public GameObject toFollow;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(toFollow.transform.position.x, toFollow.transform.position.y, -10);
    }
}
