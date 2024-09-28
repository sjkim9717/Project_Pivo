using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2DControl : MonoBehaviour {

    [SerializeField] private float gravityAddSpeed = 2f;
    public float moveSpeed = 7f;
    private float gravity = -9.8f;
    protected int activeFalseLayerIndex;
    private LayerMask layerMaskIndex;
    public Animator Ani2D { get { return PlayerManage.instance.Ani2D; } }
    public GameObject Player { get { return PlayerManage.instance.Player2D; } }
    public Rigidbody2D PlayerRigid { get { return PlayerManage.instance.PlayerRigid2D; } }

    private PlayerManage playerManager;

    private Vector3 positionToMove = Vector3.zero;

    private PlayerState2D currentStateComponent;
    private Dictionary<PlayerState, PlayerState2D> stateDic;

    private GameObject groundPoint;
    public GameObject GroundPoint { get { return groundPoint; } }

    private void Awake() {
        playerManager = transform.parent.GetComponent<PlayerManage>();
        groundPoint = Player.transform.GetChild(1).gameObject;
        activeFalseLayerIndex = LayerMask.NameToLayer("ActiveFalse");
        layerMaskIndex = 1 << LayerMask.NameToLayer("Ground");
        InitializeStates();
    }
    private void OnEnable() {
        ChangeState(PlayerState.Idle);
    }

    private void InitializeStates() {
        stateDic = new Dictionary<PlayerState, PlayerState2D>();

        foreach (PlayerState state in Enum.GetValues(typeof(PlayerState))) {

            string stateClassName = $"PlayerState2D_{state}";

            Type stateType = Type.GetType(stateClassName);

            if (stateType != null) {
                var stateComponent = GetComponent(stateType) as PlayerState2D;
                if (stateComponent != null) {
                    stateDic.Add(state, stateComponent);
                }
                else {
                    //Debug.LogWarning($"Component for {stateClassName} not found on {gameObject.name}");
                }
            }
        }

        // 모든 상태를 비활성화
        foreach (var state in stateDic.Values) {
            state.enabled = false;
        }
    }

    // 플레이어 상태 변경 메서드
    public void ChangeState(PlayerState newState) {
        //Debug.Log(newState);
        // 현재 상태가 null이 아니면 비활성화
        if (currentStateComponent != null) {
            currentStateComponent.ExitState();
            currentStateComponent.enabled = false;
        }
        // 새로운 상태를 가져와서 활성화
        if (stateDic.TryGetValue(newState, out PlayerState2D newStateComponent)) {
            PlayerManage.instance.CurrentState = newState;

            currentStateComponent = newStateComponent;
            currentStateComponent.enabled = true;
            currentStateComponent.EnterState(); // 새로운 상태에서 필요한 초기화 작업이 있다면 호출
        }
        else {
            Debug.LogWarning($"State {newState} not found in stateDic.");
        }

    }

    public void Move(float horizontalInput) {
        PlayerRigid.constraints = RigidbodyConstraints2D.FreezeRotation;

        Vector3 dir = new Vector3(horizontalInput, 0, 0).normalized;

        if (horizontalInput != 0) {

            float moveDirection = horizontalInput > 0 ? 1f : -1f;
            transform.localScale = new Vector3(moveDirection, 1f, 1f);
            positionToMove = dir * moveSpeed * Time.fixedDeltaTime;

            if (CheckGroundPointsEmpty(1f)) {
                positionToMove.y += gravity * Time.deltaTime * gravityAddSpeed;
            }

            PlayerRigid.MovePosition(PlayerRigid.position + (Vector2)positionToMove);
        }
        else {
            Vector3 currentVelocity = PlayerRigid.velocity;
            PlayerRigid.velocity = new Vector3(0, currentVelocity.y, 0);

        }
    }

    // 바닥 오브젝트 확인
    public bool CheckGroundPointsEmpty(float rayLength) {
        foreach (Transform each in groundPoint.transform) {
            RaycastHit2D hit = Physics2D.Raycast(each.position, Vector2.down, rayLength, layerMaskIndex);
            if (hit.collider != null) {
                return false;
            }
        }

        return true;
    }


    // player 주변 원형으로 모든 콜라이더를 감지해서 들고옴 -> y축을 기준으로 바닥 바로 위
    public GameObject CheckInteractObject() {
        GameObject interactionObj = null;

        List<GameObject> bottomObstacles = new List<GameObject>();
        List<GameObject> topObstacles = new List<GameObject>();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2.7f);     // tile : 2 + player : 0.7

        foreach (Collider2D each in colliders) {
            if (each.gameObject.layer == activeFalseLayerIndex) {
                continue;
            }

            GameObject eachParent = each.transform.parent != null ? each.transform.parent.gameObject : each.gameObject;

            Transform rootTransform = eachParent.transform.Find("Root3D");
            if (rootTransform == null || !rootTransform.CompareTag("Climb") && !rootTransform.CompareTag("PushBox")) {
                continue; // Skip if not a climable object
            }

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
        if (!CheckObstacleAngle(topObstacles, ref interactionObj)) {
            if (CheckObstacleAngle(bottomObstacles, ref interactionObj)) {
                //Debug.Log("topObstacles 가 없고 bottomObstacles 있음");
                return interactionObj;
            }
            //else {
            //    Debug.Log("topObstacles 가 없고 bottomObstacles도 없음");
            //}
        }
        //else {
        //    Debug.Log("topObstacles 가 있음");
        //}

        return null;
    }

    private bool CheckObstacleAngle(List<GameObject> objs, ref GameObject interactionObj) {

        foreach (GameObject item in objs) {

            Vector3 tilePos = item.transform.position;               // 감지된 타일의 현재 월드 위치
            Vector3 playerToTile = tilePos - transform.position;

            if (transform.localScale.x >= 0) {
                Debug.Log("player look right | object is on the right side");
                if (playerToTile.x >= 0) {
                    interactionObj = item;
                    return true;
                }
            }
            else {
                Debug.Log("player look left | object is on the left side");
                if (playerToTile.x <= 0) {
                    interactionObj = item;
                    return true;
                }
            }
        }

        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.transform.position.y>= transform.position.y) {
            Debug.Log(" 플레이어 위에 있는 물체 | " + collision.collider.name);
            if (collision.transform.parent!=null) {
                Debug.Log(" 플레이어 위에 있는 물체 | " + collision.transform.parent.name);
            }
        }
    }



    private void OnDrawGizmos() {
        Gizmos.color = Color.green;        // Set the Gizmo color
        Gizmos.DrawWireSphere(transform.position, 2.7f);
    }
}
