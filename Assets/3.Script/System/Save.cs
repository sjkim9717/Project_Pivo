using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class Save : MonoBehaviour {
    public static Save instance = null;

    public GameData GameData = new GameData();

    private string SaveJsonFilePath;

    private void Awake() {
        if (instance == null) {
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
        LoadGame();
    }

    private void InitializeData() {

        GameData.ScreenMode = ScreenMode.FullScreen;
        GameData.ScreenSize = new int[] { 1920, 1080 };

        Screen.SetResolution(GameData.ScreenSize[0], GameData.ScreenSize[1], FullScreenMode.ExclusiveFullScreen);

        GameData.GameSaveData.Clear();
        foreach (StageLevel level in Enum.GetValues(typeof(StageLevel))) {
            Debug.Log($"Adding level: {level}");
            GameData.GameSaveData.Add(new StageLevelData {
                StageLevel = level,
                IsStageClear = false,
                StageScore = 0
            });
        }
    }

    private void LoadGame() {
        if (File.Exists(SaveJsonFilePath)) {
            string jsonData = File.ReadAllText(SaveJsonFilePath);
            GameData loadedData = JsonUtility.FromJson<GameData>(jsonData);

            // 로드한 데이터로 GameSaveData 업데이트
            GameData.ScreenMode = loadedData.ScreenMode;
            GameData.ScreenSize = loadedData.ScreenSize;
            Screen.SetResolution(GameData.ScreenSize[0], GameData.ScreenSize[1], MatchMode(GameData.ScreenMode));

            GameData.GameSaveData = loadedData.GameSaveData;
            Debug.Log("Game data loaded successfully.");
        }
        else {
            Debug.LogWarning("Save file does not exist, initializing new data.");
            InitializeData();
        }
    }

    private FullScreenMode MatchMode(ScreenMode screenMode) {
        switch (screenMode) {
            case ScreenMode.Window:
                return FullScreenMode.Windowed;
            case ScreenMode.FullScreen:
                return FullScreenMode.ExclusiveFullScreen;
            case ScreenMode.FullScreenWindow:
                return FullScreenMode.FullScreenWindow;
            default:
                return FullScreenMode.FullScreenWindow;
        }

    }

    // new play 클릭시 확인
    public bool GetSaveExist() {                            // saveData 있는지 확인하는 용도
        if (File.Exists(SaveJsonFilePath)) {
            if(TryGetStageClear(StageLevel.GrassStageLevel_1, out bool isClear)) {
                if (isClear) {
                    GameManager.instance.IsTutorialCompleted = true;
                    return true;
                }
            }
        }
        return false;
    }

    public void MakeNewGame() {
        GameManager.instance.IsTutorialCompleted = false;

        if (File.Exists(SaveJsonFilePath)) {

            GameData.GameSaveData.Clear();
            foreach (StageLevel level in Enum.GetValues(typeof(StageLevel))) {
                Debug.Log($"Adding level: {level}");
                GameData.GameSaveData.Add(new StageLevelData {
                    StageLevel = level,
                    IsStageClear = false,
                    StageScore = 0
                });
            }

            SaveGame();
        }
        else {
            // Handle the case when no file exists
            GameData = new GameData();
            InitializeData();
            SaveGame();
        }
    }

    public void SaveGame() {
        GameData gameData = new GameData {
            ScreenMode = GameData.ScreenMode,
            ScreenSize = GameData.ScreenSize,
            GameSaveData = GameData.GameSaveData
        };
        string jsonData = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(SaveJsonFilePath, jsonData);
    }

    public void SaveWindow(ScreenMode screenMode, int[] screenSize) {
        if (!File.Exists(SaveJsonFilePath)) {
            SaveGame();
        }

        GameData.ScreenMode = screenMode;
        GameData.ScreenSize = screenSize;

        SaveGame();  // 새로 만든 데이터를 저장
    }

    public StageLevelData GetStageLevelData(StageLevel level) {
        return GameData.GameSaveData.Find(data => data.StageLevel == level);
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
        if (data != null) {
            score = data.StageScore;
            return true;
        }
        else {
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

/*
 1. 목적 : 저장

 2. 저장 내용 
    - 스테이지 클리어 여부
    - 각 스테이지 별 뼈다귀 개수 
    - 각 스테이지 별 클리어 점수 기준

 3. 해야하는 내용
    - 싱글톤
    - 자동 저장

 4. 순서
    - 로컬에 저장 폴더 생성
    - 저장 파일 생성=> 스테이지 클리어시
    - new game 할 경우 파일 있는 지 확인
    - Load -> stage 선택에서 깬 스테이지 다음 거에 플레이어가 서 있음
 */
