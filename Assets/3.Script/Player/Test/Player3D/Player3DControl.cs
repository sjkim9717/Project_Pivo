using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3DControl : MonoBehaviour {

    public float moveSpeed = 5f;
    private bool isDead = false;

    public Animator Ani3D { get { return PlayerManage.instance.Ani3D; } }
    public GameObject Player { get { return PlayerManage.instance.Player3D; } }
    public Rigidbody PlayerRigid { get { return PlayerManage.instance.PlayerRigid3D; } }

    private PlayerManage playerManager;

    private Vector3 positionToMove = Vector3.zero;

    private PlayerState3D currentStateComponent;
    private Dictionary<PlayerState, PlayerState3D> stateDic;

    private GameObject groundPoint;
    public GameObject GroundPoint { get { return groundPoint; } }
    public GameObject InteractionObject;

    private void Awake() {
        playerManager = transform.parent.GetComponent<PlayerManage>();

        groundPoint = Player.transform.GetChild(1).gameObject;

        PlayerManager.PlayerDead += Dead;

        InitializeStates();
    }

    private void OnEnable() {
        ChangeState(PlayerState.Idle);
    }

    private void OnCollisionStay(Collision collision) {
        if (collision.collider.CompareTag("BombSpawner") || collision.collider.CompareTag("OpenPanel") || collision.collider.CompareTag("ClimbObj")) {
            InteractionObject = collision.transform.parent.gameObject;
        }
    }

    private void OnCollisionExit(Collision collision) {
        if (InteractionObject!= null) {
            InteractionObject = null;
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("PushSwitch")) {
            InteractionObject = other.transform.parent.gameObject;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (InteractionObject != null) {
            InteractionObject = null;
        }
    }



    private void InitializeStates() {
        stateDic = new Dictionary<PlayerState, PlayerState3D>();

        foreach (PlayerState state in Enum.GetValues(typeof(PlayerState))) {

            string stateClassName = $"PlayerState3D_{state}";

            Type stateType = Type.GetType(stateClassName);

            if (stateType != null) {
                var stateComponent = GetComponent(stateType) as PlayerState3D;
                if (stateComponent != null) {
                    stateDic.Add(state, stateComponent);
                }
                else {
                    Debug.LogWarning($"Component for {stateClassName} not found on {gameObject.name}");
                }
            }
            else {
                Debug.LogWarning($"State class {stateClassName} not found");
            }
        }

        // 모든 상태를 비활성화
        foreach (var state in stateDic.Values) {
            state.enabled = false;
        }
    }

    // 플레이어 상태 변경 메서드
    public void ChangeState(PlayerState newState) {
        Debug.Log(newState);
        // 현재 상태가 null이 아니면 비활성화
        if (currentStateComponent != null) {
            currentStateComponent.ExitState();
            currentStateComponent.enabled = false;
        }
        // 새로운 상태를 가져와서 활성화
        if (stateDic.TryGetValue(newState, out PlayerState3D newStateComponent)) {
            PlayerManage.instance.CurrentState = newState;

            currentStateComponent = newStateComponent;
            currentStateComponent.enabled = true;
            currentStateComponent.EnterState(); // 새로운 상태에서 필요한 초기화 작업이 있다면 호출
        }
        else {
            Debug.LogWarning($"State {newState} not found in stateDic.");
        }
    }


    public void Move(float horizontalInput, float verticalInput) {
        positionToMove = Vector3.zero;

        if(horizontalInput != 0 || verticalInput !=0) {
            // 방향 벡터를 생성
            Vector3 dir = new Vector3(horizontalInput, 0, verticalInput).normalized;

            // 캐릭터 회전
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * moveSpeed);

            positionToMove = dir * moveSpeed * Time.deltaTime;
            PlayerRigid.MovePosition(PlayerRigid.position + positionToMove);
        }
        else {
            PlayerRigid.velocity = Vector3.zero;
        }
    }

    // 바닥 오브젝트 확인
    public bool CheckGroundPointsEmpty(float rayLength) {

        bool[] hitsbool = new bool[groundPoint.transform.childCount];
        int falseCount = 0;

        for (int i = 0; i < groundPoint.transform.childCount; i++) {
            Transform child = groundPoint.transform.GetChild(i);

            RaycastHit[] hits = Physics.RaycastAll(child.position, -child.up, rayLength);

            List<RaycastHit> filteredHits = new List<RaycastHit>();          // `hits` 배열에서 태그가 "Player"인 오브젝트를 제외

            foreach (RaycastHit hit in hits) {
                if (!hit.collider.CompareTag("Player")) {
                    filteredHits.Add(hit);
                }
            }

            // 필터링된 배열로 `hitsbool` 업데이트
            if (filteredHits.Count <= 0) hitsbool[i] = false;               // 오브젝트가 없을 경우 false
            else hitsbool[i] = true;                                        // 나머지 경우에는 true

        }

        for (int i = 0; i < hitsbool.Length; i++) {
            if (hitsbool[i] == false) {
                falseCount++;
            }
        }

        return falseCount == hitsbool.Length ? true : false;
    }

    public bool IsAnimationFinished(string flagName) {
        // 현재 애니메이션 상태 정보 가져오기
        AnimatorStateInfo stateInfo = Ani3D.GetCurrentAnimatorStateInfo(0);

        // 애니메이션 상태가 완료되었는지 확인
        return stateInfo.IsName($"{flagName}") && stateInfo.normalizedTime >= 1.0f;
    }



    // player 주변 원형으로 모든 콜라이더를 감지해서 들고옴 -> y축을 기준으로 바닥 바로 위
    public bool CheckInteractObject() {
        List<GameObject> bottomObstacles = new List<GameObject>();
        List<GameObject> topObstacles = new List<GameObject>();

        Collider[] colliders = Physics.OverlapSphere(transform.position, 2.7f);     // tile : 2 + player : 0.7

        foreach (Collider each in colliders) {

            if (!each.CompareTag("ClimbObj")) continue;

            GameObject eachParent = each.transform.parent != null ? each.transform.parent.gameObject : each.gameObject;

            if ((eachParent.transform.position.y) >= transform.position.y) {
                //Debug.Log("전체 다 들어오는지 | " + eachParent.name);
                if ((eachParent.transform.position.y + 1) <= transform.position.y + 2.5f) {        // 플레이어 y축 0 ~ 2 까지 : 첫 번째 층
                    bottomObstacles.Add(eachParent);
                    //Debug.Log("bottomObstacle | " + eachParent.name);
                }
                else if ((eachParent.transform.position.y + 1) <= transform.position.y + 4.5f) {   // 플레이어 y축 +2이상 :  두 번째 층
                    topObstacles.Add(eachParent);
                    //Debug.Log("topObstacles | " + eachParent.name);
                }
            }
        }


        // bottom and top nomal vector check
        if (!CheckObstacleAngle(topObstacles)) {
            if (CheckObstacleAngle(bottomObstacles)) {
                //Debug.Log("topObstacles 가 없고 bottomObstacles 있음");
                return true;
            }
            else {
                //Debug.Log("topObstacles 가 없고 bottomObstacles도 없음 ");
            }
        }
        else {
            //Debug.Log("topObstacles 가 있음");
        }

        return false;
    }

    private bool CheckObstacleAngle(List<GameObject> objs) {

        foreach (GameObject item in objs) {

            Vector3 tilePos = item.transform.position;               // 감지된 타일의 현재 월드 위치
            Vector3 playerToTile = tilePos - transform.position;
            float angle = Vector3.SignedAngle(transform.forward, playerToTile, Vector3.up);
            //Debug.Log("Calculated angle: " + angle);

            if (angle >= -40f && angle <= 40f) {
                //Debug.Log("타일이 시야 범위 내에 있습니다.");
                return true;
            }
        }

        return false;
    }


    private void OnDrawGizmos() {
        Gizmos.color = Color.green;        // Set the Gizmo color
        Gizmos.DrawWireSphere(transform.position, 2.7f);
    }

    //================== 미사용


    private void Dead() {
        Ani3D.SetTrigger("IsDie");
        isDead = true;

    }



}
