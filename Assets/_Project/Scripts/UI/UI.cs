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

    [SerializeField] TextMeshProUGUI _topText;
    [SerializeField] GameObject _winMenu;
    [SerializeField] TextMeshProUGUI _winMenuFoodText;
    [SerializeField] TextMeshProUGUI _winMenuPigsText;
    [SerializeField] GameObject _loseMenu;
    [SerializeField] TextMeshProUGUI _loseMenuFoodText;
    [SerializeField] TextMeshProUGUI _loseMenuPigsText;

    [SerializeField] TextMeshProUGUI _HappyPigsCount;
    [SerializeField] TextMeshProUGUI _currentFoodCount;

    public SummonMenu SummonMenu => _summonMenu;


    public event Action OnEndLevelClicked;
    public event Action OnRestartLevelClicked;

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
        if (Game.Instance.Level.IsLevelSucceed())
        {
            _topText.text = "Pigs like you!";
            _winMenu.SetActive(true);
            _winMenuFoodText.text = food.ToString();
            _winMenuPigsText.text = Game.Instance.Level.HappyPigsCount().ToString();
        }
        else
        {
            _topText.text = "Pigs don't like you!";
            _loseMenu.SetActive(true);
            _loseMenuFoodText.text = food.ToString();
            _loseMenuPigsText.text = Game.Instance.Level.HappyPigsCount().ToString();
        }

        _endLevelWindow.SetActive(true);
        _endLevelWindow.transform.localScale = Vector3.zero;
        _endLevelWindow.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public void EndLevelClicked()
    {
        OnEndLevelClicked?.Invoke();
    }

    public void SetCurrentFoodCount(int food)
    {
        _currentFoodCount.text = food.ToString();
    }

    public void SetHappyPigsCount(int count)
    {
        _HappyPigsCount.text = $"{count}/{Game.Instance.Level.RequiredPigsToWin}";
    }

    public void RestartLevel()
    {
        OnRestartLevelClicked?.Invoke();
    }
}
