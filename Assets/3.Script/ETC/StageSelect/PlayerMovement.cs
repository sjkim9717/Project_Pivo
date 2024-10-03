using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class PlayerMovement : MonoBehaviour {
    private StageLevel currentLevel;
    private Waypoint waypoint;

    private Animator playerAni;

    private Rigidbody playerRigid;
    private Transform nextWaypoint;
    private Transform climbTransfrom;
    private GameObject groundPoint;


    public float moveSpeed = 5f; // 이동 속도
    private float gravity = -9.8f;
    private bool isMoving = false; // 플레이어가 이동 중인지 여부


    private void Awake() {
        waypoint = FindObjectOfType<Waypoint>();

        playerRigid = GetComponent<Rigidbody>();
        playerAni = GetComponentInChildren<Animator>();
        groundPoint = transform.GetChild(1).gameObject;
        climbTransfrom = transform.GetChild(2);
    }
    private void Start() {
        currentLevel = GameManager.instance.PreviousGameStage;
        Debug.LogWarning($"PlayerMovement loaded currentLevel : {currentLevel}");
        transform.position = waypoint.FindCurrentPosition(currentLevel).position;
    }

    private void Update() {

        if (!isMoving) {
            // 4방향 입력을 처리
            if (Input.GetKeyDown(KeyCode.UpArrow)) CheckDirection(Direction.Up);
            if (Input.GetKeyDown(KeyCode.DownArrow)) CheckDirection(Direction.Down);
            if (Input.GetKeyDown(KeyCode.LeftArrow)) CheckDirection(Direction.Left);
            if (Input.GetKeyDown(KeyCode.RightArrow)) CheckDirection(Direction.Right);
        }
        else {
            playerRigid.velocity = Vector3.zero;
        }

    }



    // 입력받은 방향에 해당하는 경로로 이동 가능한지 체크
    private void CheckDirection(Direction direction) {
        // 해당 방향으로 이동할 수 있는지 확인하고 이동
        Debug.Log("현재 스테이지 | " + currentLevel);
        Debug.Log("현재 스테이지 Direction | " + direction);
        if (waypoint.CanMoveTo(currentLevel, direction, out StageLevel selectLevel)) {
            if (waypoint.CanEnterTo(currentLevel, selectLevel, ref nextWaypoint)) {
                StartCoroutine(MoveToNextWaypoint_Co(nextWaypoint));
                currentLevel = selectLevel;
            }
        }
    }


    private IEnumerator MoveToNextWaypoint_Co(Transform nextWaypoint) {
        playerAni.SetBool("IsMove", true);
        isMoving = true;
        Vector3 startPosition = playerRigid.position;
        Vector3 endPosition = new Vector3(nextWaypoint.position.x, playerRigid.position.y, nextWaypoint.position.z);
        float elapsedTime = 0f;
        float moveDuration = 1f;

        Quaternion startRotation = playerRigid.rotation;                    // 현재 방향 저장

        Vector3 direction = (endPosition - startPosition).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        playerRigid.MoveRotation(targetRotation);

        while (elapsedTime < moveDuration) {

            // 오르막 모션
            if (Physics.Raycast(climbTransfrom.position, transform.forward, out RaycastHit hit, 1f)) {
                if (hit.collider.CompareTag("Climb")) {
                    playerRigid.velocity = Vector3.zero;

                    Debug.Log("Run TestRoutine");
                    yield return StartCoroutine(Climb_Co());
                    Debug.Log("Finish TestRoutine");
                    climbTransfrom.localPosition = Vector3.zero;

                    //Debug.Log(" Finnish | elapsedTime | " + elapsedTime + " | " + isClimbing);

                    playerAni.SetBool("IsMove", true);

                    startPosition = playerRigid.position;
                    endPosition = new Vector3(nextWaypoint.position.x, playerRigid.position.y, nextWaypoint.position.z);
                    elapsedTime = 0f; // Reset elapsed time                

                    continue;
                }
            }

            // 떨어지는 모션
            if (CheckGroundPointsEmpty(1.5f)) {
                //endPosition.y += gravity * Time.deltaTime * moveSpeed;

                Debug.Log("Falling Start");
                yield return StartCoroutine(Fall_Co());
                Debug.Log("Falling End");

                startPosition = playerRigid.position;
                endPosition = new Vector3(nextWaypoint.position.x, playerRigid.position.y, nextWaypoint.position.z);
                elapsedTime = 0f; // Reset elapsed time    
            }

            // 일반 걷기 모션
            elapsedTime += Time.deltaTime * moveSpeed;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            playerRigid.MovePosition(Vector3.Lerp(startPosition, endPosition, t));


            yield return null;
        }

        playerRigid.position = nextWaypoint.position;
        playerRigid.rotation = startRotation;
        isMoving = false;

        playerAni.SetBool("IsMove", false);
    }

    private IEnumerator Climb_Co() {
        playerAni.SetBool("IsMove", false);
        playerAni.SetTrigger("IsClimb");

        yield return new WaitForSeconds(3.5f); // 애니메이션이 실행 될 때까지 대기
        Debug.Log("Climb Animation Length: " + playerAni.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        yield return new WaitForSeconds(.5f); // collider가 현재 위치까지 따라와야함
        climbTransfrom.localPosition = Vector3.zero;
    }
    private IEnumerator Fall_Co() {
        playerAni.SetBool("IsMove", false);
        playerAni.SetBool("IsFalling", true);

        // 중력의 크기 조정
        float fallSpeed = 0f;

        while (CheckGroundPointsEmpty(0.5f)) {
            // 중력 적용: 가속도
            fallSpeed += gravity * Time.deltaTime;
            playerRigid.position -= Vector3.down * fallSpeed * Time.deltaTime; // 아래로 이동

            yield return null;
        }

        playerAni.SetBool("IsFalling", false);
        playerAni.SetBool("IsMove", true);
    }

    // 바닥 오브젝트 확인
    public bool CheckGroundPointsEmpty(float rayLength) {

        groundPoint.transform.localPosition = Vector3.zero;
        foreach (Transform each in groundPoint.transform) {
            if (Physics.Raycast(each.position, Vector3.down, out RaycastHit hit, rayLength)) {
                if (hit.collider.gameObject != transform.gameObject) {
                    return false;
                }
            }
        }
        return true;
    }

    private void OnDrawGizmos() {
        if (nextWaypoint != null) {
            // 현재 위치에서 다음 웨이포인트까지 선 그리기
            Gizmos.color = Color.blue; // 선 색상 설정
                                       // 레이캐스트의 끝점 계산
            Vector3 rayEndPoint = climbTransfrom.position + transform.forward;

            // 선 그리기
            Gizmos.DrawLine(climbTransfrom.position, rayEndPoint);
        }
    }
}

/*
// 플레이어가 자동으로 경로점으로 이동 -> y좌표 일정하게 가다가 오르막이 있을 경우 climb모션
private IEnumerator MoveToNextWaypoint(Transform nextWaypoint) {
    isMoving = true;

    // DOTween을 사용하여 위치 이동
    transform.DOMove(nextWaypoint.position, moveSpeed).OnComplete(() => {
        isMoving = false;
    });

    // DOTween의 이동이 완료될 때까지 기다리기
    while (isMoving) {
        yield return null;
    }
}
*/
