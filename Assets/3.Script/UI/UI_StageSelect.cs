using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_StageSelect : UserData {

    public RectTransform ScoreTexts;

    private Text stage;

    private GameObject returnToTitle;


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

        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Return)) {
            GameManager.isLoadTitle = false;
            GameManager.instance.LoadSelectStage(selectStageLevel);
        }
    }

    public void OnButtonClick_ReturnTitle() {
        GameManager.isLoadTitle = true;
        GameManager.instance.LoadSelectStage(StageLevel.GrassStageLevel_1);
    }

    public void SelectStageLevel_Stage(StageLevel stageLevel) {
        selectStageLevel = stageLevel;
        stage.text = stageLevel.ToString();
    }


}

/*
 1. 플레이어가 클리어한 레벨확인해서 
 2. 클리어 레벨 위로 플레이어 위치
 3. 
 
 */
