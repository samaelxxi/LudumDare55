using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct LevelData
{
    public int _requiredPigsToWin;
}

[CreateAssetMenu(fileName = "LevelsData", menuName = "LevelsData", order = 0)]
public class LevelsData : ScriptableObject
{
    [SerializeField] List<LevelData> _levelsData;
}
