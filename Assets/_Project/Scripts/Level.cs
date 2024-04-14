using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] SummonPoint[] _summonPoints;

    [SerializeField] SummonPoint[] SummonPoints => _summonPoints;

    public RoadManager RoadManager { get; private set; }

    public event Action OnLevelCompleted;

    SummonPoint _chosenSummonPoint;

    List<Piggy> _summonedPiggies = new();

    UI _ui;

    int _currentFood = 0;
    int _totalFoodCollected = 0;
    bool _goingToEndWindow;


    void Start()
    {
        RoadManager = FindFirstObjectByType<RoadManager>();
        _ui = FindFirstObjectByType<UI>();
        _ui.SummonMenu.OnCardClicked += SummonPiggy;
        _ui.OnEndLevelClicked += CompleteLevel;
        _currentFood = Game.Instance.PlayerCorn;
        _summonPoints[0].BeChosen();
        _chosenSummonPoint = _summonPoints[0];

        Game.Instance.OnMouseClick += OnMouseClick;
        Game.Instance.OnCornChanged += PiggyGotSomeFood;
        Game.Instance.OnKeyNumberPressed += KeyNumberPressed;
    }

    void OnDestroy()
    {
        Debug.Log("Level destroyed");
        Game.Instance.OnMouseClick -= OnMouseClick;
        Game.Instance.OnCornChanged -= PiggyGotSomeFood;
        Game.Instance.OnKeyNumberPressed -= KeyNumberPressed;
    }

    void KeyNumberPressed(int number)
    {
        if (number >= 1 && number <= _summonPoints.Length)
            ChooseSummonPoint(_summonPoints[number - 1]);
    }

    void Update()
    {
        if (!_goingToEndWindow && IsLevelCompleted())
        {
            _goingToEndWindow = true;
            this.InSeconds(2, ShowEndWindow);
        }
    }

    void ShowEndWindow()
    {
        _ui.SetupEndLevelWindow(_totalFoodCollected);
    }

    public void CompleteLevel()
    {
        OnLevelCompleted?.Invoke();
    }

    public void PiggyGotSomeFood(int food)
    {
        _totalFoodCollected += food - _currentFood;
        _currentFood = food;
    }

    public void OnMouseClick(Vector3 worldPosition)
    {
        TryChooseSummonPoint(worldPosition);
        LevelMouseClick(worldPosition);
    }

    void SummonPiggy(PiggyData piggyData)
    {
        _chosenSummonPoint.AddToSummonQueue(piggyData);
    }

    public void AddHungryPiggy(Piggy piggy)
    {
        _summonedPiggies.Add(piggy);
    }

    public bool IsLevelCompleted()
    {
        return _summonedPiggies.Count == Game.Instance.PiggiesQuantity && 
                _summonedPiggies.All(piggy => piggy.IsFinishedHarvesting);
    }

    void TryChooseSummonPoint(Vector3 worldPosition)
    {
        var summonPointCollider = Physics2D.OverlapPoint(worldPosition, LayerMask.GetMask("SummonPoint"));
        if (summonPointCollider == null)
            return;
        if (summonPointCollider.TryGetComponent<SummonPoint>( out var summonPoint ) )
            ChooseSummonPoint(summonPoint);
    }

    void ChooseSummonPoint(SummonPoint summonPoint)
    {
        // Debug.Log("Chosen summon point");
        if (_chosenSummonPoint != null)
            _chosenSummonPoint.BeUnchosen();
        _chosenSummonPoint = summonPoint;
        _chosenSummonPoint.BeChosen();
    }

    void LevelMouseClick(Vector3 worldPosition)
    {
        RoadManager.TrySwitchRoadTileAt(worldPosition);

        var t = RoadManager.GetRoadTileAt(worldPosition);
        if (t != null)
            Debug.Log(t.Position);
    }
}
