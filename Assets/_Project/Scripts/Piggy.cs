using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using System;
using Random = UnityEngine.Random;

[SelectionBase]
public class Piggy : MonoBehaviour
{
    [SerializeField] float _speed = 1.0f;
    [SerializeField] float _hp = 100.0f;
    [SerializeField] int _foodCapacity = 20;

    [SerializeField] SpriteRenderer _spriteRenderer;

    RoadTile _previousRoadTile;
    RoadTile _currentRoadTile;

    public RoadTile CurrentRoadTile => _currentRoadTile;

    public bool CanBeScared => !_isScared && !_isEating;
    public bool IsGotSomeFood => _isEating;
    public bool IsFinishedHarvesting => _isEating || _isScared;

    List<RoadTile> _currentPath;

    Tweener _moveTween;

    bool _isScared = false;
    bool _isEating = false;

    void Start()
    {
        _currentRoadTile = Game.Instance.RoadManager.GetRoadTileAt(transform.position);
        MoveToCrops();
    }

    public void SetupPiggy(PiggyData data)
    {
        _speed = data.Speed;
        _hp = data.Health;
        _foodCapacity = data.FoodCapacity;
    }

    void MoveToCrops()
    {
        var nextTile = Game.Instance.RoadManager.GetNeighbour(_currentRoadTile, _currentRoadTile.Direction);
        if (nextTile == null)
        {
            Debug.Log("Piggy can't find the next tile :(");
            return;
        }
        nextTile.VisitTile();
        _previousRoadTile = _currentRoadTile;
        _currentRoadTile = nextTile;
        GoToTile(nextTile, MoveToCrops);
    }

    void GoAlongPath()
    {
        if (_currentPath.Count == 0)
            Debug.LogError("Piggy doesn't know what to do next ;(");

        _previousRoadTile = _currentRoadTile;
        if (_previousRoadTile.IsEscapeTile)
        {
            RunAway();
            return;
        }
        _currentRoadTile = _currentPath[0];
        _currentPath.RemoveAt(0);

        GoToTile(_currentRoadTile, GoAlongPath);
    }

    void GoToTile(RoadTile tile, Action onComplete = null)
    {
        float speed = _speed * _currentRoadTile.SpeedModifier;
        float dist = Vector3.Distance(_previousRoadTile.transform.position, _currentRoadTile.transform.position);
        float time = dist / speed;
        
        _moveTween = transform.DOMove(_currentRoadTile.transform.position, time).SetEase(Ease.Linear);
        if (onComplete != null)
            _moveTween.OnComplete(() => onComplete());
    }

    public void ReceiveNegativeVibes(float damage)
    {
        if (!CanBeScared)
            return;
        _hp -= damage;
        if (_hp <= 0)
            BeScared();
    }

    void BeScared()
    {
        _isScared = true;
        _currentPath = Game.Instance.RoadManager.GetPathToEscape(_currentRoadTile);
        _speed = 8;
        _currentPath.RemoveAt(0); // Remove the current tile from the path
        _moveTween.Kill();
        _spriteRenderer.flipX = false;

        GoAlongPath();
    }

    void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.TryGetComponent(out PiggyEndZone endZone))
        {
            _moveTween.Kill();
            var newPos = endZone.GetFreePosition();
            _isEating = true;
            transform.DOMove(newPos, Vector3.Distance(transform.position, newPos) / _speed)
                .SetEase(Ease.Linear).OnComplete(() => StartCoroutine(StartEating()));
        }
    }

    IEnumerator StartEating()
    {
        Game.Instance.PiggyGotSomeFood(_foodCapacity);
        Debug.Log("Start eating");
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1.0f, 5.0f));
            if (Random.value > 0.5f)
                _spriteRenderer.flipX = !_spriteRenderer.flipX;
            else
                transform.DOLocalJump(transform.position, 0.5f, 1, 0.5f).SetEase(Ease.Linear);
        }
    }

    void RunAway()
    {
        transform.DOMove(transform.position - Vector3.left * 2, 1).OnComplete(delegate
        {
            Destroy(gameObject);
        });
    }

    void OnDrawGizmosSelected()
    {
        if (_currentPath == null || _currentPath.Count == 0)
        {
            if (_currentRoadTile == null)
                return;
            GizmosExtensions.DrawArrow(transform.position, _currentRoadTile.DirectionVector, 2.0f, 1);
            return;
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < _currentPath.Count - 1; i++)
            Handles.Label(_currentPath[i].transform.position, i.ToString());
    }
}
