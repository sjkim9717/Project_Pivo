using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    private int dieCount;
    private bool isDie;
    private bool is3DPlayer;
    private bool isRespqwnDone;

    public void SetPlayerDieCount() { dieCount++; }         // 죽었을 경우 Die Count 증가
    public void SetPlayerMode(bool is3DPlayer) { this.is3DPlayer = is3DPlayer; }

    private GameObject player3D;
    private GameObject player2D;

    private Transform respawnposition;  // 큐브위로 올라갈때 위치가 변경될 경우만 잡아서 갱신할 것 
    // 타일 밟을 때마다 이벤트로 위치변경

    private Vector3 moveposition;

    private void Awake() {
        player3D = GetComponentInChildren<Rigidbody>().gameObject;
        player2D = GetComponentInChildren<Rigidbody2D>().gameObject;

        moveposition = Vector3.zero;

        player3D.SetActive(true);
        player2D.SetActive(false);
    }



    public void SwitchMode() {
        if (is3DPlayer) {

            moveposition = player2D.transform.position;
            player3D.transform.position = moveposition;

            player2D.SetActive(false);
            player3D.SetActive(true);
        }
        else {

            moveposition = player3D.transform.position;
            player2D.transform.position = moveposition;

            player3D.SetActive(false);
            player2D.SetActive(true);
        }
    }



}


/*
  - 상위 오브젝트

 1. 플레이어 2D, 3D변경 : 이동 x, 모드 변경시 위치 동기화

 하위 프로젝트에서 플레이어 모드를 변경해줌(스킬사용 할 경우)
 해당 모드에 맞춰서 3d position과 2d position을 저장하고 
 반대 모드 켜지면 위치 변경해줌 
 
 
 */
