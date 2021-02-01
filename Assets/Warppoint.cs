using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warppoint : MonoBehaviour
{
    public Vector3 center;
    public float height, width;
    public string mapName;

    public void Start()
    {
        var sprite = GetComponentInParent<SpriteRenderer>();
        height = sprite.sprite.rect.height;
        width = sprite.sprite.rect.width;
        center = transform.parent.transform.position;
    }
}
