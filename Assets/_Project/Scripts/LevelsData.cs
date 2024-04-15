using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct LevelData
{
    public int RequiredPigsToWin;
    public int PiggiesReward;
}

[CreateAssetMenu(fileName = "LevelsData", menuName = "LevelsData", order = 0)]
public class LevelsData : ScriptableObject
{
    [SerializeField] List<LevelData> _levelsData;

    public LevelData GetLevelData(int level)
    {
        return _levelsData[level];
    }
}
