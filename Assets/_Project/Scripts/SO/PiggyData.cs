using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PiggyType
{
    Normal,
    Fat,
    Fast
}

[CreateAssetMenu(fileName = "PiggyData", menuName = "PiggyData", order = 0)]
public class PiggyData : ScriptableObject
{
    [field: SerializeField] public float Speed { get; set; }
    [field: SerializeField] public float Health { get; set; }
    [field: SerializeField] public float Fattiness { get; set; }
    [field: SerializeField] public int FoodCapacity { get; set; } = 30;
    [field: SerializeField] public PiggyType Type { get; set; }
    [field: SerializeField] public int Rank { get; set; }
    [field: SerializeField] public Sprite Sprite { get; set; }
    [field: SerializeField] public Sprite Avatar { get; set; }
    [field: SerializeField] public int UpgradeCost { get; set; }

    public string Name { get; private set; }


    public void ClonePiggyData(PiggyData data)
    {
        Speed = data.Speed;
        Health = data.Health;
        Fattiness = data.Fattiness;
        Type = data.Type;
        Rank = data.Rank;
        Sprite = data.Sprite;
        Avatar = data.Avatar;
        UpgradeCost = data.UpgradeCost;
    }

    public void SetName(string name)
    {
        Name = name;
    }
}
