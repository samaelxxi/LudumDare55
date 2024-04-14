using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class UI : MonoBehaviour
{
    [SerializeField] SummonMenu _summonMenu;

    [SerializeField] GameObject _endLevelWindow;
    [SerializeField] TextMeshProUGUI _endLevelFoodText;

    public SummonMenu SummonMenu => _summonMenu;


    public event Action OnEndLevelClicked;

    void Start()
    {
        InitSummonMenu();
    }

    public void InitSummonMenu()
    {
        _summonMenu.Init();
    }

    public void SetupEndLevelWindow(int food)
    {
        _endLevelFoodText.text = food.ToString();
        _endLevelWindow.SetActive(true);
        _endLevelWindow.transform.localScale = Vector3.zero;
        _endLevelWindow.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public void EndLevelClicked()
    {
        OnEndLevelClicked?.Invoke();
    }
}
