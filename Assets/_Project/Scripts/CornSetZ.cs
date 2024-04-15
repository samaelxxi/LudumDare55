using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornSetZ : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.transform.position = sprite.transform.position.SetZ(transform.position.y);
    }
}
