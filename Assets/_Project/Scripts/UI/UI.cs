using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] SummonMenu _summonMenu;

    public SummonMenu SummonMenu => _summonMenu;

    public event Action<Vector3> OnMouseClick;

    void Start()
    {
        InitSummonMenu();
    }

    public void InitSummonMenu()
    {
        _summonMenu.Init();
    }
}
