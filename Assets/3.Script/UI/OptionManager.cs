using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager : MonoBehaviour {
    private GameObject mainGroup;

    private void Awake() {
        mainGroup = transform.parent.GetChild(0).gameObject;
    }

    //TODO: 창모드 화면 사이즈 결정 해서 버튼 눌릴 경우 변경되어야함
    public void ButtonOnClick_LeftArrow() {

    }
    public void ButtonOnClick_RightArrow() {

    }


    public void ButtonOnClick_Cancle() {
        gameObject.SetActive(false);
        mainGroup.SetActive(true);
    }

    public void ButtonOnClick_Apply() {
        //TODO: 창모드, 사운드 조정하는 메소드
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