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
    StageLevel_1,
    StageLevel_5,
    StageLevel_7,
    StageSelect
}

[System.Serializable]
public class TutorialData {
    public bool IsTutorialCompleted;
}

[Serializable]
public class StageLevelData {
    public StageLevel StageLevel;
    public bool IsStageClear;
    public int StageScore;
}

[System.Serializable]
public class GameSaveData {
    public List<StageLevelData> SaveDataList = new List<StageLevelData>();
    public TutorialData TutorialData = new TutorialData();
}
