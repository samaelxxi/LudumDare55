using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelFood : MonoBehaviour
{
    [SerializeField] int _foodCount;

    public int FoodCount => _foodCount;

    void Start()
    {
        transform.DOMoveY(transform.position.y + 0.2f, 1).SetLoops(-1, LoopType.Yoyo);
    }
}
