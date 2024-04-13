using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DesignPatterns.Singleton;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Game : Singleton<Game>
{
    [field: SerializeField] public Grid Grid { get; private set; }
    [field: SerializeField] public Tilemap Roads { get; private set; }
    [field: SerializeField] public RoadManager RoadManager { get; private set; }
    [field: SerializeField] public PiggyEvolutions PiggyEvolutions { get; private set; }

    Player _player;

    public IEnumerable<PiggyData> PlayerPiggies() => _player.GetPiggies();
    public int PlayerCorn => _player.Corn;
    public int PiggiesQuantity => _player.GetPiggies().Count();
    public int NewPiggyCost => PiggyEvolutions.BasePiggyCost + (PiggiesQuantity - PiggyEvolutions.InitialPiggies) * PiggyEvolutions.ExtraNewPiggyCost;
    public bool CanBuyNewPiggy => PlayerCorn >= NewPiggyCost && PiggiesQuantity < PiggyEvolutions.MaxPiggies;

    public event System.Action<int> OnCornChanged
    {
        add { _player.OnCornChanged += value; }
        remove { _player.OnCornChanged -= value; }
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

    public override void Awake()
    {
        base.Awake();
        if (PiggyEvolutions == null)
            PiggyEvolutions = Resources.Load<PiggyEvolutions>("SO/PiggyEvolutions");

        _player = new Player(PiggyEvolutions.Normal[0], PiggyEvolutions.InitialPiggies);
    }
}
