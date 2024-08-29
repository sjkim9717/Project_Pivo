using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3DController : MonoBehaviour {

    public float moveSpeed = 3f;
    private int skillCount = 0;

    private bool IsMove;
    private bool isClimb;
    private bool isTryToUseSkill;  // skill 사용하려고 할 경우 섹션 표시 및 사용 가능인지 불가능인지 확인
    private bool isSkillButtonPressed = false;


    private Animator ani3D;
    private PlayerManager playerManager;
    private Obstacle3DCheck obstacleCheck;

    private Vector3 positionToMove = Vector3.zero;

    private void Awake() {
        playerManager = transform.parent.GetComponent<PlayerManager>();

        obstacleCheck = GetComponent<Obstacle3DCheck>();

        ani3D = GetComponentInChildren<Animator>();
    }

    private void Update() {
        if (!isClimb) {
            Move();
        }

        if (!IsMove) Climb();

        if (Input.GetKeyDown(KeyCode.E)) {
            Debug.Log("WOW");
            transform.position = new Vector3(0, 10f, 0);
        }
    }

    private void FixedUpdate() {
        if (!IsMove && !isClimb) Skill();
    }


    private void Move() {
        if (isTryToUseSkill) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        positionToMove = Vector3.zero;
        IsMove = (horizontalInput != 0 || verticalInput != 0);

        if (horizontalInput != 0) {       // 오른쪽
            float moveDirection = horizontalInput > 0 ? 1f : 0f;
            transform.rotation = Quaternion.Euler(0f, moveDirection * 180f, 0f);
            positionToMove -= Vector3.forward * moveSpeed * horizontalInput * Time.deltaTime;
        }
        else if (verticalInput != 0) {        // 앞쪽
            float moveDirection = verticalInput > 0 ? -1f : 1f;
            transform.rotation = Quaternion.Euler(0f, -moveDirection * 90, 0f);
            positionToMove += Vector3.right * moveSpeed * verticalInput * Time.deltaTime;
        }

        // Animation
        ani3D.SetBool("IsMove", IsMove);

        if (IsMove) {
            transform.position += positionToMove;
        }

    }

    private void Climb( ) {
        if (isTryToUseSkill) return;

        float climbInput = Input.GetAxis("Climb");

        if (climbInput != 0) {

            if (obstacleCheck.CheckClimbPointsEmpty()) {
                //TODO: climb시 플레이어가 이동해야함
                isClimb = true; 
                ani3D.SetTrigger("IsClimb"); 
            }
        }
        isClimb = false;
    }


    //TODO: 스킬 사용 구간과 플레이어가 겹치는지 확인해야함 
    private void Skill() {
        float skillSectionInput = Input.GetAxis("SkillSection");

        if (skillSectionInput != 0 && !isSkillButtonPressed) {                      // 스킬 버튼이 눌렸는지 감지
            isSkillButtonPressed = true;                                            // 버튼이 눌린 상태로 표시
            skillCount++;
            isTryToUseSkill = true;
            Debug.Log("스킬 시도 등록. 현재 스킬 횟수: " + skillCount);
        }


        if (skillCount >= 2) {                                                      // 스킬 사용 시도 횟수가 2회 이상인지 확인
            if (CheckSkillUsable()) {                                               //TODO: [기억] 스킬 사용해서 2D로 변경됨
                playerManager.SetPlayerMode(false);
                playerManager.SwitchMode();
                Debug.Log("2D 모드로 전환됨");
            }
            else {                                                                  // 3D 모드 유지
                playerManager.SetPlayerMode(true);
            }
            skillCount = 0;                                                         // 스킬 시도 후 시도 횟수 초기화
            isTryToUseSkill = false;
        }

        ani3D.SetBool("IsTryUseSkill", isTryToUseSkill);                            // 애니메이션 상태 처리

        if (skillSectionInput == 0 && isSkillButtonPressed) {                           // 스킬 버튼이 해제되었는지 감지
            isSkillButtonPressed = false;                                               // 버튼 눌림 상태를 초기화
        }
    }


    private bool CheckSkillUsable() {                                                   //TODO: 플레이어가 스킬 자르면 해당하는 영역을 확인해야함
        return true;
    }


    // 아래 방향 확인해서 없으면? 떨어짐 
    private void CheckPlayerFalling() {

    }
}

/*
 1. 플레이어 : 이동 x, 모드 변경
 
 2. 내용
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

 3. 변수
    - 스킬 사용 여부 : 카메라, 맵, 오브젝트에서 View 변경 되어야함

 4. 
 
 */