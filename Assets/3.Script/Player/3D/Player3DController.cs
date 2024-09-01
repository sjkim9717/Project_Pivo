using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3DController : MonoBehaviour {

    public float moveSpeed = 5f;


    private bool isDead = false;

    public bool IsClimb;
    public bool IsMove { get; private set; }

    private Animator ani3D;
    private Rigidbody playerRigid;
    private PlayerManager playerManager;
    private Obstacle3DCheck obstacleCheck;
    private SkillController skillController;

    private Vector3 positionToMove = Vector3.zero;

    private void Awake() {
        playerManager = transform.parent.GetComponent<PlayerManager>();
        obstacleCheck = GetComponent<Obstacle3DCheck>();
        skillController = GetComponent<SkillController>();
        playerRigid = GetComponent<Rigidbody>();

        ani3D = GetComponentInChildren<Animator>();
    }

    private void Start() {
        PlayerManager.PlayerDead += Dead;
    }

    private void Update() {

        if (playerManager.IsMovingStop) {
            IsMove = false;
            return;
        }

        if (!IsClimb) Move();

        if (!IsMove) Climb();

        // Dead() 메서드가 호출된 후 애니메이션이 끝났는지 확인
        if (isDead && IsAnimationFinished()) {
            gameObject.SetActive(false); // 오브젝트 비활성화
        }
    }

    private void Dead() {
        ani3D.SetTrigger("IsDie");
        isDead = true;

    }

    private bool IsAnimationFinished() {
        // 현재 애니메이션 상태 정보 가져오기
        AnimatorStateInfo stateInfo = ani3D.GetCurrentAnimatorStateInfo(0);

        // "IsDie" 애니메이션 상태가 완료되었는지 확인
        return stateInfo.IsName("IsDie") && stateInfo.normalizedTime >= 1.0f;
    }

    private void Move() {
        if (skillController.IsTryToUseSkill) return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        positionToMove = Vector3.zero;
        IsMove = (horizontalInput != 0 || verticalInput != 0);

        if (horizontalInput != 0 || verticalInput != 0) playerManager.isChangingModeTo3D = false;

        Vector3 dir = new Vector3(horizontalInput, 0, verticalInput);

        if (horizontalInput != 0) {       // 오른쪽
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * moveSpeed);
            positionToMove = Vector3.right * moveSpeed * horizontalInput * Time.deltaTime;
        }
        else if (verticalInput != 0) {        // 앞쪽
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * moveSpeed);
            positionToMove = Vector3.forward * moveSpeed * verticalInput * Time.deltaTime;
        }

        // Animation
        ani3D.SetBool("IsMove", IsMove);

        if (IsMove) {
            playerRigid.MovePosition(playerRigid.position + positionToMove);
        }
        else {
            playerRigid.velocity = Vector3.zero; // Stop moving when no input
        }

    }

    private void Climb() {
        if (skillController.IsTryToUseSkill) return;

        float climbInput = Input.GetAxis("Climb");

        if (climbInput != 0 && !IsClimb) {

            if (obstacleCheck.CheckClimbPointsEmpty() && !IsClimb) {
                IsClimb = true;
                ani3D.SetTrigger("IsClimb");
            }
        }
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
        1. 플레이어 바닥의 오브젝트가 있는지 확인 
        2. 없을 경우만 
            - 일정시간동안 일시정지 
            - 2D로 돌아갈건지 떨어질건지 확인
            - 시간이 다되면 자동으로 떨어지기 선택

 3. 변수
    - 스킬 사용 여부 : 카메라, 맵, 오브젝트에서 View 변경 되어야함

 4. 
 
 */