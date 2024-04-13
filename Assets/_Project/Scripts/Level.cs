using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] SummonPoint[] _summonPoints;

    public SummonPoint[] SummonPoints => _summonPoints;
}
