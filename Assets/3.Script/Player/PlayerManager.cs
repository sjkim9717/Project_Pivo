using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// stage별 각기다른 위치로 인하여 Scene마다 개별 플레이어 존재
public class PlayerManager : MonoBehaviour {
    public static Action PlayerDead;

    private int dieCount;
    private bool is3DPlayer;

    public bool IsMovingStop;                                                           // getaxis의 키를 받아서 움직임 외 다른 기능을 사용할 경우 
    public bool isChangingModeTo3D = false;                                             // 2D에서 3D로 넘어갈 경우 -> fall 조절

    private Vector3 moveposition;

    public int GetPlayerDieCount() {
        if (dieCount >= 3) dieCount = 3;
        return dieCount;
    }
    public bool GetPlayerMode() { return is3DPlayer; }

    public void SetPlayerDieCount() { dieCount++; }                                     // 죽었을 경우 Die Count 증가
    public void SetPlayerMode(bool is3DPlayer) { this.is3DPlayer = is3DPlayer; }

    private GameObject player2D;
    private GameObject player3D;

    private Transform respawnposition;                                                  // 큐브위로 올라갈때 위치가 변경될 경우만 잡아서 갱신할 것 \=

    public UnityEvent<Vector3> onPlayerEnterTile;

    private StageClearController stageClear;

    private void Awake() {
        player3D = transform.GetChild(0).gameObject;
        player2D = transform.GetChild(1).gameObject;

        moveposition = Vector3.zero;
        respawnposition = new GameObject("RespawnPosition").transform;

        stageClear = FindObjectOfType<StageClearController>();

        Init();
    }

    private void Start() {
        onPlayerEnterTile.AddListener(UpdateRespawnPosition);

        stageClear.StageClear += StageClear;

        PlayerDead += Dead;

        // restart 초기화 값 - diecount, 플레이어 위치, 플레이어 모드
        StaticManager.Restart += Init;
        StaticManager.Restart += PositionInit;
    }
    private void Update() {
        if (dieCount >= 3) {
            PlayerDead?.Invoke();
        }
    }

    private void Init() {
        dieCount = 0;
        player3D.SetActive(true);
        player2D.SetActive(false);
        is3DPlayer = true;

    }

    private void StageClear() {
        player3D.SetActive(false);
        player2D.SetActive(false);
    }

    public void PositionInit() {                                                    // Restart 연결? 씬넘어가면 전부 풀리겠지?
        player3D.GetComponent<Rigidbody>().position = transform.position;
        player2D.GetComponent<Rigidbody2D>().position = transform.position;
    }

    private void Dead() {
        dieCount = 0;
        player2D.SetActive(false);
        player3D.SetActive(true);
    }

    public void SwitchMode() {

        FindObjectOfType<MapManager>().ChangeActiveTile();

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

    // respawn 위치 맞추기
    private void UpdateRespawnPosition(Vector3 newRespawnPosition) {
        if (respawnposition != null) {
            respawnposition.position = newRespawnPosition;
            //Debug.Log("Respawn position updated to: " + newRespawnPosition);
        }
        else {
            Debug.LogWarning("Respawn position transform is not set in the PlayerManager.");
        }
    }

    //TODO: button에 달아야함
    public void Respawn() {
        if (is3DPlayer) {
            player3D.transform.position = respawnposition.position;
            Rigidbody playerRigidbody = player3D.GetComponent<Rigidbody>();
            playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else {
            player2D.transform.position = respawnposition.position;
            Rigidbody2D playerRigidbody = player2D.GetComponent<Rigidbody2D>();
            playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        SetPlayerDieCount();
        Debug.Log("player Manager | die count" + dieCount);
    }

    public void Falling() {     // SetPlayerDieCount 증가하고 

        if (is3DPlayer) {//TODO:[Test 필요] y값 위치 확인 후 떨어져야함

            player3D.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            if (CheckPlayerYPosition(player3D)) { Respawn(); }
            else {
                player3D.GetComponentInChildren<Animator>().SetTrigger("IsFalling");
            }
        }
        else {

            player2D.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            if (CheckPlayerYPosition(player2D)) { Respawn(); }
            else {
                player3D.GetComponent<Animator>().SetTrigger("IsFalling");
            }
        }
    }

    private bool CheckPlayerYPosition(GameObject player) {
        return player.transform.position.y <= (transform.position.y - 20f);
    }



    /*
     1. die count 올리고
     2. die count 세서 3이상이면 respawn -> die 호출
     3. die count 3이하일 경우는 y값 일정 이하인지 확인해서 respawn 호출 

     */

}


/*
  - 상위 오브젝트

 1. 플레이어 2D, 3D변경 : 이동 x, 모드 변경시 위치 동기화

 하위 프로젝트에서 플레이어 모드를 변경해줌(스킬사용 할 경우)
 해당 모드에 맞춰서 3d position과 2d position을 저장하고 
 반대 모드 켜지면 위치 변경해줌 
 
 
 */
