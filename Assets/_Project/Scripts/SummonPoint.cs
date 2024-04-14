using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class SummonPoint : MonoBehaviour
{
    [SerializeField] Piggy _piggyPrefab;
    [SerializeField] float _summonCooldown = 1;
    [SerializeField] Animator _animator;

    Queue<PiggyData> _summonQueue = new();

    float _lastSummonTime;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void AddToSummonQueue(PiggyData data)
    {
        _summonQueue.Enqueue(data);
    }

    public void BeChosen()
    {
        // Debug.Log($"Summon point {name} is chosen");
        _animator.SetBool("IsActive", true);
    }

    public void BeUnchosen()
    {
        // Debug.Log($"Summon point {name} is unchosen");
        _animator.SetBool("IsActive", false);
    }

    void Update()
    {
        if (_summonQueue.Count > 0 && Time.time - _lastSummonTime > _summonCooldown)
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
        piggy.enabled = false;
        piggy.SetupPiggy(data);
        piggy.transform.localScale = Vector3.zero;
        piggy.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).OnComplete(() => piggy.transform.DOShakeScale(0.5f, 0.5f, 5, 40));
        piggy.transform.DOShakeRotation(1, new Vector3(0, 0, Random.Range(30, 60) * (Random.value > 0.3f ? 1 : -1)), 
                1, randomnessMode:ShakeRandomnessMode.Harmonic).OnComplete(() => piggy.enabled = true);
        piggy.transform.DOJump(piggy.transform.position, 1, 1, 1);
        Game.Instance.Level.AddHungryPiggy(piggy);
    }
}
