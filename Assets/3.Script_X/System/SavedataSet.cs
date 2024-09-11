using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: [시간 남을 경우] 이거 관리 할 수 있나?
public enum SceneStatus {
    Stage,
    Select
}

[Serializable]
public enum StageLevel {
    GrassStageLevel_1,
    GrassStageLevel_5,
    GrassStageLevel_7,
    SnowStageLevel_3,
    StageSelect
}

[Serializable]
public class StageLevelData {
    public StageLevel StageLevel;
    public bool IsStageClear;
    public int StageScore;
}

