using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2DController : MonoBehaviour {

    public float moveSpeed = 3f;
    private int skillCount = 0;
    private bool IsMove;
    public bool isClimb { get; private set; }

    private bool isSkillButtonPressed = false;

    private Animator ani2D;
    private PlayerManager playerManager;

    private Vector3 positionToMove = Vector3.zero;

    // 장애물이 앞쪽에 있을 경우
    private bool isObstacleFrontPlayer;

    // 바닥이 있는지 없는지 확인
    private bool isFloorExist;
    public Vector3 Playerpos { get; private set; }
    private Vector3 obstaclepos;


    private void Awake() {
        playerManager = transform.parent.GetComponent<PlayerManager>();

        ani2D = GetComponent<Animator>();
    }

    private void Update() {
        if (!isClimb) {
            Move();
        }

        if (!IsMove) Climb();
    }

    private void FixedUpdate() {
        if (!IsMove && !isClimb) Skill();

    }
    private void OnCollisionStay2D(Collision2D collision) {
        obstaclepos = collision.transform.parent != null ? collision.transform.parent.position : collision.transform.position;

        Vector3 playerToObstacle = obstaclepos - Playerpos;

        Vector3 directionFront = transform.InverseTransformDirection(playerToObstacle);
        float d = Vector3.Dot(directionFront, Vector3.right);

        if (d >= 0) {   // 장애물이 플레이어 앞쪽에 있을 경우 

            //TODO: 장애물 bool 넘겨야함
            isObstacleFrontPlayer = true;

            float midpointY = transform.position.y;
            float collisionY = collision.transform.position.y;
        }

        Vector3 directionBottom = transform.InverseTransformDirection(playerToObstacle);
        float b = Vector3.Dot(directionBottom, Vector3.down);
        if (b >= 0) {       // 바닥 오브젝트가 있다면
            //Debug.Log("Object stay from bottom: " + collision.transform.parent.position);
            isFloorExist = true;
        }
    }

    private void Move() {

        float horizontalInput = Input.GetAxis("Horizontal");

        positionToMove = Vector3.zero;

        IsMove = (horizontalInput != 0);

        if (horizontalInput != 0) {       // 오른쪽 키를 입력받아 2D에서는 앞 뒤로만 이동
            float moveDirection = horizontalInput > 0 ? 1f : -1f;
            transform.localScale = new Vector3(moveDirection, 1f, 1f);
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            positionToMove += Vector3.right * moveSpeed * horizontalInput * Time.deltaTime;
        }

        // Animation
        ani2D.SetBool("IsMove", IsMove);

        if (IsMove) {
            transform.position += positionToMove;
        }

    }

    private void Climb() {
        float climbInput = Input.GetAxis("Climb");

        if (climbInput != 0) {

            if (/*장애물 bool 받아야함*/ true) {
                isClimb = true;
                ani2D.SetTrigger("IsClimb");
            }
        }
        isClimb = false;
    }


    //TODO: 스킬 사용 구간과 플레이어가 겹치는지 확인해야함 
    private void Skill() {
        float skillSectionInput = Input.GetAxis("SkillSection");

        if (skillSectionInput != 0 && !isSkillButtonPressed) {                       // 2D 모드에서 스킬 버튼 입력 감지
            isSkillButtonPressed = true;                                             // 버튼이 눌린 상태로 표시
            Debug.Log("3D 모드로 전환됨");
            playerManager.SetPlayerMode(true);
            playerManager.SwitchMode();
        }

        if (skillSectionInput == 0 && isSkillButtonPressed) {                           // 스킬 버튼이 해제되었는지 감지
            isSkillButtonPressed = false;                                               // 버튼 눌림 상태를 초기화
        }

    }



    private bool CheckSkillUsable() {                                                   //TODO: 플레이어가 스킬 자르면 해당하는 영역을 확인해야함
        return true;
    }


    // 아래 방향 확인해서 없으면? 떨어짐 
    private void CheckPlayerFalling(bool Is3DPlayer) {
        Ray2D ray = new Ray2D(transform.position, -transform.up);
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
