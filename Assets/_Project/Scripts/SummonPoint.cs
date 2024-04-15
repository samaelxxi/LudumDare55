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
        var collider = piggy.GetComponent<CircleCollider2D>();
        var renderer = piggy.GetComponentInChildren<SpriteRenderer>();
        float radius = collider.radius;
        collider.enabled = false;
        piggy.transform.DOScale(Vector3.one*1.5f, 0.5f).SetEase(Ease.OutBack).OnComplete(() => piggy.transform.DOShakeScale(0.5f, 0.5f, 5, 40));
        piggy.transform.DOShakeRotation(1, new Vector3(0, 0, Random.Range(30, 60) * (Random.value > 0.3f ? 1 : -1)), 
                1, randomnessMode:ShakeRandomnessMode.Harmonic)
                .OnComplete(delegate { collider.enabled = true; piggy.enabled = true; collider.radius = radius/1.5f;});

        piggy.transform.DOJump(piggy.transform.position, 1, 1, 1);
        Game.Instance.Level.AddHungryPiggy(piggy);
        Game.Instance.AudioManager.PlayRange(Oinks.GetOinks(data.Type, data.Rank), pitch: Random.Range(0.9f, 1.1f));
    }
}
