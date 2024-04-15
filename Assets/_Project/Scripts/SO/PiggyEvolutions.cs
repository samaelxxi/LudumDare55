using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PiggyEvolutions", menuName = "PiggyEvolutions", order = 0)]
public class PiggyEvolutions : ScriptableObject
{
    [field: SerializeField] public List<PiggyData> Normal { get; private set; }
    [field: SerializeField] public List<PiggyData> Fat { get; private set; }
    [field: SerializeField] public List<PiggyData> Fast { get; private set; }

    [field: SerializeField] public int InitialPiggies { get; private set; }
    [field: SerializeField] public int MakeFatCost { get; private set; }
    [field: SerializeField] public int MakeFastCost { get; private set; }
    [field: SerializeField] public int BasePiggyCost { get; private set; }
    [field: SerializeField] public int ExtraNewPiggyCost { get; private set; }
    [field: SerializeField] public int MaxPiggies { get; private set; }
    [field: SerializeField] public int LevelsNum { get; private set; }

    public PiggyData GetNextEvolution(PiggyType type, int rank)
    {
        if (rank >= Normal.Count)
        {
            Debug.LogError("No more evolutions for this type");
            return null;
        }

        switch (type)
        {
            case PiggyType.Normal:
                return Normal[rank + 1];
            case PiggyType.Fat:
                return Fat[rank + 1];
            case PiggyType.Fast:
                return Fast[rank + 1];
            default:
                return null;
        }
    }

    public PiggyData GetEvolution(PiggyType type, int rank)
    {
        switch (type)
        {
            case PiggyType.Normal:
                return Normal[rank];
            case PiggyType.Fat:
                return Fat[rank];
            case PiggyType.Fast:
                return Fast[rank];
            default:
                return null;
        }
    }
}
