using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SkillController : MonoBehaviour {
    private PlayerManager playerManager;
    private Vector3 startSection = Vector3.zero;
    private Vector3 finishSection = Vector3.zero;


    private void Awake() {
        playerManager = GetComponent<PlayerManager>();
    }

    private void Update() {
        if (playerManager.IsTryToUseSkill) {
            GetKeyInput();
        }
        else {
            startSection = Vector3.zero;
            finishSection = Vector3.zero;
        }
    }

    // 입력을 받아서 일정 범위 결정
    private void GetKeyInput() {
        // 각 화살표 키가 눌렸는지 확인
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.UpArrow)) {
            MoveSectionLine(true);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.DownArrow)) {
            MoveSectionLine(false);
        }

    }

    // section line 결정하기
    private void MoveSectionLine(bool up) {
        float direction = up ? 1f : -1f;
        
        startSection = new Vector3(transform.position.x, transform.position.y, (int)transform.position.z - direction * 1f);

        if(finishSection == Vector3.zero) {
            finishSection = new Vector3(startSection.x, startSection.y, startSection.z + direction * 2f);
        }
        else {
            finishSection.z += direction * 2f;
            
            float calSectionDirection = direction * (finishSection.z - startSection.z);

            calSectionDirection = Mathf.Clamp(calSectionDirection, 2, 10);

            finishSection.z = startSection.z + calSectionDirection;
        }
    }

    // section Lin에 있는 구간에 절대 좌표 기준으로 좌우

}

/*
 1. PlayerManager에서 스킬 시전 할 경우를 받아서 
 2. 키보드 입력을 받고
    CLAMP?
 3. 위 아래 섹션 중가 

=> MoveSectionLine 메소드
 startSection 벡터 :현재 위치를 중심으로 z-1,x, y는 고정 
 finishSection 벡터 : z+1,x, y고정에서 출발
            z축을 +2단위로 움직임
단 clamp함수를 사용해서 to벡터의 z가 움직일 수 있는 최대 범위(from - to)는 10까지로 제한 


    1. 해당되는 오브젝트들 다 담아놓고
    2. 플레이어와 z축 비교
    3. 같으면 material 변경 빨간색 표시 
    4. 

 
 */