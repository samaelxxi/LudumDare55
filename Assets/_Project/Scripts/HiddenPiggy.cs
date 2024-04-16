using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HiddenPiggy : MonoBehaviour
{
    float _nextJumpTime;

    void Start()
    {
        _nextJumpTime = Random.Range(7, 14);
    }

    void Update()
    {
        _nextJumpTime -= Time.deltaTime;
        if (_nextJumpTime <= 0)
        {
            _nextJumpTime = Random.Range(7, 14);
            transform.DOJump(transform.position, 0.5f, 1, 0.5f);
        }
    }
}
