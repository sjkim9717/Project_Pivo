using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour {

    private int dieCount;
    private bool is3DPlayer;
    private bool isRespqwnDone;

    private Vector3 moveposition;
    public void SetPlayerDieCount() { dieCount++; }                                     // 죽었을 경우 Die Count 증가
    public void SetPlayerMode(bool is3DPlayer) { this.is3DPlayer = is3DPlayer; }
    public bool GetPlayerMode() { return is3DPlayer; }

    private GameObject player2D;
    private GameObject player3D;

    private Transform respawnposition;                                                  // 큐브위로 올라갈때 위치가 변경될 경우만 잡아서 갱신할 것 \=

    public UnityEvent OnPlayerDead;                                                     // 플레이어가 죽었을 경우 이벤트 인스펙터 창에서 연결
    public UnityEvent<Vector3> onPlayerEnterTile;


    private void Awake() {
        player3D = transform.GetChild(0).gameObject;
        player2D = transform.GetChild(1).gameObject;

        moveposition = Vector3.zero;
        respawnposition = transform;

        player3D.SetActive(true);
        player2D.SetActive(false);
        is3DPlayer = true;
    }

    private void Start() {
        onPlayerEnterTile.AddListener(UpdateRespawnPosition);
    }
    private void Update() {
        if (dieCount >= 3) {
            dieCount = 0;
            //TODO: [respawn]
            Dead();
        }
    }

    private void Dead() {
        OnPlayerDead.Invoke();
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

    // respawn 위치 맞추기
    private void UpdateRespawnPosition(Vector3 newRespawnPosition) {
        if (respawnposition != null) {
            respawnposition.position = newRespawnPosition;
            Debug.Log("Respawn position updated to: " + newRespawnPosition);
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

    }

    public void Falling() {
        if (is3DPlayer) {
            Rigidbody playerRigidbody = player3D.GetComponent<Rigidbody>();
            playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else {
            Rigidbody2D playerRigidbody = player2D.GetComponent<Rigidbody2D>();
            playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
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
