using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using System;
using Random = UnityEngine.Random;
using TMPro;


[SelectionBase]
public class Piggy : MonoBehaviour
{
    [SerializeField] float _speed = 1.0f;
    [SerializeField] float _hp = 100.0f;
    [SerializeField] int _foodCapacity = 20;

    [SerializeField] float _spriteYOffset = 0.2f;

    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] AnimationCurve _jumpCurve;
    [SerializeField] float _smoothTime = 0.3f;
    [SerializeField] Material _flashMaterial;

    [SerializeField] TMP_Text _foodText;

    public PiggyData Data { get; private set; }
    public float Speed => _speed;

    RoadTile _previousRoadTile;
    RoadTile _currentRoadTile;

    public RoadTile CurrentRoadTile => _currentRoadTile;

    public bool CanBeScared => !_isScared && !_isEating;
    public bool IsGotSomeFood => _isEating;
    public bool IsFinishedHarvesting => _isEating || _isScared;
    public string Name => _name;

    List<RoadTile> _currentPath;

    Tweener _moveTween;

    bool _isScared = false;
    bool _isEating = false;
    bool _shouldJump = false;
    float _jumpTime = 0;
    string _name;

    float _barkSpeedModifier = 1.0f;

    void Start()
    {
        _currentRoadTile = Game.Instance.RoadManager.GetRoadTileAt(transform.position);
        MoveToCrops();
        _shouldJump = true;
    }


    void Update()
    {
        if (_shouldJump)
        {
            _jumpTime += Time.deltaTime;
            float jumpLength = 1 / _speed;
            float jumpHeight = Mathf.Max(jumpLength - 0.1f, 0.2f);
            float y = _jumpCurve.Evaluate(_jumpTime / jumpLength);
            var newPos = _spriteRenderer.transform.localPosition.SetY(y * jumpHeight + _spriteYOffset);
            _spriteRenderer.transform.localPosition = Vector3.Lerp(_spriteRenderer.transform.localPosition, newPos, _smoothTime);
            _spriteRenderer.transform.position = _spriteRenderer.transform.position.SetZ(transform.position.y);
            if (_jumpTime >= jumpLength)
                _jumpTime = 0;
        }
    }

    public void SetupPiggy(PiggyData data)
    {
        _speed = data.Speed;
        _hp = data.Health;
        _foodCapacity = data.FoodCapacity;
        _spriteRenderer.sprite = data.Sprite;

        Data = data;
        _name = data.Name;
    }

    public void GetBarked()
    {
        _barkSpeedModifier = 0.8f;
    }

    public void GetUnbarked()
    {
        _barkSpeedModifier = 1.0f;
    }

    void MoveToCrops()
    {
        var nextTile = Game.Instance.RoadManager.GetNeighbourTile(_currentRoadTile, _currentRoadTile.Direction);
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
        float speed = _speed * _currentRoadTile.SpeedModifier * _barkSpeedModifier;
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
        StartCoroutine(Flash());
        if (_hp <= 0)
            BeScared();
    }

    bool _isFlashing = false;
    IEnumerator Flash()
    {
        if (_isFlashing)
            yield break;
        _isFlashing = true;
        Material _oldMaterial = _spriteRenderer.material;
        _spriteRenderer.material = _flashMaterial;
        yield return new WaitForSeconds(0.1f);
        _spriteRenderer.material = _oldMaterial;
        _isFlashing = false;
    }

    void BeScared()
    {
        Oink();
        _isScared = true;
        _currentPath = Game.Instance.RoadManager.GetPathToEscape(_currentRoadTile);
        _speed = 6;
        _currentPath.RemoveAt(0); // Remove the current tile from the path
        _moveTween.Kill();
        _spriteRenderer.flipX = false;

        GoAlongPath();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PiggyEndZone endZone))
        {
            _moveTween.Kill();
            var newPos = endZone.GetFreePosition();
            _isEating = true;
            GetSomeFood(_foodCapacity);
            transform.DOMove(newPos, Vector3.Distance(transform.position, newPos) / _speed)
                .SetEase(Ease.Linear).OnComplete(() => StartCoroutine(StartEating()));
        }
        else if (collision.TryGetComponent(out LevelFood food))
        {
            Debug.Log("Piggy got some food");
            collision.gameObject.SetActive(false);
            GetSomeFood(food.FoodCount);
        }
    }

    public void Oink()
    {
        Piggy.MegaOink(Data);
    }

    public static float LastTimeOiinked = 0;
    public static void MegaOink(PiggyData data)
    {
        var myOiks = Oinks.GetOinks(data.Type, data.Rank);
        Game.Instance.AudioManager.PlayRange(myOiks, pitch: Random.Range(0.9f, 1.1f));
        LastTimeOiinked = Time.time;
    }

    void GetSomeFood(int food)
    {
        Oink();
        _foodText.text = "+" + _foodCapacity.ToString();
        _foodText.transform.DOLocalMoveY(0.5f, 0);
        _foodText.DOColor(Color.white, 0);
        _foodText.DOColor(Color.clear, 1).SetDelay(1);
        _foodText.transform.DOLocalMoveY(0.75f, 1).SetEase(Ease.OutBack);
        Game.Instance.Level.PiggyGotSomeFood(food);
    }

    IEnumerator StartEating()
    {
        _spriteRenderer.transform.position = _spriteRenderer.transform.position.SetZ(_spriteRenderer.transform.position.y);
        // Debug.Log("Start eating");
        while (true)
        {
            _shouldJump = false;
            yield return new WaitForSeconds(Random.Range(1.0f, 5.0f));
            if (Random.value > 0.5f)
                _spriteRenderer.flipX = !_spriteRenderer.flipX;
            else
                transform.DOLocalJump(transform.position, 0.5f, 1, 0.5f).SetEase(Ease.Linear);
        }
    }

    void RunAway()
    {
        transform.DOMove(transform.position + Vector3.left * 10, 10).OnComplete(delegate
        {
            gameObject.SetActive(false);
        });
    }

    #if UNITY_EDITOR
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
    #endif
}
