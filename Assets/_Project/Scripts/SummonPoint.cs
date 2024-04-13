using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SummonPoint : MonoBehaviour
{
    [SerializeField] Piggy _piggyPrefab;
    [SerializeField] float _summonCooldown = 1;
    [SerializeField] SpriteRenderer _chosenSprite;

    Queue<PiggyData> _summonQueue = new();

    float _lastSummonTime;

    void Awake()
    {
        _chosenSprite.enabled = false;
    }

    public void AddToSummonQueue(PiggyData data)
    {
        _summonQueue.Enqueue(data);
    }

    public void BeChosen()
    {
        _chosenSprite.enabled = true;
    }

    public void BeUnchosen()
    {
        _chosenSprite.enabled = false;
    }

    void Update()
    {
        if (Time.time - _lastSummonTime > _summonCooldown && _summonQueue.Count > 0)
        {
            _lastSummonTime = Time.time;
            SummonPiggy();
        }
    }

    void SummonPiggy()
    {
        if (_summonQueue.Count == 0)
            return;

        var data = _summonQueue.Dequeue();
        var piggy = Instantiate(_piggyPrefab, transform.position, Quaternion.identity);
        piggy.SetupPiggy(data);
    }
}
