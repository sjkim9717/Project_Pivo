using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast3DCheck : MonoBehaviour
{
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
        int trueCount=0;

        for (int i = 0; i < groundPoint.transform.childCount; i++) {

            Transform child = groundPoint.transform.GetChild(i);

            RaycastHit[] hits = Physics.RaycastAll(child.position, child.forward, rayLength);
            Debug.DrawRay(child.position, child.forward, Color.red, rayLength);

            for (int j = 0; j < hits.Length; j++) {
                if (hits.Length <= 0) {
                    hitsbool[i] =  false;
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

    private bool 

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