using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager_Old : MonoBehaviour {

    public float moveSpeed = 3f;
    private int skillCount = 0;
    public bool Is3DPlayer { get; private set; }    // player가 skill을 사용완료하여 3D에서 2D로 변경되어야하는 경우
    public bool IsDie { get; private set; }     // 죽는 거를 여기서 확인하는게 맞나?
    private bool IsMove;
    public bool IsClimb { get; private set; }
    public bool IsTryToUseSkill { get; private set; }     // skill 사용하려고 할 경우 섹션 표시 및 사용 가능인지 불가능인지 확인

    private bool isSkillButtonPressed = false;

    private GameObject player3D;
    private GameObject player2D;

    private Animator ani3D;
    private Animator ani2D;

    private Vector3 positionToMove = Vector3.zero;

    private void Awake() {
        player3D = transform.GetChild(0).gameObject;
        player2D = transform.GetChild(1).gameObject;

        ani3D = player3D.GetComponentInChildren<Animator>();
        ani2D = player2D.GetComponent<Animator>();
        Is3DPlayer = true;
    }

    private void Update() {
        if (!IsClimb) {
            Move(Is3DPlayer);
        }

        if (!IsMove) Climb(Is3DPlayer);
    }

    private void FixedUpdate() {
        if (!IsMove && !IsClimb) Skill(Is3DPlayer);

    }

    private void Move(bool Is3DPlayer) {
        if (IsTryToUseSkill) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        //TODO: [수정 필요함] 물체가 플레이어 앞에 있을 경우 움직이면 안됨 -> 방향 번경은 되어야함 / 위치 이동은 되면 안됨
        positionToMove = Vector3.zero;

        if (Is3DPlayer) {
            IsMove = (horizontalInput != 0 || verticalInput != 0);

            if (horizontalInput != 0) {       // 오른쪽
                float moveDirection = horizontalInput > 0 ? -1f : 1f;
                transform.rotation = Quaternion.Euler(0f, -moveDirection * 90, 0f);
                positionToMove -= Vector3.forward * moveSpeed * horizontalInput * Time.deltaTime;
            }
            else if (verticalInput != 0) {        // 앞쪽
                float moveDirection = verticalInput > 0 ? 0f : 1f;
                transform.rotation = Quaternion.Euler(0f, moveDirection * 180f, 0f);
                positionToMove += Vector3.right * moveSpeed * verticalInput * Time.deltaTime;
            }

            // Animation
            ani3D.SetBool("IsMove", IsMove);
        }
        else {
            IsMove = (horizontalInput != 0);

            if (horizontalInput != 0) {       // 오른쪽 키를 입력받아 2D에서는 앞 뒤로만 이동
                float moveDirection = horizontalInput > 0 ? 1f : -1f;
                transform.localScale = new Vector3(moveDirection, 1f, 1f);
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                positionToMove += Vector3.right * moveSpeed * horizontalInput * Time.deltaTime;
            }

            // Animation
            ani2D.SetBool("IsMove", IsMove);
        }


        if (IsMove) {
            if (SetChildCollisionSetting()) {
                transform.position += positionToMove;

            }
        }

    }

    private bool SetChildCollisionSetting() {
        GameObject player;
        if (Is3DPlayer) player = player3D;
        else player = player2D;

        CollisionCheck collisionCheck = player.GetComponent<CollisionCheck>();
        Debug.Log("CheckTrasformMove 들어오나?");
        if (!collisionCheck.GetIsObstacleFrontPlayer()) return true;
        else return false;
    }



    private void Climb(bool Is3DPlayer) {
        if (IsTryToUseSkill) return;

        float climbInput = Input.GetAxis("Climb");

        if (climbInput != 0) {

            if (/*장애물 bool 받아야함*/ true) {
                IsClimb = true;
                if (Is3DPlayer) ani3D.SetTrigger("IsClimb");
                else ani2D.SetTrigger("IsClimb");
            }
        }
        IsClimb = false;
    }


    //TODO: 스킬 사용 구간과 플레이어가 겹치는지 확인해야함 
    private void Skill(bool Is3DPlayer) {
        float skillSectionInput = Input.GetAxis("SkillSection");

        if (Is3DPlayer) {                                                               // 3D player 상태에서 skill 키를 사용했을 경우

            if (skillSectionInput != 0 && !isSkillButtonPressed) {                      // 스킬 버튼이 눌렸는지 감지
                isSkillButtonPressed = true;                                            // 버튼이 눌린 상태로 표시
                skillCount++;
                IsTryToUseSkill = true;
                Debug.Log("스킬 시도 등록. 현재 스킬 횟수: " + skillCount);
            }


            if (skillCount >= 2) {                                                      // 스킬 사용 시도 횟수가 2회 이상인지 확인
                if (CheckSkillUsable()) {                                               //TODO: [기억] 스킬 사용해서 2D로 변경됨
                    SwichTo2DMode();
                    Debug.Log("2D 모드로 전환됨");
                }
                else {                                                                  // 3D 모드 유지
                    SwitchTo3DMode();
                }
                skillCount = 0;                                                         // 스킬 시도 후 시도 횟수 초기화
                IsTryToUseSkill = false;
            }

            ani3D.SetBool("IsTryUseSkill", IsTryToUseSkill);                            // 애니메이션 상태 처리
        }

        else {                                                                           // 플레이어가 2D 모드인 경우
            if (skillSectionInput != 0 && !isSkillButtonPressed) {                       // 2D 모드에서 스킬 버튼 입력 감지
                isSkillButtonPressed = true;                                             // 버튼이 눌린 상태로 표시
                Debug.Log("3D 모드로 전환됨");
                SwitchTo3DMode();
            }
        }


        if (skillSectionInput == 0 && isSkillButtonPressed) {                           // 스킬 버튼이 해제되었는지 감지
            isSkillButtonPressed = false;                                               // 버튼 눌림 상태를 초기화
        }

    }

    private void SwitchTo3DMode() {
        Is3DPlayer = true;
        player3D.SetActive(true);
        player2D.SetActive(false);
    }

    private void SwichTo2DMode() {
        Is3DPlayer = false;
        player3D.SetActive(false);
        player2D.SetActive(true);
    }

    private bool CheckSkillUsable() {                                                   //TODO: 플레이어가 스킬 자르면 해당하는 영역을 확인해야함
        return true;
    }


    // 아래 방향 확인해서 없으면? 떨어짐 
    private void CheckPlayerFalling(bool Is3DPlayer) {
        if (Is3DPlayer) {

            Ray ray = new Ray(player3D.transform.position, -player3D.transform.up);
            if (Physics.Raycast(ray, out RaycastHit hit, 20f)) {
                if (hit.collider == null) {
                    // Animation
                    ani3D.SetTrigger("IsFalling");
                }
                else {
                    Debug.Log("Falling Raycast hit | " + hit.collider.name);
                }
            }
        }
        else {

            Ray2D ray = new Ray2D(player2D.transform.position, -player3D.transform.up);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 20f);
            if (hit.collider == null) {
                // Animation
                ani2D.SetTrigger("IsFalling");
            }
            else {
                Debug.Log("Falling Raycast hit | " + hit.collider.name);
            }
        }
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

