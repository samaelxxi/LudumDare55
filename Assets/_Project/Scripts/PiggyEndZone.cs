using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PiggyEndZone : MonoBehaviour
{
    [SerializeField] BoxCollider2D _collider;

    List<Vector3> _availablePositions = new();

    float _piggyDist = 1;

    void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        int maxPiggies = Game.Instance.PiggiesQuantity;
        float height = _collider.size.y;
        _piggyDist = (height-1) / (maxPiggies - 1);
        for (int i = 0; i < maxPiggies; i++)
        {
            float y = transform.position.y - height / 2 + i * _piggyDist + 0.5f;
            Vector3 pos = new(transform.position.x, y, y);
            _availablePositions.Add(pos);
        }
    }

    public Vector3 GetFreePosition()
    {
        if (_availablePositions.Count == 0)
            return transform.position;

        int idx = UnityEngine.Random.Range(0, _availablePositions.Count);
        Vector3 pos = _availablePositions[idx];
        _availablePositions.RemoveAt(idx);
        float dy = _piggyDist * 0.4f;
        float dx = _collider.size.x / 3;
        pos += new Vector3(UnityEngine.Random.Range(-dx, 0), 
                           UnityEngine.Random.Range(-dy, dy), 0);
        return pos;
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(transform.position, 0.1f);
    //     for (int i = 0; i <= 9; i++)
    //     {
    //         _piggyDist = (_collider.size.y-1) / (9 - 1);
    //         Vector3 pos = new(transform.position.x, 
    //                           transform.position.y - _collider.size.y / 2 + i * _piggyDist + 0.5f, 0);
    //         Gizmos.DrawWireSphere(pos, 0.1f);
    //     }
    // }
}
