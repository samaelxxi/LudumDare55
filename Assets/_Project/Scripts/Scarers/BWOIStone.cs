using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class BWOIStone : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] SpriteRenderer _spriteRenderer;

    const float PIGGY_RADIUS = 0.5f;
    const float STONE_SPEED = 9.0f;



    Piggy _target;
    float _damage;
    public void SetTarget(Piggy target, float damage)
    {
        _target = target;
        _damage = damage;
        // Vector3 direction = target.transform.position - transform.position;
        // Vector3 targetPos = target.transform.position - direction.normalized * PIGGY_RADIUS;
        // float time = direction.magnitude / STONE_SPEED;
        // transform.DOMove(target.transform.position, time).OnComplete(delegate
        // {
        //     _animator.Play("StoneVFX");
        //     target.ReceiveNegativeVibes(damage);
        //     Destroy(gameObject);
        // }).SetEase(Ease.Linear);
    }

    bool _isused;

    void Update()
    {
        if (_isused) return;
        _animator.enabled = false;
        if (_target == null) return;
        if (Vector3.Distance(transform.position, _target.transform.position) < 0.1f)
        {
            _isused = true;
            StartCoroutine(Damage());

        }

        Vector3 direction = _target.transform.position - transform.position;
        // rotate slightly on z axis
        transform.Rotate(Vector3.forward, 180 * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, STONE_SPEED * Time.deltaTime);

    }

    IEnumerator Damage()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        _spriteRenderer.gameObject.SetActive(true);
        _animator.enabled = true;
        _animator.Play("StoneVFX");
        _target.ReceiveNegativeVibes(_damage);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject, 0.5f);
    }
}
