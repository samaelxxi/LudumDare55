using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSetZ : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        transform.localPosition = transform.localPosition.SetZ(transform.localPosition.y - GetComponent<SpriteRenderer>().bounds.size.y / 2);
    }
}
