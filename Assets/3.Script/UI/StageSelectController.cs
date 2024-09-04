using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelectController : MonoBehaviour {

    public RectTransform ScoreTexts;
    private Text stage;
    private Text saveScore;
    private Text[] requireScore = new Text[3];

    private GameObject returnToTitle;
    private StageLevel selectStageLevel = StageLevel.StageSelect;

    private Dictionary<StageLevel, int[]> StageRequiementScore = new Dictionary<StageLevel, int[]> {
        {StageLevel.StageLevel_1, new int[] { 0, 11, 15} },
        {StageLevel.StageLevel_5, new int[] { 0, 8, 14} },
        {StageLevel.StageLevel_7, new int[] { 0, 11, 15} }
    };

    private void Awake() {
        returnToTitle = transform.GetChild(2).gameObject;

        Text[] childs = ScoreTexts.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < childs.Length; i++) {
            if (i == 0) {
                saveScore = childs[i];
            }
            else if (i < 4) {
                requireScore[i - 1] = childs[i];
            }
            else {
                stage = childs[i];
            }
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            returnToTitle.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.X)) {
            LoadSelectStage();
        }
    }

    public void OnButtonClick_ReturnTitle() {
        SceneManager.LoadScene("GrassStage_Stage1");
    }

    private void LoadSelectStage() {
        string sceneName = "";
        switch (selectStageLevel) {
            case StageLevel.StageLevel_1:
                sceneName = "GrassStage_Stage1";
                break;
            case StageLevel.StageLevel_5:
                sceneName = "GrassStage_Stage5";
                break;
            case StageLevel.StageLevel_7:
                sceneName = "GrassStage_Stage7";
                break;
            case StageLevel.StageSelect:
                break;
            default:
                break;
        }

        SceneManager.LoadScene(sceneName);
    }


    public void SelectStageLevel(StageLevel stageLevel) {
        selectStageLevel = stageLevel;
        stage.text = stageLevel.ToString();
        int[] requirement;

        // 해당 레벨에 필요한 별 갯수
        if (StageRequiementScore.TryGetValue(stageLevel, out requirement)) {
            for (int i = 0; i < requirement.Length; i++) {
                if(requireScore[i].gameObject.activeSelf)
                requireScore[i].text = requirement[i].ToString();
            }
        }

        // 플레이어가 획득한 별 갯수
        int _saveScore = 0;
        if (Save.instance.TryGetStageScore(stageLevel,out _saveScore)) {
            saveScore.text = string.Format($"x {_saveScore}");
        }
        else {
            saveScore.text = "x 0";
        }

    }
}

/*
 1. 플레이어가 클리어한 레벨확인해서 
 2. 클리어 레벨 위로 플레이어 위치
 3. 
 
 */
