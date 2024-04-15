using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] SummonPoint[] _summonPoints;

    [SerializeField] SummonPoint[] SummonPoints => _summonPoints;

    [SerializeField] LevelData _levelData;

    public RoadManager RoadManager { get; private set; }

    public event Action OnLevelCompleted;

    SummonPoint _chosenSummonPoint;

    List<Piggy> _summonedPiggies = new();
    List<string> _summonedPiggyNames = new();

    public bool IsPiggySummoned(string name)
    {
        return _summonedPiggyNames.Any(piggyName => piggyName == name);
    }

    public IEnumerable<Piggy> SummonedPiggies => _summonedPiggies;
    public int RequiredPigsToWin => _levelData.RequiredPigsToWin;

    UI _ui;

    int _totalFoodCollected = 0;
    public int TotalFoodCollected => _totalFoodCollected;
    bool _goingToEndWindow;


    void Start()
    {
        RoadManager = FindFirstObjectByType<RoadManager>();
        _ui = FindFirstObjectByType<UI>();
        _ui.SummonMenu.OnCardClicked += SummonPiggy;
        _ui.OnEndLevelClicked += CompleteLevel;
        _ui.OnRestartLevelClicked += RestartLevel;
        _summonPoints[0].BeChosen();
        _chosenSummonPoint = _summonPoints[0];

        Game.Instance.OnMouseClick += OnMouseClick;
        Game.Instance.OnKeyNumberPressed += KeyNumberPressed;
    }

    void OnDestroy()
    {
        Debug.Log("Level destroyed");
        Game.Instance.OnMouseClick -= OnMouseClick;
        Game.Instance.OnKeyNumberPressed -= KeyNumberPressed;
    }

    void KeyNumberPressed(int number)
    {
        if (number >= 1 && number <= _summonPoints.Length)
            ChooseSummonPoint(_summonPoints[number - 1]);
    }


    int _currentHappyPigsCount = 0;
    void Update()
    {
        if (!_goingToEndWindow && IsLevelCompleted())
        {
            _goingToEndWindow = true;
            this.InSeconds(2, ShowEndWindow);
        }
        if (_currentHappyPigsCount != HappyPigsCount())
        {
            _currentHappyPigsCount = HappyPigsCount();
            _ui.SetHappyPigsCount(_currentHappyPigsCount);
        }
    }

    void ShowEndWindow()
    {
        _ui.SetupEndLevelWindow(_totalFoodCollected);
    }

    public void CompleteLevel()
    {
        Game.Instance.AudioManager.Play("Click", pitch: UnityEngine.Random.Range(0.9f, 1.1f));
        OnLevelCompleted?.Invoke();
    }

    public bool IsLevelSucceed()
    {
        return _summonedPiggyNames.Count == Game.Instance.PiggiesQuantity && 
                _summonedPiggies.Where(piggy => piggy.IsGotSomeFood).Count() >= _levelData.RequiredPigsToWin;
    }

    public int GetFoodCollected()
    {
        return _totalFoodCollected;
    }

    public int HappyPigsCount()
    {
        return _summonedPiggies.Where(piggy => piggy.IsGotSomeFood).Count();
    }

    public void RestartLevel()
    {
        Debug.Log("Restarting level");
    }

    public void PiggyGotSomeFood(int food)
    {
        _totalFoodCollected += food ;
        _ui.SetCurrentFoodCount(_totalFoodCollected);
    }

    public void OnMouseClick(Vector3 worldPosition)
    {
        TryChooseSummonPoint(worldPosition);

        RoadManager.TrySwitchRoadTileAt(worldPosition);
        var t = RoadManager.GetRoadTileAt(worldPosition);
        if (t != null)
            Debug.Log(t.Position);
    }

    void SummonPiggy(PiggyData piggyData)
    {
        _summonedPiggyNames.Add(piggyData.Name);
        _chosenSummonPoint.AddToSummonQueue(piggyData);
    }

    public void AddHungryPiggy(Piggy piggy)
    {
        _summonedPiggies.Add(piggy);
    }

    public bool IsLevelCompleted()
    {
        return _summonedPiggyNames.Count == Game.Instance.PiggiesQuantity && 
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
}
