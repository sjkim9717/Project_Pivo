using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StaticManager : MonoBehaviour {
    public static Action Restart;

    private int HpCount = 3;
    private int biscuitCount = 0;
    private Text biscuitCountText;
    private Image[] HpImgaes = new Image[3];

    private GameObject HPGroup;
    private GameObject biscuitGroup;
    public GameObject HoldingGroup;
    public GameObject RestartGroup;

    private PlayerManager playerManager;
    private StageClearController stageClearController;
    private List<BiscuitController> biscuitControllers = new List<BiscuitController>();

    private void Awake() {
        HPGroup = transform.GetChild(0).gameObject;
        biscuitGroup = transform.GetChild(1).gameObject;
        HoldingGroup = transform.GetChild(2).gameObject;
        RestartGroup = transform.GetChild(6).gameObject;

        for(int i =1; i<HPGroup.transform.GetChild(0).childCount; i++) {
            HpImgaes[i-1] = HPGroup.transform.GetComponentsInChildren<Image>()[i];
        }
        biscuitCountText = biscuitGroup.GetComponentInChildren<Text>();

        Restart += LevelInitWhenRestart;
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += FindBiscuitsWhenLevelChange;
        SceneManager.sceneLoaded += LevelInitWhenLevelChange;
        SceneManager.sceneLoaded += FindObjectsWhenLevelChange;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= FindBiscuitsWhenLevelChange;
        SceneManager.sceneLoaded -= LevelInitWhenLevelChange;
        SceneManager.sceneLoaded -= FindObjectsWhenLevelChange;
    }
    private void Update() {
        if (playerManager != null) {
            if ((HpCount + playerManager.GetPlayerDieCount()) != 3) {
                HpImageChange();
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

    // stage 클리어시 변경되는 오브젝트 다시 받아와야함(초기화 다시해야함)
    private void FindObjectsWhenLevelChange(Scene scene, LoadSceneMode mode) {
        playerManager = FindObjectOfType<PlayerManager>();
        stageClearController = FindObjectOfType<StageClearController>();

        //TODO: [확인 필요함]
        PlayerManager.PlayerDead += ShowRestart;
    }


    // stage 변경시 초기화
    private void LevelInitWhenLevelChange(Scene scene, LoadSceneMode mode) {
        HpCount = 3;
        for (int i = 0; i < HpImgaes.Length; i++) {
            HpImgaes[i].gameObject.SetActive(true);
        }
        biscuitCountText.text = "X 0";
        biscuitCount = 0;
    }

    // 비스킷을 먹었을 경우 카운트 증기
    // level clear시에 secen변경이 일어남 -> 모든 스크립트가 달린 오브젝트를 찾아서 재할당이 필요함
    private void FindBiscuitsWhenLevelChange(Scene scene, LoadSceneMode mode) {
        Debug.Log($"Scene loaded: {scene.name}");
        Debug.Log($"Load mode: {mode}");

        biscuitControllers.Clear();

        BiscuitController[] controllers = FindObjectsOfType<BiscuitController>();
        foreach (var controller in controllers) {
            if (!biscuitControllers.Contains(controller)) {
                controller.BiscuiEat += BiscuitEatCount;
                controller.BiscuiEat += BiscuitEatTextChange;
                biscuitControllers.Add(controller);
            }
        }
    }
    private void BiscuitEatCount() {
        biscuitCount++;
        //Debug.Log("StaticManager | biscuitCount |" + biscuitCount);
    }
    private void BiscuitEatTextChange() {
        biscuitCountText.text = string.Format($"X {biscuitCount}");
    }

    // player가 떨어졌을 경우 hp 이미지 없어짐
    private void HpImageChange() {
        HpCount = 3 - playerManager.GetPlayerDieCount();
        for (int i = 0; i < HpImgaes.Length; i++) {
            if (i >= HpCount) HpImgaes[i].gameObject.SetActive(false);
        }
    }


    // 플레이어가 죽었을 경우 restart 화면이 떠야함
    private void ShowRestart() {
        RestartGroup.SetActive(true);
    }

    //TODO: Restart Yes 버튼 할당 필요함
    public void OnButtonClick_Restart() {
        Restart?.Invoke();
    }

}

/*
 1. 비스킷을 먹는 이벤트 발동 시 카운트 증가
 2. 비스킷 텍스트 변경

 3. 클리어 이벤트 발동 시 비스킷 카운트 초기화


//TODO: 재시작 리스트
1. 플레이어 위치 초기화
2. 비스킷 생명 초기화
 
 */