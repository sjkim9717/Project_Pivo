using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum StageLevel {
    GrassStageLevel_1,
    GrassStageLevel_5,
    GrassStageLevel_7,
    SnowStageLevel_3,
    SnowStageLevel_4,
    SnowStageLevel_6,
    SnowStageLevel_7,
    StageSelect
}

[Serializable]
public class StageLevelData {
    public StageLevel StageLevel;
    public bool IsStageClear;
    public int StageScore;
}

[Serializable]
public class GameData {
    public List<StageLevelData> GameSaveData = new List<StageLevelData>();
}

