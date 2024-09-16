using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviour {
    private StageLevel currentLevel;
    private Waypoint waypoint;

    private Animator playerAni;

    private Rigidbody playerRigid;
    private Transform nextWaypoint;
    private Transform climbTransfrom;

    public float moveSpeed = 5f; // 이동 속도
    private bool isMoving = false; // 플레이어가 이동 중인지 여부


    private void Awake() {
        waypoint = FindObjectOfType<Waypoint>();

        playerRigid = GetComponent<Rigidbody>();
        playerAni = GetComponentInChildren<Animator>();
        climbTransfrom = transform.GetChild(1);
    }
    private void Start() {
        currentLevel = GameManager.instance.PreviousGameStage;
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

        // 현재 방향 저장
        Quaternion startRotation = playerRigid.rotation;

        Vector3 direction = (endPosition - startPosition).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        playerRigid.MoveRotation(targetRotation);

        while (elapsedTime < 1f) {
            elapsedTime += Time.deltaTime * moveSpeed;
            playerRigid.MovePosition(Vector3.Lerp(startPosition, endPosition, elapsedTime));
            if (Physics.Raycast(climbTransfrom.position, transform.forward,out RaycastHit hit, 3f)) {
                if (hit.collider.CompareTag("ClimbObj")) {
                    Debug.LogWarning("!!!!!!!Climb 애니메이션 시작해야하는데");
                    StartCoroutine(Climb_Co());
                }
            }
            yield return null;
        }

        playerRigid.position = endPosition;
        playerRigid.rotation = startRotation;
        isMoving = false;

        playerAni.SetBool("IsMove", false);
    }

    private IEnumerator Climb_Co() {
        playerAni.Play("Climb"); 
        yield return null; 
    }





    /*
    //TODO: 플레이어가 자동으로 경로점으로 이동 -> y좌표 일정하게 가다가 오르막이 있을 경우 climb모션
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
}
