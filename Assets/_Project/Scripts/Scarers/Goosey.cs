using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CarterGames.Assets.AudioManager;
using UnityEngine;
using CarterGames;

[SelectionBase]
public class Goosey : PiggyScarer
{
    const float AttackTime = 4.0f / 12.0f;

    [SerializeField] GameObject _slamVFX;
    [SerializeField] Animator _slamAnimator;

    Vector3 _direction;


    RoadTile _pred;
    Vector3 _predPos;
    Vector3 _targetPos;


    void OnDrawGizmos()
    {
        if (_pred != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_pred.transform.position, 0.5f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_predPos, 0.2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(_targetPos, 0.2f);
        }
    }

    protected override void Attack()
    {
        // Debug.Log("Triggered attack");

        var roadManager = Game.Instance.RoadManager;

        var targets = _piggies.Where(piggy => piggy.CanBeScared);

        // pick closest piggy
        Piggy target = targets.OrderBy(p => Vector3.Distance(transform.position, p.transform.position)).FirstOrDefault();

        // Piggy target = targets.ElementAt(Random.Range(0, targets.Count()));
        RoadTile targetTile = roadManager.GetRoadTileAt(target.transform.position);

        float ds = target.Speed * AttackTime;
        var predictedPos = target.transform.position + RoadTile.Directions[targetTile.Direction].ToVector3() * ds;
        _targetPos = target.transform.position;
        _predPos = predictedPos;
        var predictedTile = roadManager.GetRoadTileAt(predictedPos);
        _pred = predictedTile;
        // Debug.Log($"{predictedPos} {target.transform.position} {ds} {RoadTile.Directions[targetTile.Direction].ToVector3()} {predictedTile} {targetTile.Direction} {targetTile}");

        if (!IsTileMyNeighbour(predictedTile))
            return;

        Vector3 direction = predictedTile.transform.position - transform.position;
        _direction = direction;
        bool isVertical = Mathf.Abs(direction.x) < Mathf.Abs(direction.y);
        RoadTile.RoadDirection dir = RoadTile.RoadDirection.Up;
        if (isVertical)
        {
            if (direction.y > 0)
                dir = RoadTile.RoadDirection.Up;
            else
                dir = RoadTile.RoadDirection.Down;
        }
        else
        {
            if (direction.x > 0)
                dir = RoadTile.RoadDirection.Right;
            
            else
                dir = RoadTile.RoadDirection.Left;
        }
        switch (dir)
        {
            case RoadTile.RoadDirection.Up:
                _animator.SetTrigger("Top");
                break;
            case RoadTile.RoadDirection.Right:
                _animator.SetTrigger("Right");
                break;
            case RoadTile.RoadDirection.Down:
                _animator.SetTrigger("Down");
                break;
            case RoadTile.RoadDirection.Left:
                 _animator.SetTrigger("Left");
                 break;
        };


        Game.Instance.AudioManager.Play("Quack1", pitch: Random.Range(0.9f, 1.1f));
        this.InSeconds(AttackTime, delegate
        {
            _slamVFX.SetActive(true);
            _slamAnimator.Play("Slam", 0, 0);
            _slamVFX.transform.position = predictedTile.transform.position;
            
            DamageAllOnTile(predictedTile);
        });
        _lastAttackTime = Time.time;
    }

    bool IsTileMyNeighbour(RoadTile tile)
    {
        var roadManager = Game.Instance.RoadManager;
        foreach (var dir in RoadTile.Directions.Keys)
        {
            var neigh = roadManager.GetNeighbour(transform.position, dir);
            if (neigh == null)
                continue;
            if (neigh == tile)
                return true;
        }
        return false;
    }

    void DamageAllOnTile(RoadTile tile)
    {
        if (tile == null)
        {
            Debug.LogError($"No road tile found for attack on ");
            return;
        }
        foreach (var piggy in _piggies.Where(p => p.CanBeScared))
            if (Game.Instance.RoadManager.GetRoadTileAt(piggy.transform.position) == tile)
                piggy.ReceiveNegativeVibes(_damage);
    }
}
