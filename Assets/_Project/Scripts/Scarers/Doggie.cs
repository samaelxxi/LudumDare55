using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doggie : PiggyScarer
{
    [SerializeField] GameObject _barkVFX;
    [SerializeField] Animator _barkAnimator;

    Vector3 _direction;

    protected override void Attack()
    {
        _animator.SetTrigger("Bark");
        this.InSeconds(4.0f/6.0f, delegate
        {
            // _barkVFX.SetActive(true);
            // _barkAnimator.SetTrigger("Bark");
            // _barkVFX.transform.position = transform.position + RoadTile.Directions[dir].ToVector3();
            // target.ReceiveNegativeVibes(_damage);
            AttackAOE();
        });
    }
}
