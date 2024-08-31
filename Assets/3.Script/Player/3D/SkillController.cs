using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SkillController : MonoBehaviour {
    private Rigidbody playerRigidbody;

    private Player3DController playerController;

    private Vector3 startSection = Vector3.zero;
    private Vector3 finishSection = Vector3.zero;

    private void Awake() {
        playerRigidbody = GetComponent<Rigidbody>();
        playerController = GetComponent<Player3DController>();
    }

    private void Update() {
        if (playerController.IsTryToUseSkill) {
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
    private void MoveSectionLine(bool up) {         //TODO: 라인 간이로 표시할 수 있는 방법 알아올것
        float direction = up ? 1f : -1f;
        
        startSection = new Vector3(playerRigidbody.position.x, playerRigidbody.position.y, (int)playerRigidbody.position.z - direction * 1f);

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

    // 같은 선상에 있으면 해당 오브젝트 담아야함


}

/*
 1. Player3DController에서 스킬 시전 할 경우를 받아서 
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