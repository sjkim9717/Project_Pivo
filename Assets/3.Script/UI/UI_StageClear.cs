using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_StageClear : UserData {

    protected Image[] stars = new Image[3];
    protected Image[] lines = new Image[2];

    private void Awake() {
        Text[] childs = transform.GetChild(1).GetComponentsInChildren<Text>(true);
        for (int i = 0; i < childs.Length; i++) {
            if (i == childs.Length - 1) {
                saveScore = childs[i];
            }
            else {
                requireScore[i] = childs[i];
            }
        }
        stars = transform.GetChild(2).GetComponentsInChildren<Image>(true);
        lines = transform.GetChild(3).GetComponentsInChildren<Image>(true);

    }

    private void OnEnable() {
        selectStageLevel = GameManager.instance.currentStage;

        // 해당 레벨에 필요한 별 개수와 플레이어가 얻은 별 개수 표현
        SelectStageLevel(selectStageLevel);

        // 플레이어가 얻은 별 개수와 필요한 별 갯수를 비교해서 이미지로 표현
        SelectStageLevel_StarImage(selectStageLevel);
    }

    private void SelectStageLevel_StarImage(StageLevel stageLevel) {

        int[] requirement;
        int saveScore;
        if (Save.instance.TryGetStageScore(stageLevel, out saveScore)) {
            if (StageRequiementScore.TryGetValue(stageLevel, out requirement)) {

                // 별 이미지 활성화
                for (int i = 0; i < requirement.Length; i++) {
                    stars[i].gameObject.SetActive(saveScore >= requirement[i]);
                }

                // 활성화 된 별 이미지 개수에 따른 연결선 활성화
                for (int i = 0; i < stars.Length - 1; i++) {
                    bool isStarActive = stars[i].gameObject.activeSelf;
                    bool isNextStarActive = stars[i + 1].gameObject.activeSelf;

                    lines[i].gameObject.SetActive(isStarActive && isNextStarActive);
                }

            }
        }
    }

    public void ButtonOnClick_StageSelect() {
        SceneManager.LoadScene("StageSelect_Grass");
        //TODO: StageSelect_Grass에서 플레이어가 서있는 위치 조정 필요함

    }
}


/*
 1. 목적 : save 에 저장된 스코어를 확인해서 점수표시하고 
 2. 시작 :stage가 클리어되었을 경우 켜짐
 
 */ 