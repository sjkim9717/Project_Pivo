using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerState3D_OpenPanel : PlayerState3D {
    private StaticManager UI_Static;
    private GameObject PanelGroup;
    private GameObject snow;
    private GameObject frame;

    private GameObject temple;
    private int sceneNum;

    protected override void OnEnable() {

        UI_Static = FindObjectOfType<StaticManager>();
        PanelGroup = UI_Static.transform.GetChild(3).gameObject;
        snow = PanelGroup.transform.GetChild(1).gameObject;
        temple = PanelGroup.transform.GetChild(2).gameObject;

        frame = snow.transform.GetChild(1).gameObject;

        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.Contains("Snow")) {
            int lastDigitIndex = sceneName.Length - 1;

            // 마지막 문자가 숫자인지 확인
            if (char.IsDigit(sceneName[lastDigitIndex])) {
                sceneNum = int.Parse(sceneName[lastDigitIndex].ToString());
            }
        }
    }

    public override void EnterState() {
        PanelGroup.SetActive(true);
        if (sceneNum <=7) {
            snow.SetActive(true);
            frame.transform.GetChild(sceneNum + 1).gameObject.SetActive(true);
        }
        else {
            temple.SetActive(true);
        }
    }

    private void Update() {
        if (!PanelGroup.activeSelf) Control3D.ChangeState(PlayerState.Idle);
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape)) {
            PanelGroup.SetActive(false);
            Control3D.ChangeState(PlayerState.Idle);
        }
    }

    public override void ExitState() {
        if (sceneNum <= 7) {
            snow.SetActive(false);
            frame.transform.GetChild(sceneNum + 1).gameObject.SetActive(false);
        }
        else {
            temple.SetActive(false);
        }
    }
}

/*
 1. onenable 할경우 씬을 확인해서
 2. 해당하는 씬의 이름의 뒷자리 숫자를 확인해서
 3. 해당하는 숫자 - 1로 해당 판넬의 이미지 오픈
 
 */