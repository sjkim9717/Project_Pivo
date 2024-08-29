using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle3DCheck : MonoBehaviour {
    private GameObject groundPoint;
    private GameObject blockPoint;
    private GameObject climbPoint;

    private PlayerManager playerManager;


    private void Awake() {
        playerManager = transform.parent.GetComponent<PlayerManager>();

        groundPoint = transform.GetChild(1).gameObject;
        blockPoint = transform.GetChild(2).gameObject;
        climbPoint = transform.GetChild(3).gameObject;
    }

    private void Update() {
        if (CheckGroundPointsEmpty(2f)) {                   // 바로 밑에 없으면 확인해야함
            if (CheckGroundPointsEmpty(10f)) {              // 이동 했을 경우 일정거리 안으로 확인해서 
                //TODO: [falling]

            }
            else {                                          // die count 증가               
                playerManager.SetPlayerDieCount();
                Debug.Log("SetPlayerDieCount ");
            }
        }
    }


    // 바닥 오브젝트 확인
    private bool CheckGroundPointsEmpty(float rayLength) {

        bool[] hitsbool = new bool[groundPoint.transform.childCount];
        int trueCount = 0;

        for (int i = 0; i < groundPoint.transform.childCount; i++) {

            Transform child = groundPoint.transform.GetChild(i);

            RaycastHit[] hits = Physics.RaycastAll(child.position, child.forward, rayLength);
            Debug.DrawRay(child.position, child.forward, Color.red, rayLength);

            for (int j = 0; j < hits.Length; j++) {
                if (hits.Length <= 0) {
                    hitsbool[i] = false;
                }
                else if (hits.Length == 1) {
                    if (hits[0].collider.CompareTag("Player")) {
                        hitsbool[i] = false;
                    }
                    else {
                        hitsbool[i] = true;
                    }
                }
                else {
                    hitsbool[i] = true;
                }
            }
        }

        for (int i = 0; i < hitsbool.Length; i++) {
            if (hitsbool[i] == false) {
                trueCount++;
            }
        }

        return trueCount == hitsbool.Length ? true : false;

    }

    public void OnButtonTest_Climb() {
        if(CheckClimbPointsEmpty()) {
            Debug.Log("여기가 climb 완료 시점");
        }
    }



    // player 주변 원형으로 모든 콜라이더를 감지해서 들고옴 -> y축을 기준으로 바닥 바로 위
    public bool CheckClimbPointsEmpty() {
        List<GameObject> bottomObstacles = new List<GameObject>();
        List<GameObject> topObstacles = new List<GameObject>();

        Collider[] colliders = Physics.OverlapSphere(transform.position, 2.7f);     // tile : 2 + player : 0.7

        foreach (Collider each in colliders) {
            GameObject eachParent = each.transform.parent != null ? each.transform.parent.gameObject : each.gameObject;

            if ((eachParent.transform.position.y) >= transform.position.y) {
                Debug.Log("전체 다 들어오는지 | " + eachParent.name);
                if ((eachParent.transform.position.y + 1) <= transform.position.y + 2.5f) {        // 플레이어 y축 0 ~ 2 까지 : 첫 번째 층
                    bottomObstacles.Add(eachParent);
                    Debug.Log("bottomObstacle | " + eachParent.name);
                }
                else if ((eachParent.transform.position.y + 1) <= transform.position.y + 4.5f) {   // 플레이어 y축 +2이상 :  두 번째 층
                    topObstacles.Add(eachParent);
                    Debug.Log("topObstacles | " + eachParent.name);
                }
            }
        }

        // bottom and top nomal vector check
        if (!CheckObstacleAngle(topObstacles)) {
            if (CheckObstacleAngle(bottomObstacles)) {
                Debug.Log("topObstacles 가 없고 bottomObstacles 있음");
                return true;
            }
            else {
                Debug.Log("topObstacles 가 없고 bottomObstacles도 없음 ");
            }
        }
        else {
            Debug.Log("topObstacles 가 있음");
        }

        return false;
    }

    // gameobject를 돌면서  각도 계산
    // 좌표 변환을 플레이어만 절대 좌표로 돌림 : 방향이 다름
    // y좌표를 플레이어 기준으로 맞추고
    // 각도 계산
    private bool CheckObstacleAngle(List<GameObject> objs) {

        foreach (GameObject item in objs) {

            Vector3 tilePos = item.transform.parent != null ? item.transform.parent.position : item.transform.position;               // 감지된 타일의 현재 월드 위치

            Vector3 playerToTile = tilePos - transform.position;
            Vector3 posTile = transform.InverseTransformDirection(playerToTile);

            float direction = Vector3.Dot(posTile, Vector3.forward);

            if (direction >= 0) { return true; }
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
: block의 34번이 없어야함 12번은 있어야함 5번 뭐야 
 

구형 감지
OverlapSphere OnCollisionEnter()
SphereCast	RaycastHit[] 

others => postion.y 가 같은 object position - postion <= (오차범위)

other -> 내 좌표를 기준으로 각도를 계산 (hitpoint normal, position에 대한 각도 계산)
 if( 각도 < 부채꼴최대 && 각도 > 부채꼴최소 )

 
 */

/*
 using UnityEngine;
using System.Collections.Generic;

public class YourClassName : MonoBehaviour
{
    // Define your sector angle limits
    public float minAngle = -45f; // Minimum angle of the sector
    public float maxAngle = 45f;  // Maximum angle of the sector

    // player 주변 원형으로 모든 콜라이더를 감지해서 들고옴 -> y축을 기준으로 바닥 바로 위
    private bool CheckClimbPointsEmpty()
    {
        List<GameObject> bottomObstacles = new List<GameObject>();
        List<GameObject> topObstacles = new List<GameObject>();

        Collider[] colliders = Physics.OverlapSphere(transform.position, 2.7f); // tile: 2 + player: 0.7

        foreach (Collider each in colliders)
        {
            GameObject eachParent = each.transform.parent.gameObject;

            if ((eachParent.transform.position.y - 1) >= transform.position.y)
            {
                if ((eachParent.transform.position.y + 1) <= transform.position.y + 2) // 플레이어 y축 0 ~ 2 까지 : 첫 번째 층
                {
                    bottomObstacles.Add(eachParent);
                }
                else if ((eachParent.transform.position.y + 1) <= transform.position.y + 4) // 플레이어 y축 +2이상 : 두 번째 층
                {
                    topObstacles.Add(eachParent);
                }
            }

            // TODO: bottom and top normal vector check
            // Calculate the direction from player to the collider
            Vector3 directionToCollider = each.transform.position - transform.position;

            // Calculate the angle between the forward vector and the directionToCollider
            float angle = Vector3.SignedAngle(transform.forward, directionToCollider, Vector3.up);

            // Check if the angle is within the specified sector
            if (angle > minAngle && angle < maxAngle)
            {
                Debug.Log($"Collider within sector: {each.gameObject.name}, Angle: {angle}");
                // Additional logic if necessary, e.g., adding to a list or handling the object
            }
        }


        return true;
    }

    // Draw Gizmos in the editor to visualize the sector
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2.7f);

        // Visualize the sector angles
        Vector3 forward = transform.forward * 3f;
        Vector3 minAngleDirection = Quaternion.Euler(0, minAngle, 0) * forward;
        Vector3 maxAngleDirection = Quaternion.Euler(0, maxAngle, 0) * forward;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, minAngleDirection);
        Gizmos.DrawRay(transform.position, maxAngleDirection);
    }
}
 
 
 
 */