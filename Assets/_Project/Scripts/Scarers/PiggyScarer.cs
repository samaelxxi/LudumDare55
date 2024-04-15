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

    protected float _lastAttackTime = 0;


    SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.transform.position = _spriteRenderer.transform.position.SetZ(transform.position.y - 0.5f);
    }

    void Update()
    {
        if (_piggies.Where(piggy => piggy.CanBeScared).Count() == 0) return;

        if (_lastAttackTime + _rechargeTime < Time.time)
        {
            Attack();
            
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
        foreach (var piggy in _piggies.Where(p => p.CanBeScared))
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
        foreach (var piggy in _piggies.Where(p => p.CanBeScared))
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

    protected void AttackAOE()
    {
        foreach (var piggy in _piggies.Where(p => p.CanBeScared))
        {
            piggy.ReceiveNegativeVibes(_damage);
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log("Triggered");
        if (other.TryGetComponent(out Piggy piggy))
        {
            if (!_piggies.Contains(piggy))
                _piggies.Add(piggy);
        }
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Piggy piggy))
        {
            _piggies.Remove(piggy);
            // Debug.LogError("Piggy left");
        }
    }
}
