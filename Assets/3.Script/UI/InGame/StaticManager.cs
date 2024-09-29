using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StaticManager : MonoBehaviour {
    //public static Action Restart;

    private int HpCount = 3;
    private int biscuitCount = 0;

    private bool isPauseCanShow;

    private Text biscuitCountText;
    private Image[] HpImgaes = new Image[3];

    private GameObject UI_Title;
    private GameObject HPGroup;
    private GameObject biscuitGroup;
    public GameObject HoldingGroup;
    public GameObject PanelGroup;
    public GameObject PauseGroup;

    private PlayerManage playerManager;
    private List<Biscuit> biscuitControllers = new List<Biscuit>();
    public int GetBiscuitCount() { return biscuitCount; }

    private void Awake() {

        HPGroup = transform.GetChild(0).gameObject;
        biscuitGroup = transform.GetChild(1).gameObject;
        HoldingGroup = transform.GetChild(2).gameObject;
        PanelGroup = transform.GetChild(3).gameObject;
        PauseGroup = transform.GetChild(4).gameObject;

        for(int i =1; i<HPGroup.transform.GetChild(0).childCount; i++) {
            HpImgaes[i-1] = HPGroup.transform.GetComponentsInChildren<Image>()[i];
        }
        biscuitCountText = biscuitGroup.GetComponentInChildren<Text>();

        //Restart += LevelInitWhenRestart;
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += FindObjectsWhenLevelChange;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= FindObjectsWhenLevelChange;
    }
    private void Update() {
        if (playerManager != null) {
            if ((HpCount + playerManager.GetPlayerDieCount()) != 3) {
                HpImageChange();
            }
        }

        if (isPauseCanShow) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                PauseGroup.SetActive(true);
            }
        }

    }

    // Restart시에도 카운트 초기화 되어야함
    private void LevelInitWhenRestart() {
        HpCount = 3;
        for (int i = 0; i < HpImgaes.Length; i++) {
            HpImgaes[i].gameObject.SetActive(true);
        }
        biscuitCountText.text = "X 0";
        biscuitCount = 0;
    }

    private void FindObjectsWhenLevelChange(Scene scene, LoadSceneMode mode) {
        Debug.LogWarning($"Static Manager Scene loaded: {scene.name}");

        //TODO: [수정]
        if (scene.name == "GrassStage_Stage1") {
            UI_Title = FindObjectOfType<UI_Title>(true).gameObject;
            if (UI_Title != null) {
                isPauseCanShow = !UI_Title.activeSelf;
            }
        }
        else {
            if (scene.name == "StageSelect_Grass") {
                isPauseCanShow = false;
            }
            else {
                isPauseCanShow = true;
            }
        }

        playerManager = FindObjectOfType<PlayerManage>();

        LevelInitWhenRestart();

        biscuitControllers.Clear();
        // 모든 비스킷들을 찾아서 할당해주어야함
        Biscuit[] controllers = FindObjectsOfType<Biscuit>();
        Debug.LogWarning($"BiscuitController | {controllers.Length}");
        foreach (var controller in controllers) {
            if (!biscuitControllers.Contains(controller)) {
                controller.BiscuiEat += BiscuitEatTextChange;
                biscuitControllers.Add(controller);
            }
        }
    }

    private void BiscuitEatTextChange() {
        biscuitCount++;
        //Debug.Log("StaticManager | biscuitCount |" + biscuitCount);
        biscuitCountText.text = string.Format($"X {biscuitCount}");
    }

    // player가 떨어졌을 경우 hp 이미지 없어짐
    private void HpImageChange() {
        HpCount = 3 - playerManager.GetPlayerDieCount();
        for (int i = 0; i < HpImgaes.Length; i++) {
            if (i >= HpCount) HpImgaes[i].gameObject.SetActive(false);
        }
    }

    // Restart Yes 버튼 할당 필요함
    public void OnButtonClick_Restart() {
        //씬 재로드 해보기
        GameManager.isLoadTitle = false;
        GameManager.instance.LoadSelectStage(GameManager.instance.currentStage);
        
        //Restart?.Invoke();
    }

    public void ButtonOnClick_StageSelect() {
        GameManager.instance.LoadSelectStage(StageLevel.StageSelect);

        //TODO: StageSelect_Grass에서 플레이어가 서있는 위치 조정 필요함

    }
}

/* 목적 : 씬 변경되는 시점의 표시 상태 초기화 / 각 아이템들 스크립트 잡아옴
 1. 비스킷을 먹는 이벤트 발동 시 카운트 증가
 2. 비스킷 텍스트 변경

 3. 클리어 이벤트 발동 시 비스킷 카운트 초기화


// 재시작 리스트
1. 플레이어 위치 초기화
2. 비스킷 생명 초기화
3. 플레이어 애니메이션 idle
 */