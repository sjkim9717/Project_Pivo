using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance { get; private set; }

    public static bool isLoadTitle=true;
    public bool IsTutorialCompleted;
    public StageLevel currentStage;
    private GameObject staticGroup;
    //private GameObject UI_Title;

    private StageClearController[] stageClearController;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

        staticGroup = transform.GetChild(0).gameObject;
        //UI_Title = FindObjectOfType<MainTitleManager>().gameObject;
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += FindScenLevelWhenLevelChange;
        SceneManager.sceneLoaded += FindObjectsWhenLevelChange;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= FindScenLevelWhenLevelChange;
        SceneManager.sceneLoaded -= FindObjectsWhenLevelChange;
    }

    // 씬이 변경될때마다 씬 레벨 확인
    private void FindScenLevelWhenLevelChange(Scene scene, LoadSceneMode mode) {
        string sceneName = SceneManager.GetActiveScene().name;
        currentStage  =  SelectSceneLevelWithSceneName( sceneName);
    }

    // 씬이 변경될때마다 스테이지 클리어 조건 확인
    private void FindObjectsWhenLevelChange(Scene scene, LoadSceneMode mode) {

        if (currentStage == StageLevel.StageSelect) {
            staticGroup.SetActive(false);
        }

        else {
            staticGroup.SetActive(true);

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
    }

    // StageClear 이벤트가 호출될 때 실행될 메서드
    private void OnStageClear() {
        PlayerManage.instance.CurrentMode = PlayerMode.AutoMode;
        PlayerManage.instance.ChangeAutoMode();
        //TODO: outro
        Save.instance.SetStageData(currentStage, true, GetComponentInChildren<StaticManager>().GetBiscuitCount());
        Save.instance.SaveGame();
        SceneManager.LoadScene("StageSelect_Grass");
    }

    // scene이 로드될경우 해당씬 확인
    public StageLevel SelectSceneLevelWithSceneName( string sceneName) {
        StageLevel stageLevel = StageLevel.StageSelect;

        switch (sceneName) {
            case "GrassStage_Stage1":
                stageLevel = StageLevel.GrassStageLevel_1;
                break;
            case "GrassStage_Stage5":
                stageLevel = StageLevel.GrassStageLevel_5;
                break;
            case "GrassStage_Stage7":
                stageLevel = StageLevel.GrassStageLevel_7;
                break;
            case "StageSelect_Grass":
                stageLevel = StageLevel.StageSelect;
                break;
            case "SnowStage_Stage3":
                stageLevel = StageLevel.SnowStageLevel_3;
                break;
            case "SnowStage_Stage4":
                stageLevel = StageLevel.SnowStageLevel_4;
                break;
            case "SnowStage_Stage6":
                stageLevel = StageLevel.SnowStageLevel_6;
                break;
            case "SnowStage_Stage7":
                stageLevel = StageLevel.SnowStageLevel_7;
                break;
            default:
                stageLevel = StageLevel.GrassStageLevel_1;
                break;
        }
        return stageLevel;
    }


    public void LoadSelectStage(StageLevel selectStageLevel) {
        string sceneName = "";
        switch (selectStageLevel) {
            case StageLevel.GrassStageLevel_1:
                sceneName = "GrassStage_Stage1";
                break;
            case StageLevel.GrassStageLevel_5:
                sceneName = "GrassStage_Stage5";
                break;
            case StageLevel.GrassStageLevel_7:
                sceneName = "GrassStage_Stage7";
                break;
            case StageLevel.SnowStageLevel_3:
                sceneName = "SnowStage_Stage3";
                break;
            case StageLevel.SnowStageLevel_4:
                sceneName = "SnowStage_Stage4";
                break;
            case StageLevel.SnowStageLevel_6:
                sceneName = "SnowStage_Stage6";
                break;
            case StageLevel.SnowStageLevel_7:
                sceneName = "SnowStage_Stage7";
                break;
            case StageLevel.StageSelect:
                break;
            default:
                break;
        }
        SceneManager.LoadScene(sceneName);
    }




}

/*  목적 : 씬 변경되는 시점의 stagelevel data 들고옴 + clear 시 정보저장
 1. 씬 로드시에 해당 레벨 들고있을 것
    - if : 레벨 선택 씬이라면 => 버튼에 씬 레벨 각각 넣을 것
 2. 스테이지 클리어시 save의 Data호출
    - 호출된 Data에 해당 레벨 클리어 bool값 + count 넣을 것 
 
 */