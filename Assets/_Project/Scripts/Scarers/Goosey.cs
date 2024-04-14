using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class Goosey : PiggyScarer
{
    [SerializeField] GameObject _slamVFX;
    [SerializeField] Animator _slamAnimator;

    Vector3 _direction;

    protected override void Attack()
    {
        var targets = _piggies.Where(piggy => !piggy.IsScared);
        Piggy target = targets.ElementAt(Random.Range(0, targets.Count()));
        Vector3 direction = target.transform.position - transform.position;
        _direction = direction;
        bool isVertical = Mathf.Abs(direction.x) < Mathf.Abs(direction.y);
        RoadTile.RoadDirection dir = RoadTile.RoadDirection.Up;
        if (isVertical)
        {
            if (direction.y > 0)
            {
                _animator.SetTrigger("Top");
                dir = RoadTile.RoadDirection.Up;
            }
            else
            {
                _animator.SetTrigger("Down");
                dir = RoadTile.RoadDirection.Down;
            }
        }
        else
        {
            if (direction.x > 0)
            {
                _animator.SetTrigger("Right");
                dir = RoadTile.RoadDirection.Right;
            }
            else
            {
                _animator.SetTrigger("Left");
                dir = RoadTile.RoadDirection.Left;
            }
        }
        this.InSeconds(4.0f/12.0f, delegate
        {
            _slamVFX.SetActive(true);
            _slamAnimator.SetTrigger("Slam");
            _slamVFX.transform.position = transform.position + RoadTile.Directions[dir].ToVector3()*4;
            DamageAllOnTile(dir);
        });
    }

    void DamageAllOnTile(RoadTile.RoadDirection dir)
    {
        var roadManager = Game.Instance.RoadManager;
        var roadTile = roadManager.GetNeighbour(transform.position, dir);
        if (roadTile == null)
        {
            Debug.LogError($"No road tile found for attack on {dir}");
            return;
        }
        foreach (var piggy in _piggies.Where(p => !p.IsScared))
            if (roadTile == piggy.CurrentRoadTile)
                piggy.ReceiveNegativeVibes(_damage);
    }
}
