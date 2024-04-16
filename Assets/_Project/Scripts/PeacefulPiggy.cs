using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PeacefulPiggy : MonoBehaviour
{
    [SerializeField] PiggyData _data;
    [SerializeField] SpriteRenderer _spriteRenderer;


    public SpriteRenderer SpriteRenderer => _spriteRenderer;

    public PiggyData Data => _data;
    BoxCollider2D _walkArea;


    CircleCollider2D _circleCollider;

    float _waitTimer;
    bool _isWalking = false;

    Vector3 _targetPos;

    public void SetData(PiggyData data)
    {
        _data = data;
        _spriteRenderer.sprite = data.Sprite;
    }

    public void SetWalkArea(BoxCollider2D walkArea)
    {
        _walkArea = walkArea;
    }

    public void UpdateData()
    {
        SetData(_data);
    }

    [SerializeField] AnimationCurve _jumpCurve;

    void Start()
    {
        _waitTimer = Random.Range(3.0f, 8.0f);
        _spriteRenderer.flipX = Random.Range(0, 2) == 0;
        _circleCollider = GetComponent<CircleCollider2D>();
    }

    float _jumpTime = 0;

    bool _isEndPiggy = false;
    public void SetToEnd()
    {
        _isEndPiggy = true;
        StartCoroutine(Oinkaninonining());
        _waitTimer = 0;
    }

    void Update()
    {
        if (!_isWalking)
        {
            _waitTimer -= Time.deltaTime;
            if (_waitTimer <= 0)
                ChooseMoveTarget();
        }
        else
            Move();

        // _spriteRenderer.transform.position = _spriteRenderer.transform.position.SetZ(transform.position.y);

        if (_isWalking || _isEndPiggy)
        {
            _jumpTime += Time.deltaTime;
            float jumpLength = 1 / _data.Speed;
            float jumpHeight = Mathf.Max(jumpLength - 0.1f, 0.2f);
            float y = _jumpCurve.Evaluate(_jumpTime / jumpLength);
            var newPos = _spriteRenderer.transform.localPosition.SetY(y * jumpHeight + 0.2f);
            _spriteRenderer.transform.localPosition = Vector3.Lerp(_spriteRenderer.transform.localPosition, newPos, 0.3f);
            _circleCollider.offset = new Vector2(0, y * 0.5f);
            if (_jumpTime >= jumpLength)
                _jumpTime = 0;
        }
        _spriteRenderer.transform.position = _spriteRenderer.transform.position.SetZ(transform.position.y);
    }

    IEnumerator Oinkaninonining()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(4, 7));
            Piggy.MegaOink(Data);
        }
    }

    void Move()
    {
        var ds = _data.Speed * Time.deltaTime;
        if (_isEndPiggy)
            ds *= 2;
        var direction = _targetPos - transform.position;
        float distance = Vector3.Distance(transform.position.SetZ(0), _targetPos.SetZ(0));
        if (distance < ds)
        {
            _isWalking = false;
            _waitTimer = Random.Range(7, 15.0f);
            return;
        }
        transform.position += ds * direction.SetZ(0).normalized;
    }


    void ChooseMoveTarget()
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
        _targetPos = newPos;
        _isWalking = true;

        _spriteRenderer.flipX = direction.x > 0;
    }
}
