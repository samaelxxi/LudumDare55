using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class BWOI : PiggyScarer
{
    [SerializeField] GameObject _stonePrefab;
    [SerializeField] Transform _stoneSpawnPoint;
    [SerializeField] SpriteRenderer _spriteRenderer;

    Piggy _chosenPiggy;

    protected override void Attack()
    {
        ChoosePiggy();
    
        if (_chosenPiggy == null)
            return;

        // flip to face the chosen piggy
        if (_chosenPiggy.transform.position.x > transform.position.x)
            _spriteRenderer.flipX = true;
        else
            _spriteRenderer.flipX = false;

        _animator.SetTrigger("Throw");
        this.InSeconds(4.0f/6.0f, delegate
        {
            var stone = Instantiate(_stonePrefab, _stoneSpawnPoint.position, Quaternion.identity).GetComponent<BWOIStone>();
            stone.SetTarget(_chosenPiggy, _damage);
        });
    }

    void ChoosePiggy()
    {
        if (!_piggies.Contains(_chosenPiggy) || _chosenPiggy.IsScared)
            _chosenPiggy = null;

        if (_chosenPiggy != null)
            return;

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
        _chosenPiggy = closestPiggy;
    }
}
