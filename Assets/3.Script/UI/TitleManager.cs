using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    private GameObject mainGroup;
    private GameObject optionGroup;
    private GameObject newGameGroup;

    public GameObject Default;
    public GameObject Select;

    private void Awake() {
        mainGroup = transform.GetChild(0).gameObject;
        optionGroup = transform.GetChild(1).gameObject;
        newGameGroup = transform.GetChild(2).gameObject;
    }

    private void Start() {
        //TODO: 파일있는지 확인해서 없으면 Load Game button 잠금
        /*
        if (!Save.instance.GetSaveExist()) {
            Default.transform.GetChild(1).gameObject.SetActive(false);
        }
        else {
            Default.transform.GetChild(1).gameObject.SetActive(true);
        }
        */  

    }

    //TODO: escape 시 각종 UI 원위치
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) Escape();
    }



    // on pointer event

    public void ButtonOnEnter_Main(int num) {
        for (int i = 0; i < Select.transform.childCount; i++) {
            if(i == num) Select.transform.GetChild(i).gameObject.SetActive(true);
            else Select.transform.GetChild(i).gameObject.SetActive(false);
        }
    }


    // on click event

    public void ButtonOnClick_NewGame() {
        if (Save.instance.GetSaveExist()) {
            mainGroup.SetActive(false);
            newGameGroup.SetActive(true);
        }
        else {
            //TODO: play 시작
            gameObject.SetActive(false);
        }
    }
    public void ButtonOnClick_LoadGame() {
        SceneManager.LoadScene("StageSelect_Grass");
        //TODO: StageSelect_Grass에서 플레이어가 서있는 위치 조정 필요함
    }

    public void ButtonOnClick_Option() {
        mainGroup.SetActive(false);
        optionGroup.SetActive(true);
    }

    public void ButtonOnClick_Exit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // escape => Option, new Game 등 한 번에 조정
    public void Escape() {
        if (optionGroup.activeSelf || newGameGroup.activeSelf) {
            optionGroup.SetActive(false);
            newGameGroup.SetActive(false);
            mainGroup.SetActive(true);
        }
        else {
            if(mainGroup.activeSelf) mainGroup.SetActive(false);
        }
    }

}

/*
 1. UI_Title
 2. 버튼 클릭 이벤트
 3. 내용
    - New Game
        1. 파일있는지 확인
    - Load Game
    - Option
        1. Option 창 열기
    - Exit
 4. 추가해야하는 사항 
    - 마우스가 빠져나갔을 경우 확인해서 off 시킬 것
    - 키보드로 Select활성화 될 수 있는 것 만들 것
 */