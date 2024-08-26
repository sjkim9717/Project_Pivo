using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    public float moveSpeed = 3f;
    private int skillCount = 0;
    public bool Is3DPlayer { get; private set; }    // player가 skill을 사용완료하여 3D에서 2D로 변경되어야하는 경우
    public bool IsDie { get; private set; }     // 죽는 거를 여기서 확인하는게 맞나?
    public bool IsMove { get; private set; }
    public bool IsClimb { get; private set; }
    public bool IsTryToUseSkill { get; private set; }     // skill 사용하려고 할 경우 섹션 표시 및 사용 가능인지 불가능인지 확인

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
        if(!IsClimb) Move(Is3DPlayer);

        if (!IsMove) Climb(Is3DPlayer);
        /*
                 if (player3D.activeSelf) {
            position = player3D.transform.position;
            transform.position = position;
        }
        else {
            position = player2D.transform.position;
            transform.position = position;
        }
         
         */


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
                transform.localScale = new Vector3(1f, 1f, moveDirection);
                transform.rotation = Quaternion.Euler(0f, -moveDirection * 90, 0f);
                positionToMove -= Vector3.forward * moveSpeed * horizontalInput * Time.deltaTime;
            }
            else if (verticalInput != 0) {        // 앞쪽
                float moveDirection = verticalInput > 0 ? 1f : -1f;
                transform.localScale = new Vector3(moveDirection, 1f, 1f);
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                positionToMove += Vector3.right * moveSpeed * verticalInput * Time.deltaTime;
            }

            // Animation
            if (IsMove) ani3D.SetBool("IsMove", true);
            else ani3D.SetBool("IsMove", false);
        }
        else {
            IsMove = (horizontalInput != 0 );
            
            if (horizontalInput != 0) {       // 오른쪽 키를 입력받아 2D에서는 앞 뒤로만 이동
                float moveDirection = horizontalInput > 0 ? 1f : -1f;
                transform.localScale = new Vector3(moveDirection, 1f, 1f);
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                positionToMove += Vector3.right * moveSpeed * horizontalInput * Time.deltaTime;
            }

            // Animation
            if (IsMove) ani2D.SetBool("IsMove", true);
            else ani2D.SetBool("IsMove", false);
        }

        if (CheckObstacleInFrontOfPlayer(Is3DPlayer)) return;
        transform.position += positionToMove;
    }


    // player 앞에 장애물이 있는지 콜라이더로 부딪혔는지 확인 -> bool 움직임 제약

    private bool CheckObstacleInFrontOfPlayer(bool Is3DPlayer) {
        if (Is3DPlayer) {                                      // 3D일 경우 x축이 0이상일 경우가 앞

            Ray ray = new Ray(player3D.transform.position, player3D.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 20f)) {

                Debug.DrawRay(player3D.transform.position, player3D.transform.forward * 20f, Color.red);
                if (hit.collider != null) {
                    Debug.Log("Climb Raycast hit | " + hit.collider.name);
                    return true;
                }
            }
        }
        else {                                                   // 2D일 경우 z축이 0이상일 경우가 앞

            Ray2D ray = new Ray2D(player2D.transform.position, player2D.transform.right);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 20f);
            Debug.DrawRay(player2D.transform.position, player2D.transform.forward * 20f, Color.blue);
            if (hit.collider != null) {
                return true;
            }
        }
        return false;
    }


    private void Climb(bool Is3DPlayer) {
        if (IsTryToUseSkill) return;

        float climbInput = Input.GetAxis("Climb");

        if (climbInput != 0) {

            if (CheckObstacleInFrontOfPlayer(Is3DPlayer)) {
                IsClimb = true;
                if (Is3DPlayer) ani3D.SetTrigger("IsClimb");
                else ani2D.SetTrigger("IsClimb");
            }
        }
        IsClimb = false;
    }

    // 3D player 상태에서 skill 키를 사용했을 경우
    private void Skill(bool Is3DPlayer) {
        float skillSectionInput = Input.GetAxis("SkillSection"); // 

        if (Is3DPlayer) {
            if (skillSectionInput != 0) {
                skillSectionInput = 0;
                skillCount++;
                IsTryToUseSkill = true;
                Debug.Log("아니 왜지?");
            }

            if (skillCount >= 2) {
                if (true) {                                         //TODO: 스킬 사용 구간과 플레이어가 겹치는지 확인해야함
                    Is3DPlayer = false;
                    player3D.SetActive(false);
                    player2D.SetActive(true);
                    //TODO: [기억] 스킬 사용해서 2D로 변경됨
                    Debug.Log("2D로 변경되어야하는 시점");
                }
                else {                                                 // 스킬 꺼짐                
                    Is3DPlayer = true;
                    player3D.SetActive(true);
                    player2D.SetActive(false);
                }
                skillCount = 0;
                IsTryToUseSkill = false;
            }

            // Animation
            if (IsTryToUseSkill) ani3D.SetBool("IsTryUseSkill", true);
            else ani3D.SetBool("IsTryUseSkill", false);
        }
        else {

            if (skillSectionInput != 0) {
                Debug.Log("3D로 변경되어야하는 시점");
                skillCount = 0;
                Is3DPlayer = true;
                player3D.SetActive(true);
                player2D.SetActive(false);
            }
        }

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

