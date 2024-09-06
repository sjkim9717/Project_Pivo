using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle3DCheck : MonoBehaviour {

    [SerializeField] private float stopTime = 2f;
    [SerializeField] private float stayTime = 0.5f;

    private GameObject groundPoint;
    private GameObject holdingGroup;
    public GameObject ClimbObstacle;

    private Rigidbody player3DRigid;

    private PlayerManager playerManager;


    private void Awake() {
        player3DRigid = transform.GetComponent<Rigidbody>();

        playerManager = transform.parent.GetComponent<PlayerManager>();

        groundPoint = transform.GetChild(1).gameObject;
    }
    private void Start() {
        holdingGroup = FindObjectOfType<StaticManager>().HoldingGroup;
    }

    private void LateUpdate() {
        if (playerManager.isChangingModeTo3D) {          // 2d 에서 3 d 돌아왔을 때 

            if (CheckGroundPointsEmpty(2f)) {         

                holdingGroup.SetActive(true);
                playerManager.IsMovingStop = true;
                player3DRigid.constraints = RigidbodyConstraints.FreezeAll;

                if (stayTime <= 0) {

                    stopTime -= Time.deltaTime;

                    if (stopTime >= 0) {
                        WaitUntilChooseFallOr2D();
                    }
                    else {
                        InitFallOr2D();
                        playerManager.Falling();
                    }

                }
                else {
                    stayTime -= Time.deltaTime;
                    GetComponentInChildren<Animator>().SetTrigger("IsFalling");
                }
            }
        }
        else {
            if (CheckGroundPointsEmpty(20f)) {                   // 2d 에서 3 d 돌아왔을 때
                playerManager.Falling();
            }
        }        


        if (stayTime <= 0f && stopTime <= 0f) {
            InitFallOr2D();
        }

    }

    private void InitFallOr2D() {
        playerManager.IsMovingStop = false;
        playerManager.isChangingModeTo3D = false;
        holdingGroup.SetActive(false);

        stayTime = 0.5f;
        stopTime = 2f;
    }

    private void WaitUntilChooseFallOr2D() {

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float skillSectionInput = Input.GetAxis("SkillSection");

        if (horizontalInput != 0 || verticalInput != 0) {
            InitFallOr2D();

            playerManager.Falling();
        }
        else if (skillSectionInput != 0) {
            InitFallOr2D();

            playerManager.SetPlayerMode(false);
            playerManager.SwitchMode();
            Debug.Log("2D 모드로 전환됨");
        }
    }


    // 바닥 오브젝트 확인
    private bool CheckGroundPointsEmpty(float rayLength) {

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

    // player 주변 원형으로 모든 콜라이더를 감지해서 들고옴 -> y축을 기준으로 바닥 바로 위
    public bool CheckClimbPointsEmpty() {
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
                Debug.Log("타일이 시야 범위 내에 있습니다.");
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


/*
 1. 원하는 오브젝트의 하위 객체들의 transform에서 방향을 주어 ray를 쏴야함 

- ground : 모든 하위 오브젝트가 없을경우  거리 조절을 float로 할 것
    1. 일정 거리안에 없을 경우 : 끝까지 없는지 확인 있으면, fall 없으면 die count증가
    2. 끝까지 없을 경우 : die

- block : point 

- climb : 범위 안을 확인해야하나
    1. gameobject를 돌면서 OverlapSphere OnCollisionEnter() 로 구형 콜라이더 감지
    2. 모든 콜라이더의 자식객체의 y값을 나눠서 list로 topobstacle 과 bottomobstacle을 나누기
        3. list에 넣은 gameobject의 방향벡터를 순서대로 플레이어의 로컬 위치벡터로 변경
        4. 플레이어 앞쪽에 있는지 확인해서 bool값 전달
    5. topobstacle를 먼저 확인하고 위가 없으면 bottomobstacle 검사 
 


구형 감지
OverlapSphere OnCollisionEnter()
SphereCast	RaycastHit[] 

others => postion.y 가 같은 object position - postion <= (오차범위)

other -> 내 좌표를 기준으로 각도를 계산 (hitpoint normal, position에 대한 각도 계산)
 if( 각도 < 부채꼴최대 && 각도 > 부채꼴최소 )

 
 */

#region CheckObstacleAngle_save
/*
private bool CheckObstacleAngle(List<GameObject> objs) {

    foreach (GameObject item in objs) {

        Vector3 tilePos = item.transform.position;               // 감지된 타일의 현재 월드 위치

        Vector3 playerToTile = tilePos - transform.position;

        Debug.Log("item namge: " + item.transform.name);
        Debug.Log("Player position: " + transform.position);
        Debug.Log("Tile position: " + tilePos);
        Debug.Log("Player to Tile vector: " + playerToTile);

        float angle = Vector3.SignedAngle(transform.forward, playerToTile, Vector3.up);

        Debug.Log("Calculated angle: " + angle);

        if (angle >= -40f && angle <= 40f) {
            Debug.Log("타일이 시야 범위 내에 있습니다.");
            return true;
        }

        //Vector3 posTile = transform.InverseTransformDirection(playerToTile);

        //float direction = Vector3.Dot(posTile, Vector3.forward);

        //if (direction >= 0) {
        //    ClimbObstacle = item.transform.parent != null ? item.transform.parent.gameObject : item;
        //    Debug.Log("objs | " + objs + "item | " + item.name);
        //    return true; 
        //}
    }

    return false;
}
*/
#endregion