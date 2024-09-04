using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

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

[Serializable]
public class StageLevelData {
    public StageLevel StageLevel;
    public bool IsStageClear;
    public int StageScore;
}

[Serializable]
public class StageSaveData {
    public List<StageLevelData> StageLevelDataList = new List<StageLevelData>();

    public StageSaveData() {
        InitializeData();
    }

    private void InitializeData() {
        foreach (StageLevel level in Enum.GetValues(typeof(StageLevel))) {
            StageLevelDataList.Add(new StageLevelData {
                StageLevel = level,
                IsStageClear = false,
                StageScore = 0
            });
        }
    }
    public StageLevelData GetStageLevelData(StageLevel level) {
        return StageLevelDataList.Find(data => data.StageLevel == level);
    }
    public bool TryGetStageClear(StageLevel level, out bool isClear) {
        var data = GetStageLevelData(level);
        if (data != null) {
            isClear = data.IsStageClear;
            return true;
        }
        else {
            isClear = false;
            return false;
        }
    }

    public bool TryGetStageScore(StageLevel level, out int score) {
        var data = GetStageLevelData(level);
        Debug.Log("data != null " + level);
        if (data != null) {
            Debug.Log("data != null " + data.StageScore);
            score = data.StageScore;
            Debug.Log("data != null |  " + score);
            return true;
        }
        else {
            Debug.Log("data == null ");
            score = 0;
            return false;
        }
    }

    public void SetStageData(StageLevel level, bool isClear, int score) {
        var data = GetStageLevelData(level);
        if (data != null) {
            data.IsStageClear = isClear;
            data.StageScore = score;
        }
    }
}

public class Save : MonoBehaviour
{
    public static Save instance = null;

    public StageSaveData SaveData = new StageSaveData();

    private string SaveJsonFilePath;

    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

        SaveJsonFilePath = Path.Combine(Application.persistentDataPath, "Save/SaveData.json");
        if (!Directory.Exists(Path.GetDirectoryName(SaveJsonFilePath))) {
            Directory.CreateDirectory(Path.GetDirectoryName(SaveJsonFilePath));
        }
        Debug.Log("Save file path :" + SaveJsonFilePath);
    }


    // new play 클릭시 확인
    public bool GetSaveExist() {                            // saveData 있는지 확인하는 용도
        if (File.Exists(SaveJsonFilePath)) return true;
        return false;
    }

    public void MakeSave() {
        if (SaveData == null) {
            SaveData = new StageSaveData();
        }

        File.WriteAllText(SaveJsonFilePath, JsonUtility.ToJson(SaveData));  // 덮어쓰기
    }

    public StageSaveData Load() {
        if (GetSaveExist()) {
            return JsonUtility.FromJson<StageSaveData>(File.ReadAllText(SaveJsonFilePath));
        }
        return null;
    }

    public StageLevelData GetStageLevelData(StageLevel level) {
        if (SaveData != null) {
            return SaveData.GetStageLevelData(level);
        }
        return null;
    }





}

/*
 1. 목적 : 저장

 2. 저장 내용 
    - 스테이지 클리어 여부
    - //TODO: 각 스테이지 별 뼈다귀 개수 

    - //TODO: 각 스테이지 별 클리어 점수 기준

 3. 해야하는 내용
    - 싱글톤
    - 자동 저장

 4. 순서
    - 로컬에 저장 폴더 생성
    - 저장 파일 생성=> 스테이지 클리어시
    - new game 할 경우 파일 있는 지 확인
    - Load -> stage 선택에서 깬 스테이지 다음 거에 플레이어가 서 있음
 */
