using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3DControl : MonoBehaviour {

    [SerializeField] private float gravityAddSpeed = 2f;
    public float moveSpeed = 7f;
    private float gravity = -9.8f;

    private Vector3 positionToMove = Vector3.zero;
    public Animator Ani3D { get; private set; }
    public GameObject Player { get; private set; }
    public Rigidbody PlayerRigid { get; private set; }

    private PlayerManage playerManage;

    private Dictionary<PlayerState, PlayerState3D> stateDic;
    private PlayerState3D currentStateComponent;

    private GameObject groundPoint;
    public GameObject InteractionObject;
    public GameObject GroundPoint { get { return groundPoint; } }

    private void Awake() {
        playerManage = transform.parent.GetComponent<PlayerManage>();

        Ani3D = playerManage.Ani3D;
        Player = playerManage.Player3D;
        PlayerRigid = playerManage.PlayerRigid3D;

        groundPoint = Player.transform.GetChild(1).gameObject;

        InitializeStates();
    }

    private void Start() {
        PlayerManage.PlayerDead += PlayerDead;
        //StaticManager.Restart += Restart;
    }

    private void Restart() {
        Ani3D.Rebind();
        ChangeState(PlayerState.Idle);
    }

    private void OnEnable() {
        ChangeState(PlayerState.Idle);
    }
    private void OnDestroy() {
        PlayerManage.PlayerDead -= PlayerDead; // Unsubscribe from event
    }

    private void PlayerDead() {
        if (stateDic.TryGetValue(PlayerState.Dead, out PlayerState3D dd)) {
            ChangeState(PlayerState.Dead);
        }
        else {
            Debug.Log("플레이어 사망 컨포넌트 왜 없지");
        }
    }

    private void OnCollisionStay(Collision collision) {
        if (collision.transform.position.y >= PlayerRigid.position.y) {
            if (collision.collider.CompareTag("Bomb")
                || collision.collider.CompareTag("OpenPanel") || collision.collider.CompareTag("Climb")) {
                InteractionObject = collision.transform.parent.gameObject;
            }
        }
    }

    private void OnCollisionExit(Collision collision) {
        if (InteractionObject != null) {
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
        if (stateDic.TryGetValue(newState, out PlayerState3D newStateComponent)) {
            playerManage.CurrentState = newState;

            currentStateComponent = newStateComponent;
            currentStateComponent.enabled = true;
            currentStateComponent.EnterState(); // 새로운 상태에서 필요한 초기화 작업이 있다면 호출
        }
        else {
            Debug.LogWarning($"State {newState} not found in stateDic.");
        }
    }


    public void Move(float horizontalInput, float verticalInput) {
       
        PlayerRigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        Vector3 dir = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (dir != Vector3.zero) {
            string[] include = { "move" };
            string key = AudioManager.instance.GetDictionaryKey<string, List< AudioClip>>(AudioManager.Corgi, include);
            AudioManager.instance.Corgi_Play(playerManage.PlayerAudio, key);

            // player rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.fixedDeltaTime * moveSpeed);

            // player movement
            positionToMove = dir * moveSpeed * Time.fixedDeltaTime;
            if (CheckGroundPointsEmpty(1f)) {
                positionToMove.y += gravity * Time.deltaTime * gravityAddSpeed;
            }
            PlayerRigid.MovePosition(PlayerRigid.position + positionToMove);
        }
        else {
            PlayerRigid.constraints = RigidbodyConstraints.FreezeRotation;
            // when player doesn't move
            Vector3 currentVelocity = PlayerRigid.velocity;
            PlayerRigid.velocity = new Vector3(0, currentVelocity.y, 0);

        }
    }


    // 바닥 오브젝트 확인
    public bool CheckGroundPointsEmpty(float rayLength) {

        foreach (Transform each in groundPoint.transform) {
            if (Physics.Raycast(each.position, Vector3.down, out RaycastHit hit, rayLength)) {
                if (hit.collider.gameObject != Player) {
                    return false;
                }
            }
        }
        return true;
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

            if (!each.CompareTag("Climb") && !each.CompareTag("PushBox")) continue;

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
                //Debug.Log("topObstacles 가 없고 bottomObstacles도 없음");
            }
        }
        else {
            //Debug.Log("topObstacles 가 있음");
        }

        return false;
    }
    private bool CheckObstacleAngle(List<GameObject> objs) {
        foreach (GameObject item in objs) {
            Vector3 tilePos = item.transform.position; // 감지된 타일의 현재 월드 위치
            Vector3 playerPos = transform.position;

            // Y축을 무시하고 XZ 평면에서 벡터 생성
            Vector3 playerToTileXZ = new Vector3(tilePos.x - playerPos.x, 0, tilePos.z - playerPos.z);
            Vector3 forwardXZ = new Vector3(transform.forward.x, 0, transform.forward.z);

            // XZ 평면에서 각도 계산
            float angle = Vector3.SignedAngle(forwardXZ, playerToTileXZ, Vector3.up);
            //Debug.LogWarning("Object name | " + item.name + " tilePos: " + tilePos + " playerToTileXZ: " + playerToTileXZ + " Calculated angle: " + angle);

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


}
