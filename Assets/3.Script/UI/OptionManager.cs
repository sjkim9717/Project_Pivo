using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour {
    private GameObject mainGroup;
    private GameObject selectActive;

    private Text windowModeText;
    private Text windowSizeText;

    // 화면 모드
    private ScreenMode currentScreenMode = ScreenMode.FullScreen;
    private ScreenMode selectScreenMode = ScreenMode.FullScreen;

    // 해상도 변경
    private List<int[]> screenSizeList = new List<int[]>
    {
        new int[] { 960, 540 },
        new int[] { 1024, 576 },
        new int[] { 1152, 648 },
        new int[] { 1366, 768 },
        new int[] { 1280, 720 },
        new int[] { 1600, 900 },
        new int[] { 1920, 1080 }
    };

    private int selectScreenSizeIndex = 0;
    private int[] selectScreenSize = new int[2]; // 사용자 설정 너비
    private int[] currentScreenSize = new int[2]; // 현재 화면 설정 너비

    private int[] deviceScreenSize = new int[] { 1920, 1080}; // 화면 사이즈 저장

    private void Awake() {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //Screen.SetResolution(1920, 1080, true);

        mainGroup = transform.parent.GetChild(0).gameObject;
        windowModeText = transform.GetChild(2).Find("WindowModeValueText").GetComponent<Text>();
        windowSizeText = transform.GetChild(3).Find("ResoultionValueText").GetComponent<Text>();
        selectActive = transform.GetChild(6).Find("ApplyActive").gameObject;
        Debug.LogWarning("Awake First| " + selectActive.name);

        currentScreenSize = deviceScreenSize;
        selectScreenSize = currentScreenSize;
    }

    private void Update() {
        if (CheckSelectModeChange()) {
            if (selectActive != null)
                selectActive.SetActive(true);
        }
        else {
            if (selectActive != null) {
                if (selectActive.activeSelf) {
                    selectActive.SetActive(false);
                }
            }
        }
    }

    private bool CheckSelectModeChange() {
        if (currentScreenMode != selectScreenMode || selectScreenSize != currentScreenSize) {
            return true;
        }
        return false;
    }

    //TODO: 창모드 화면 사이즈 결정 해서 버튼 눌릴 경우 변경되어야함
    //TODO: [Text 추가해야함] 
    public void ButtonOnClick_ScreenMode(bool RightArrow) {

        int maxCount = Enum.GetValues(typeof(ScreenMode)).Length;

        if (RightArrow) {
            if ((int)selectScreenMode < maxCount - 1) {
                selectScreenMode = (ScreenMode)((int)selectScreenMode + 1);
            }
            else {
                selectScreenMode = (ScreenMode)0;  // 최대값일 경우 0번째 값으로
            }
        }
        else {
            if ((int)selectScreenMode > 0) {
                selectScreenMode = (ScreenMode)((int)selectScreenMode - 1);
            }
            else {
                selectScreenMode = (ScreenMode)(maxCount - 1);  // 0번째일 경우 최대값으로
            }
        }
        //windowModeText.text = selectScreenMode.ToString();
        windowModeText.text = (string)Enum.GetName(typeof(ScreenMode), selectScreenMode);
    }


    //TODO: [Text 추가해야함] 
    public void ButtonOnClick_ScreenSize(bool RightArrow) {
        int maxCount = screenSizeList.Count;
        if (RightArrow) {
            if (selectScreenSizeIndex < maxCount - 1) {
                selectScreenSizeIndex++;
            }
            else {
                selectScreenSizeIndex = 0;
            }
        }
        else {
            if (selectScreenSizeIndex > 0) {
                selectScreenSizeIndex--;
            }
            else {
                selectScreenSizeIndex = maxCount;
            }

        }
        selectScreenSize = screenSizeList[selectScreenSizeIndex];
        windowSizeText.text = string.Format($"{selectScreenSize[0]} * {selectScreenSize[1]}");
    }


    public void ButtonOnClick_Cancle() {
        gameObject.SetActive(false);
        mainGroup.SetActive(true);
    }



    public void ButtonOnClick_Apply() {
        //TODO: 창모드, 사운드 조정하는 메소드

        if (selectScreenSizeIndex < 0 || selectScreenSizeIndex >= screenSizeList.Count) {
            Debug.LogError("Invalid resolution index.");
            return;
        }

        int width = screenSizeList[selectScreenSizeIndex][0];
        int height = screenSizeList[selectScreenSizeIndex][1];

        UpdateCanvasScaler();

        // 화면 모드에 따른 설정 적용
        switch (selectScreenMode) {
            case ScreenMode.Window:
                Screen.SetResolution(width, height, FullScreenMode.Windowed);
                break;
            case ScreenMode.FullScreen:
                Screen.SetResolution(width, height, FullScreenMode.ExclusiveFullScreen);
                break;
            case ScreenMode.FullScreenWindow:
                Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);
                break;
        }

        currentScreenMode = selectScreenMode;
        currentScreenSize = selectScreenSize;

        Debug.Log("currentScreenSize | " + currentScreenSize[0] + currentScreenSize[1]);

    }

    private void UpdateCanvasScaler() {
        //Default 해상도 비율
        float fixedAspectRatio = 1920f / 1080f;

        //현재 해상도의 비율
        float currentAspectRatio = (float)Screen.width / (float)Screen.height;

        CanvasScaler[] allCanvasScalers = FindObjectsOfType<CanvasScaler>();

        foreach (CanvasScaler canvasScaler in allCanvasScalers) {
            // 현재 해상도 가로 비율이 더 길 경우
            if (currentAspectRatio > fixedAspectRatio) {
                canvasScaler.matchWidthOrHeight = 1;
            }
            // 현재 해상도의 세로 비율이 더 길 경우
            else if (currentAspectRatio < fixedAspectRatio) {
                canvasScaler.matchWidthOrHeight = 0;
            }
            else {
                canvasScaler.matchWidthOrHeight = 0.5f;
            }
        }
    }

}




/*
 1. UI Title - Option

 2. 내용
    - 창 조절
    - 선택 시 이미지 위치 수정
    - 사운드 조절

    - Apply : 변경사항 없으면 활성화 되지말것

 3. Button On Click
    - Cancle : Option 창 닫고 Main 창 열기
    - Apply : Setting 적용 ( 창 모드 / 사운드 ) //TODO: 창모드, 사운드 조정 필요함
    - Arrow : 글자 변경되야함 => 창 모드에 따라 Arrow가 눌릴지 안눌릴지 결정되어야함

4. Button On Enter //TODO: 활성화 해야함
    - Cancle 
    - Apply 
 */