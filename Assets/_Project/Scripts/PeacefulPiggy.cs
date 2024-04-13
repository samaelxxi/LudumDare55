using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PeacefulPiggy : MonoBehaviour
{
    [SerializeField] PiggyData _data;
    [SerializeField] SpriteRenderer _spriteRenderer;


    public PiggyData Data => _data;
    BoxCollider2D _walkArea;


    float _waitTimer;
    bool _isWalking = false;

    public void SetData(PiggyData data)
    {
        _data = data;
    }

    public void SetWalkArea(BoxCollider2D walkArea)
    {
        _walkArea = walkArea;
    }

    void Start()
    {
        _waitTimer = Random.Range(3.0f, 8.0f);
    }

    void Update()
    {
        if (!_isWalking)
            _waitTimer -= Time.deltaTime;
        if (_waitTimer <= 0)
        {
            _waitTimer = Random.Range(10, 15.0f);
            MoveSomewhere();
        }
    }

    void MoveSomewhere()
    {
        Vector3 randomPos = default;
        int i = 0;
        while (i < 100)
        {
            randomPos = new Vector3(Random.Range(_walkArea.bounds.min.x, _walkArea.bounds.max.x),
                                Random.Range(_walkArea.bounds.min.y, _walkArea.bounds.max.y), 0);
            if (Physics2D.OverlapCircle(randomPos, 0.5f, LayerMask.GetMask("Piggy")) == null) break;
            i++;
        }
        var direction = randomPos - transform.position;
        float distance = Vector3.Distance(transform.position, randomPos);
        var newPos = transform.position + Mathf.Min(distance, 8) * direction.normalized;
        if (Physics2D.OverlapCircle(newPos, 0.5f, LayerMask.GetMask("Piggy")) != null)
            newPos = randomPos;
        transform.DOMove(newPos, distance / 3).OnComplete(() => _isWalking = false);
    }
}