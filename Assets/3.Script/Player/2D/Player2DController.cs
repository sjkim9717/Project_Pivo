using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2DController : MonoBehaviour {

    public float moveSpeed = 5f;

    public bool IsClimb;
    public bool IsMove { get; private set; }

    private bool isSkillButtonPressed = false;

    private Vector3 positionToMove = Vector3.zero;
    
    private Animator ani2D;
    private Rigidbody2D playerRigid;

    private PlayerManager playerManager;

    private HashSet<GameObject> obstacles = new HashSet<GameObject>();


    private void Awake() {
        playerManager = transform.GetComponentInParent<PlayerManager>();
        playerRigid = transform.GetComponent<Rigidbody2D>();

        ani2D = GetComponent<Animator>();
    }

    private void Update() {

        if (playerManager.IsMovingStop) return;

        if (!IsClimb) Move();

        if (!IsMove) Climb();
    }

    private void FixedUpdate() {

        if (playerManager.IsMovingStop) {
            IsMove = false;
            return;
        }

        if (!IsMove && !IsClimb) Skill();

    }
    private void OnCollisionStay2D(Collision2D collision) {
        foreach (ContactPoint2D contact in collision.contacts) {
            if (contact.normal.x != 0 && contact.normal.y == 0) {
                if (collision.transform.parent != null) obstacles.Add(collision.transform.parent.gameObject);
                //if (collision.transform.parent != null) Debug.Log(" 2D collision parent name" + collision.transform.parent.name);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.transform.parent != null) {
            obstacles.Clear();
        }
    }

    private void Move() {
        playerRigid.velocity = new Vector2(0, playerRigid.velocity.y);

        float horizontalInput = Input.GetAxis("Horizontal");

        positionToMove = Vector3.zero;

        IsMove = (horizontalInput != 0);

        if (horizontalInput != 0) {       // 오른쪽 키를 입력받아 2D에서는 앞 뒤로만 이동
            float moveDirection = horizontalInput > 0 ? 1f : -1f;
            transform.localScale = new Vector3(moveDirection, 1f, 1f);
            positionToMove = Vector3.right * moveSpeed * horizontalInput * Time.deltaTime;

        }

        // Animation
        ani2D.SetBool("IsMove", IsMove);

        if (IsMove) {
            playerRigid.MovePosition(playerRigid.position + (Vector2)positionToMove);
        }

    }

    private void Climb() {
        float climbInput = Input.GetAxis("Climb");

        if (climbInput != 0 && !IsClimb) {
            if(obstacles != null) {
                if (!CheckClimbCountOverTwo()) {
                    IsClimb = true;
                    ani2D.SetTrigger("IsClimb");
                }
            }
        }
    }

    // 접점의 모든 오브젝트를 돌아 y값이 차이가 나는지확인
    private bool CheckClimbCountOverTwo() {

        HashSet<float> yPositions = new HashSet<float>();           // y 위치를 저장할 HashSet 리스트

        foreach (GameObject obj in obstacles) {                             // HashSet을 순회하며 y 위치를 리스트에 추가
            if (obj != null) {
                //Debug.Log(obj.name);
                float yPos = obj.transform.position.y;
                yPositions.Add(yPos);
            }
        }
        Debug.Log("2D controller | CheckClimbCountOverTwo | y값이 차이 개수 | " + yPositions.Count);
        return yPositions.Count >= 2;                                   // y 위치가 두 개 이상이면 true 반환
    }


    private void Skill() {
        float skillSectionInput = Input.GetAxis("SkillSection");
        //Debug.Log(" 2d contrller | skillSectionInput | " + skillSectionInput);
        if (skillSectionInput != 0 && !isSkillButtonPressed) {                       // 2D 모드에서 스킬 버튼 입력 감지
            isSkillButtonPressed = true;                                             // 버튼이 눌린 상태로 표시
            Debug.Log("3D 모드로 전환됨");
            FindObjectOfType<ConvertMode>().ChangeLayerAllActiveTrue();

            playerManager.SetPlayerMode(true);
            playerManager.isChangingModeTo3D = true;
            playerManager.SwitchMode();
        }

        if (skillSectionInput == 0 && isSkillButtonPressed) {                           // 스킬 버튼이 해제되었는지 감지
            isSkillButtonPressed = false;                                               // 버튼 눌림 상태를 초기화
        }

    }


}

