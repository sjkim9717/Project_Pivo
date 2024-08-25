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
        //TODO: �����ִ��� Ȯ���ؼ� ������ Load Game button ���
        /*
        if (!Save.instance.GetSaveExist()) {
            Default.transform.GetChild(1).gameObject.SetActive(false);
        }
        else {
            Default.transform.GetChild(1).gameObject.SetActive(true);
        }
        */  

    }

    //TODO: escape �� ���� UI ����ġ
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
            //TODO: play ����
        }
    }
    public void ButtonOnClick_LoadGame() {
        SceneManager.LoadScene("StageSelect_Grass");
        //TODO: StageSelect_Grass���� �÷��̾ ���ִ� ��ġ ���� �ʿ���
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

    // escape => Option, new Game �� �� ���� ����
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
 2. ��ư Ŭ�� �̺�Ʈ
 3. ����
    - New Game
        1. �����ִ��� Ȯ��
    - Load Game
    - Option
        1. Option â ����
    - Exit
 4. �߰��ؾ��ϴ� ���� 
    - ���콺�� ���������� ��� Ȯ���ؼ� off ��ų ��
    - Ű����� SelectȰ��ȭ �� �� �ִ� �� ���� ��
 */