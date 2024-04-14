using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class BWOIStone : MonoBehaviour
{
    const float PIGGY_RADIUS = 0.5f;
    const float STONE_SPEED = 8.0f;

    public void SetTarget(Piggy target, float damage)
    {
        Vector3 direction = target.transform.position - transform.position;
        Vector3 targetPos = target.transform.position - direction.normalized * PIGGY_RADIUS;
        float time = direction.magnitude / STONE_SPEED;
        transform.DOMove(target.transform.position, time).OnComplete(delegate
        {
            target.ReceiveNegativeVibes(damage);
            Destroy(gameObject);
        }).SetEase(Ease.Linear);
    }
}
