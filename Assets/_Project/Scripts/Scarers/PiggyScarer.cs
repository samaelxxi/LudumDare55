using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class PiggyScarer : MonoBehaviour
{
    enum AttackType { Default, AOE, Ranged }


    [SerializeField] protected float _damage = 20;
    [SerializeField] float _rechargeTime = 1;
    [SerializeField] AttackType _attackType = AttackType.Default;
    [SerializeField] protected Animator _animator;


    protected List<Piggy> _piggies = new();

    float _lastAttackTime = 0;


    void Update()
    {
        if (_piggies.Where(piggy => !piggy.IsScared).Count() == 0) return;

        if (_lastAttackTime + _rechargeTime < Time.time)
        {
            Attack();
            _lastAttackTime = Time.time;
        }
    }

    protected virtual void Attack()
    {
        Debug.Log("Attacking");
        switch (_attackType)
        {
            case AttackType.Default:
                AttackDefault();
                break;
            case AttackType.AOE:
                AttackAOE();
                break;
            case AttackType.Ranged:
                AttackRange();
                break;
        }
    }

    void AttackRange()  // TODO remove
    {
        // attack closest piggy
        Piggy closestPiggy = null;
        float closestDistance = float.MaxValue;
        foreach (var piggy in _piggies.Where(p => !p.IsScared))
        {
            float distance = Vector3.Distance(transform.position, piggy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPiggy = piggy;
            }
        }
        closestPiggy.ReceiveNegativeVibes(_damage);
    }

    void AttackDefault()
    {
        // attack closest piggy
        Piggy closestPiggy = null;
        float closestDistance = float.MaxValue;
        foreach (var piggy in _piggies.Where(p => !p.IsScared))
        {
            float distance = Vector3.Distance(transform.position, piggy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPiggy = piggy;
            }
        }
        closestPiggy.ReceiveNegativeVibes(_damage);
    }

    void AttackAOE()
    {
        foreach (var piggy in _piggies.Where(p => !p.IsScared))
        {
            piggy.ReceiveNegativeVibes(_damage);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Piggy piggy))
        {
            _piggies.Add(piggy);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Piggy piggy))
        {
            _piggies.Remove(piggy);
        }
    }
}
