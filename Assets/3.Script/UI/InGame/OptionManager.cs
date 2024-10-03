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
    private ScreenMode currentScreenMode;
    private ScreenMode selectScreenMode;

    // 해상도 변경
    private List<int[]> screenSizeList = new List<int[]>
    {
        //new int[] { 960, 540 },
        //new int[] { 1024, 576 },
        new int[] { 1152, 648 },
        new int[] { 1366, 768 },
        new int[] { 1280, 720 },
        new int[] { 1600, 900 },
        new int[] { 1920, 1080 }
    };

    //Default 해상도 비율
    private float fixedAspectRatio = 1920f / 1080f;

    private int selectScreenSizeIndex = 0;
    private int[] selectScreenSize = new int[2]; // 사용자 설정 너비
    private int[] currentScreenSize = new int[2]; // 현재 화면 설정 너비

    private Image BGMScrollValue;
    private Image SFXScrollValue;
    private RectTransform BGMrect;
    private RectTransform SFXrect;

    private void Awake() {
        mainGroup = transform.parent.GetChild(0).gameObject;
        windowModeText = transform.GetChild(2).Find("WindowModeValueText").GetComponent<Text>();
        windowSizeText = transform.GetChild(3).Find("ResoultionValueText").GetComponent<Text>();
        selectActive = transform.GetChild(6).Find("ApplyActive").gameObject;

        BGMScrollValue = transform.Find("BGMGroup/Scroll/ScrollDefault").GetComponent<Image>();
        SFXScrollValue = transform.Find("SFXGroup/Scroll/ScrollDefault").GetComponent<Image>();

        BGMrect = transform.Find("BGMGroup/Scroll/ScrollButton").GetComponent<RectTransform>();
        SFXrect = transform.Find("SFXGroup/Scroll/ScrollButton").GetComponent<RectTransform>();

        currentScreenMode = Save.instance.GameData.ScreenMode;
        currentScreenSize = Save.instance.GameData.ScreenSize;
        selectScreenSize = currentScreenSize;
        selectScreenMode = currentScreenMode;

        windowModeText.text = (string)Enum.GetName(typeof(ScreenMode), currentScreenMode);
        windowSizeText.text = string.Format($"{currentScreenSize[0]} * {currentScreenSize[1]}");
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

    private void OnEnable() {
        BGMrect.anchoredPosition = AudioManager.instance.BGMPosition;
        SFXrect.anchoredPosition = AudioManager.instance.SFXPosition;
        BGMScrollValue.fillAmount = AudioManager.instance.BGMValue;
        SFXScrollValue.fillAmount = AudioManager.instance.SFXValue;
    }

    // 사이즈 혹은 모드가 변경됬는지 확인
    private bool CheckSelectModeChange() {
        if (currentScreenMode != selectScreenMode || selectScreenSize != currentScreenSize) {
            return true;
        }
        return false;
    }

    // 창모드 화면 사이즈 결정 해서 버튼 눌릴 경우 변경
    public void ButtonOnClick_ScreenMode(bool RightArrow) {

        int maxCount = Enum.GetValues(typeof(ScreenMode)).Length;

        if (RightArrow) {
            selectScreenMode = (ScreenMode)(((int)selectScreenMode + 1) % maxCount);
        }
        else {
            selectScreenMode = (ScreenMode)(((int)selectScreenMode - 1 + maxCount) % maxCount);
        }

        windowModeText.text = (string)Enum.GetName(typeof(ScreenMode), selectScreenMode);
    }


    // screen size 설정 버튼 눌릴 경우 변경
    public void ButtonOnClick_ScreenSize(bool RightArrow) {
        int maxCount = screenSizeList.Count;

        if (RightArrow) {
            selectScreenSizeIndex = (selectScreenSizeIndex + 1) % maxCount;
        }
        else {
            selectScreenSizeIndex = (selectScreenSizeIndex - 1 + maxCount) % maxCount;
        }

        selectScreenSize = screenSizeList[selectScreenSizeIndex];
        windowSizeText.text = string.Format($"{selectScreenSize[0]} * {selectScreenSize[1]}");
    }


    public void ButtonOnClick_Cancle() {
        gameObject.SetActive(false);
        mainGroup.SetActive(true);
    }


    //TODO: 창모드, 사운드 조정하는 메소드
    public void ButtonOnClick_Apply() {

        if (selectScreenSizeIndex < 0 || selectScreenSizeIndex >= screenSizeList.Count) {
            Debug.LogError("Invalid resolution index.");
            return;
        }

        int width = screenSizeList[selectScreenSizeIndex][0];
        int height = screenSizeList[selectScreenSizeIndex][1];


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

        StartCoroutine(UpdateScalersNextFrame(width, height));

        currentScreenMode = selectScreenMode;
        currentScreenSize = selectScreenSize;

        Save.instance.SaveWindow(selectScreenMode, selectScreenSize);

        Debug.Log(" Window Mode | " + selectScreenMode + "currentScreenSize | " + currentScreenSize[0] + " | " + currentScreenSize[1]);

    }

    private IEnumerator UpdateScalersNextFrame(float width, float height) {
        // 한 프레임 대기
        yield return null;

        UpdateCanvasScaler();

        // 창 모드가 아닌 경우에만 카메라 스케일 업데이트
        if (selectScreenMode == ScreenMode.FullScreen || selectScreenMode == ScreenMode.FullScreenWindow) {
            UpdateCameraScaler(width, height);
        }

    }

    private void UpdateCanvasScaler() {

        //현재 해상도의 비율
        float currentAspectRatio = (float)Screen.width / (float)Screen.height;

        CanvasScaler[] allCanvasScalers = FindObjectsOfType<CanvasScaler>();

        foreach (CanvasScaler canvasScaler in allCanvasScalers) {

            // 비율에 따라 matchWidthOrHeight 값을 계산
            float ratioDifference = currentAspectRatio / fixedAspectRatio;

            // 비율이 동일한 경우
            if (Mathf.Approximately(currentAspectRatio, fixedAspectRatio)) {
                canvasScaler.matchWidthOrHeight = 0.5f; // 중앙에 맞춤
            }
            else {
                if (currentAspectRatio > fixedAspectRatio) {
                    // 현재 가로 비율이 더 긴 경우, 높이에 맞춤
                    canvasScaler.matchWidthOrHeight = Mathf.Clamp(1 - ratioDifference, 0, 1);
                }
                else {
                    // 현재 세로 비율이 더 긴 경우, 너비에 맞춤
                    canvasScaler.matchWidthOrHeight = Mathf.Clamp(ratioDifference, 0, 1);
                }
            }

        }
    }

    private void UpdateCameraScaler(float width, float height) {
        Camera mainCam = Camera.main;
        if (mainCam != null) {
            Rect rect = mainCam.rect;

            float currentAspectRatio = width / height;
            float scaleHeight = currentAspectRatio / fixedAspectRatio;

            if (scaleHeight < 1.0f) {
                rect.height = scaleHeight;                 // 비율에 맞춰 높이 조정
                rect.y = (1.0f - scaleHeight) / 2.0f;      // 중앙 정렬
            }
            else {
                rect.width = 1.0f / scaleHeight;            // 비율에 맞춰 너비 조정
                rect.x = (1.0f - rect.width) / 2.0f;        // 중앙정렬
            }

            mainCam.rect = rect;
        }
        else {
            Debug.LogWarning("main camera is null");
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