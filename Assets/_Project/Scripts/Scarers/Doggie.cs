using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Doggie : PiggyScarer
{
    [SerializeField] GameObject _barkVFX;
    [SerializeField] Animator _barkAnimator;


    protected override void Attack()
    {
        _lastAttackTime = Time.time;
        _animator.SetTrigger("Bark");
        this.InSeconds(4.0f/6.0f, delegate
        {
            _barkVFX.SetActive(true);
            _barkAnimator.Play("Bark", 0,0);
            AttackAOE();
            Game.Instance.AudioManager.Play("Bark1", pitch: Random.Range(0.9f, 1.1f));
        });
    }
}
