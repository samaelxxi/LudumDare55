using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DesignPatterns.Singleton;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Game : Singleton<Game>
{
    [field: SerializeField] public RoadManager RoadManager { get; private set; }
    [field: SerializeField] public PiggyEvolutions PiggyEvolutions { get; private set; }

    public IEnumerable<PiggyData> PlayerPiggies() => _player.GetPiggies();
    public int PlayerCorn => _player.Corn;
    public int PiggiesQuantity => _player.GetPiggies().Count();
    public int NewPiggyCost => PiggyEvolutions.BasePiggyCost + (PiggiesQuantity - PiggyEvolutions.InitialPiggies) * PiggyEvolutions.ExtraNewPiggyCost;
    public bool CanBuyNewPiggy => PlayerCorn >= NewPiggyCost && PiggiesQuantity < PiggyEvolutions.MaxPiggies;

    Player _player;
    Level _level;
    UI _ui;

    SummonPoint _chosenSummonPoint;

    public event System.Action<int> OnCornChanged
    {
        add { _player.OnCornChanged += value; }
        remove { _player.OnCornChanged -= value; }
    }

    public override void Awake()
    {
        base.Awake();
        if (PiggyEvolutions == null)
            PiggyEvolutions = Resources.Load<PiggyEvolutions>("SO/PiggyEvolutions");
        if (RoadManager == null)
            RoadManager = FindFirstObjectByType<RoadManager>();

        _player = new Player(PiggyEvolutions.Normal[0], PiggyEvolutions.InitialPiggies);
        _level = FindFirstObjectByType<Level>();
        _level.SummonPoints[0].BeChosen();
        _chosenSummonPoint = _level.SummonPoints[0];
        _ui = FindFirstObjectByType<UI>();
        _ui.SummonMenu.OnCardClicked += SummonPiggy;


        // add all types of piggies
        _player.UpgradePiggyRank(_player.GetPiggies().First());
        _player.UpgradePiggyRank(_player.GetPiggies().First());
        _player.UpgradePiggyRank(_player.GetPiggies().ElementAt(1));

        _player.ChangePiggyType(_player.GetPiggies().ElementAt(4), PiggyType.Fast);
        _player.ChangePiggyType(_player.GetPiggies().ElementAt(5), PiggyType.Fast);
        _player.ChangePiggyType(_player.GetPiggies().ElementAt(6), PiggyType.Fast);
        _player.UpgradePiggyRank(_player.GetPiggies().ElementAt(5));
        _player.UpgradePiggyRank(_player.GetPiggies().ElementAt(6));
        _player.UpgradePiggyRank(_player.GetPiggies().ElementAt(6));

        _player.ChangePiggyType(_player.GetPiggies().ElementAt(3), PiggyType.Fat);
        _player.ChangePiggyType(_player.GetPiggies().ElementAt(7), PiggyType.Fat);
        _player.ChangePiggyType(_player.GetPiggies().ElementAt(8), PiggyType.Fat);
        _player.UpgradePiggyRank(_player.GetPiggies().ElementAt(7));
        _player.UpgradePiggyRank(_player.GetPiggies().ElementAt(8));
        _player.UpgradePiggyRank(_player.GetPiggies().ElementAt(8));


    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnMouseClick(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public void UpgradePiggyRank(PiggyData piggyData)
    {
        _player.UpgradePiggyRank(piggyData);
    }

    public void ChangePiggyType(PiggyData piggyData, PiggyType type)
    {
        _player.ChangePiggyType(piggyData, type);
    }

    public PiggyData BuyNewPiggy()
    {
        return _player.AddNewPiggy();
    }

    void SummonPiggy(PiggyData piggyData)
    {
        _chosenSummonPoint.AddToSummonQueue(piggyData);
    }

    void OnMouseClick(Vector3 worldPosition)
    {
        RoadManager.TrySwitchRoadTileAt(worldPosition);
        TryChooseSummonPoint(worldPosition);
    }

    void TryChooseSummonPoint(Vector3 worldPosition)
    {
        var summonPointCollider = Physics2D.OverlapPoint(worldPosition, LayerMask.GetMask("SummonPoint"));
        if (summonPointCollider == null)
            return;
        if (summonPointCollider.TryGetComponent<SummonPoint>( out var summonPoint ) )
        {
            if (_chosenSummonPoint != null)
                _chosenSummonPoint.BeUnchosen();
            _chosenSummonPoint = summonPoint;
            _chosenSummonPoint.BeChosen();
        }
    }
}
