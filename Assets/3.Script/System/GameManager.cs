using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance { get; private set; }


    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }


    private void Option() {
    }

    //TODO: stage level 선택씬에서 연결할 것
    public void OnButtonClick_LevelChoose(StageLevel stageLevel) {

    }

}

/*
 1. 씬 로드시에 해당 레벨 들고있을 것
    - if : 레벨 선택 씬이라면 => 버튼에 씬 레벨 각각 넣을 것
 2. 스테이지 클리어시 save의 Data호출
    - 호출된 Data에 해당 레벨 클리어 bool값 + count 넣을 것 
 
 */