using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    List<string> PIGGY_NAMES = new() 
    {
        "Oinkers", "Piglet", "Baconator", "Piggly Wiggly", "Peppa", "Elvis Pigsley",
        "Sir Oinksalot", "Hamlet", "Squealer", "Ham Solo", "Pigglypuff", "Prissy",
        "Pop", "Donut", "Porky"
    };

    public int Corn { get; private set; } = 900;

    public event System.Action<int> OnCornChanged;

    PiggyData _defaultPiggyData;


    List<PiggyData> _myPiggies = new();

    public Player(PiggyData defaultPiggy, int initialPiggies)
    {
        _defaultPiggyData = defaultPiggy;
        for (int i = 0; i < initialPiggies; i++)
            AddNewPiggy(isFree: true);
    }

    string PickName()
    {
        if (PIGGY_NAMES.Count == 0)
            return "Piggy" + Random.Range(0, 1000);  // fallback name (should never happen
        int idx = Random.Range(0, PIGGY_NAMES.Count);
        string name = PIGGY_NAMES[idx];
        PIGGY_NAMES.RemoveAt(idx);
        return name;
    }

    public PiggyData AddNewPiggy(bool isFree = false)
    {
        if (!isFree)
            SpendCorn(Game.Instance.NewPiggyCost);
        var newData = ScriptableObject.Instantiate(_defaultPiggyData);
        newData.SetName(PickName());
        _myPiggies.Add(newData);
        return newData;
    }

    public void UpgradePiggyRank(PiggyData data)
    {
        SpendCorn(data.UpgradeCost);
        var evolutions = Game.Instance.PiggyEvolutions;
        PiggyData upgradedPiggy = evolutions.GetNextEvolution(data.Type, data.Rank);
        ChangePiggyData(data, upgradedPiggy);
    }

    public void ChangePiggyType(PiggyData data, PiggyType newType)
    {
        int cornCost = newType switch
        {
            PiggyType.Fat => Game.Instance.PiggyEvolutions.MakeFatCost,
            PiggyType.Fast => Game.Instance.PiggyEvolutions.MakeFastCost,
            _ => 0
        };
        SpendCorn(cornCost);
        var evolutions = Game.Instance.PiggyEvolutions;
        PiggyData upgradedPiggy = evolutions.GetEvolution(newType, 0);
        ChangePiggyData(data, upgradedPiggy);
    }

    void SpendCorn(int amount)
    {
        Corn -= amount;
        if (Corn < 0) Debug.LogError("Negative corn");
        OnCornChanged?.Invoke(Corn);
    }

    void ChangePiggyData(PiggyData data, PiggyData newData)
    {
        data.ClonePiggyData(newData);
    }

    public IEnumerable<PiggyData> GetPiggies()
    {
        return _myPiggies;
    }
}
