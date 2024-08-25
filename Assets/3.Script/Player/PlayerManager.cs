using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    public float moveSpeed = 3f;
    private int skillCount = 0;
    public bool Is3DPlayer { get; private set; }    // player가 skill을 사용완료하여 3D에서 2D로 변경되어야하는 경우
    public bool IsPlayerMove { get; private set; }
    public bool IsPlayerClimb { get; private set; }
    public bool IsPlayerTryToUseSkill { get; private set; }     // skill 사용하려고 할 경우 섹션 표시 및 사용 가능인지 불가능인지 확인

    private GameObject player2D;
    private GameObject player3D;

    private void Awake() {
        player3D = transform.GetChild(0).gameObject;
        player2D = transform.GetChild(1).gameObject;
    }

    private void Update() {
        if(!IsPlayerClimb ) Move();
        if (!IsPlayerMove) Climb();
        if (!IsPlayerMove && !IsPlayerClimb) {
            if (Is3DPlayer) Skill_3DPlayer();
            else Skill_2DPlayer();
        }
    }

    private void Move() {

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput != 0 || verticalInput != 0) IsPlayerMove = true;
        else IsPlayerMove = false;

        if (horizontalInput != 0) {       // 오른쪽
            float moveDirection = horizontalInput > 0 ? 1f : -1f;
            transform.localScale = new Vector3(-moveDirection, 1f, 1f);
            transform.position += Vector3.right * moveSpeed * horizontalInput * Time.deltaTime;
        }

        if (verticalInput != 0) {        // 위쪽
            float moveDirection = verticalInput > 0 ? 1f : -1f;
            transform.localScale = new Vector3(-moveDirection, 1f, 1f);
            transform.position += Vector3.up * moveSpeed * horizontalInput * Time.deltaTime;
        }
    }

    private void Climb() {
        float climbInput = Input.GetAxis("Climb");
        if(climbInput>0) {
            if (true) {                                        //TODO: 플레이어 앞에 장애물이 있는지 확인해야함
                IsPlayerClimb = true;
            }
            else IsPlayerClimb = false;
        }

    }

    // 3D player 상태에서 skill 키를 사용했을 경우
    private void Skill_3DPlayer() {
        float skillSectionInput = Input.GetAxis("SkillSection");

        if (skillSectionInput > 0) {
            skillCount++;
            IsPlayerTryToUseSkill = true;           
        }

        if(skillCount >= 2) {
            if (true) {                                         //TODO: 스킬 사용 구간과 플레이어가 겹치는지 확인해야함
                Is3DPlayer = false;
                player3D.SetActive(false);
                player2D.SetActive(true);
            }
            else {                                                 // 스킬 꺼짐                
                Is3DPlayer = true;
                player3D.SetActive(true);
                player2D.SetActive(false);
            }
            skillCount = 0;
            IsPlayerTryToUseSkill = false;
        }
    }

    // 2D player 상태에서 skill 키를 사용했을 경우 3D로 돌아갈 것
    private void Skill_2DPlayer() {
        float skillSectionInput = Input.GetAxis("SkillSection");

        if (skillSectionInput > 0) {
            Is3DPlayer = true;
            player3D.SetActive(true);
            player2D.SetActive(false);
        }
    }


    // respawn
    private void CheckPlayerFalling() {

    }
}

/*
 1. 플레이어 2D, 3D변경
 
 2. 내용
    - 키 입력받으면 tranform으로 2D, 3D 같이 이동
    - 기본 3D로 시작
    - 스킬 사용 시 2D로 변경

 3. 변수
    - 스킬 사용 여부 : 카메라, 맵, 오브젝트에서 View 변경 되어야함

 4. 
    - Move
    - Climb
    - 스킬 사용
        1. 스킬 버튼을 누름 skillcount =1
        2. 스킬 사용 중
        3. 스킬 버튼을 한번더 누름 skillcount =2
            - 스킬이 사용가능한지 확인해야함
            - 사용가능하면 스킬 쓰기
            - 사용 불가능하면 스킬 count 초기화
    - respawn
        //TODO: 리스폰 해야함
        1. 플레이어 바닥의 오브젝트가 있는지 확인 
        2. 없을 경우만 
            - 일정시간동안 일시정지 
            - 2D로 돌아갈건지 떨어질건지 확인
            - 시간이 다되면 자동으로 떨어지기 선택
 

//TODO: InputManager 만들어서 키 변경한다면 들고 올 것
 */

