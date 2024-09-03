using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance { get; private set; }

    public StageLevel currentStage;

    private StageClearController[] stageClearController;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }


    private void OnEnable() {
        SceneManager.sceneLoaded += FindScenLevelWhenLevelChange;
        SceneManager.sceneLoaded += FindStageClearWhenLevelChange;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= FindScenLevelWhenLevelChange;
        SceneManager.sceneLoaded -= FindStageClearWhenLevelChange;
    }

    // 씬이 변경될때마다 씬 레벨 확인
    private void FindScenLevelWhenLevelChange(Scene scene, LoadSceneMode mode) {
        string sceneName = SceneManager.GetActiveScene().name;
        SelectSceneLevelWithSceneName(ref currentStage, sceneName);
    }

    // 씬이 변경될때마다 스테이지 클리어 조건 확인
    private void FindStageClearWhenLevelChange(Scene scene, LoadSceneMode mode) {

        if (currentStage == StageLevel.StageSelect) return;

        stageClearController = FindObjectsOfType<StageClearController>();

        // StageClear 이벤트 구독
        foreach (var controller in stageClearController) {
            if (controller != null) {
                // 이벤트 핸들러가 이미 등록되어 있는지 확인하고, 중복 등록을 방지합니다.
                if (!controller.StageClear.GetInvocationList().Contains((Action)OnStageClear)) {
                    controller.StageClear += OnStageClear;
                }
            }
        }
    }

    //TODO:[확인 필요함] StageClear 이벤트가 호출될 때 실행될 메서드
    private void OnStageClear() {
        Save.instance.SaveData.SetStageData(currentStage, true, GetComponentInChildren<StaticManager>().GetBiscuitCount());
        Save.instance.MakeSave();
    }

    private StageLevel SelectSceneLevelWithSceneName(ref StageLevel stageLevel, string sceneName) {

        switch (sceneName) {
            case "StartTest":
                stageLevel = StageLevel.StageLevel_1;
                break;
            case "GrassStage_Stage1":
                stageLevel = StageLevel.StageLevel_1;
                break;
            case "GrassStage_Stage5":
                stageLevel = StageLevel.StageLevel_5;
                break;
            case "GrassStage_Stage7":
                stageLevel = StageLevel.StageLevel_7;
                break;
            case "StageSelect_Grass":
                stageLevel = StageLevel.StageSelect;
                break;
            default:
                break;
        }

        return stageLevel;
    }

    //TODO: stage clear 시 저장


    //TODO: stage level 선택씬에서 연결할 것
    public void OnButtonClick_LevelChoose(StageLevel stageLevel) {

    }

}

/*  목적 : 씬 변경되는 시점의 stagelevel data 들고옴 + clear 시 정보저장
 1. 씬 로드시에 해당 레벨 들고있을 것
    - if : 레벨 선택 씬이라면 => 버튼에 씬 레벨 각각 넣을 것
 2. 스테이지 클리어시 save의 Data호출
    - 호출된 Data에 해당 레벨 클리어 bool값 + count 넣을 것 
 
 */