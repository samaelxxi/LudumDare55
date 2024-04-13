using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using System;

[SelectionBase]
public class Piggy : MonoBehaviour
{
    [SerializeField] float _speed = 1.0f;
    [SerializeField] float _hp = 100.0f;

    RoadTile _previousRoadTile;
    RoadTile _currentRoadTile;

    public bool IsScared { get; private set; }
    List<RoadTile> _currentPath;

    Tweener _moveTween;

    void Start()
    {
        _currentRoadTile = Game.Instance.RoadManager.GetRoadTileAt(transform.position);
        MoveToCrops();
    }

    public void SetupPiggy(PiggyData data)
    {
        _speed = data.Speed;
        _hp = data.Health;
    }

    void MoveToCrops()
    {
        var nextTile = Game.Instance.RoadManager.GetNeighbour(_currentRoadTile, _currentRoadTile.Direction);
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
        if (IsScared)
            return;
        _hp -= damage;
        if (_hp <= 0)
            BeScared();
    }

    void BeScared()
    {
        IsScared = true;
        _currentPath = Game.Instance.RoadManager.GetPathToEscape(_currentRoadTile);
        _speed = 10;
        _currentPath.RemoveAt(0); // Remove the current tile from the path
        _moveTween.Kill();

        GoAlongPath();
    }

    void RunAway()
    {
        Destroy(gameObject);
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
