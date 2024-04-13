using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class Goosey : PiggyScarer
{

    Vector3 _direction;

    protected override void Attack()
    {
        var targets = _piggies.Where(piggy => !piggy.IsScared);
        Piggy target = targets.ElementAt(Random.Range(0, targets.Count()));
        Vector3 direction = target.transform.position - transform.position;
        _direction = direction;
        bool isVertical = Mathf.Abs(direction.x) < Mathf.Abs(direction.y);
        if (isVertical)
        {
            if (direction.y > 0)
                _animator.SetTrigger("Top");
            else
                _animator.SetTrigger("Down");
        }
        else
        {
            if (direction.x > 0)
                _animator.SetTrigger("Left");
            else
                _animator.SetTrigger("Right");
        }
        this.InSeconds(4.0f/12.0f, () => target.ReceiveNegativeVibes(_damage));
    }
}
