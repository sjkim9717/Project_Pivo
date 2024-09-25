using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManage : PlayerBase {
    private static PlayerManage _instance;
    public static PlayerManage instance { get { return _instance; } }

    public Action IsSwitchMode;
    public static Action PlayerDead;
    public UnityEvent<Vector3> onPlayerEnterTile;

    private Transform respawnposition;                                                  // 큐브위로 올라갈때 위치가 변경될 경우만 잡아서 갱신할 것 \=
    public Transform Respawnposition { get { return respawnposition; } }

    private int dieCount;
    private bool isDieActionDone;
    public bool IsBombOnGround;
    public bool IsChangingModeTo3D = false;                                             // 2D에서 3D로 넘어갈 경우 -> fall 조절
    
    private IBomb groundBomb;

    public int GetPlayerDieCount() {
        if (dieCount >= 3) dieCount = 3;
        return dieCount;
    }

    public void SetPlayerDieCount() { dieCount++; }                                     // 죽었을 경우 Die Count 증가



    protected override void Awake() {

        if (_instance == null) {
            _instance = this;
        }
        else {
            Destroy(gameObject); // 기존 인스턴스가 있으면 현재 객체를 제거
        }

        base.Awake();

        respawnposition = new GameObject("RespawnPosition").transform;

    }

    private void Start() {
        Init();

        onPlayerEnterTile.AddListener(UpdateRespawnPosition);

        // restart 초기화 값 - diecount, 플레이어 위치, 플레이어 모드
        StaticManager.Restart += Restart;
    }
    private void Update() {
        if (dieCount >= 3 && !isDieActionDone) {
            isDieActionDone = true;
            PlayerDead?.Invoke();
            SwitchMode_Dead();
        }
    }

    private void Init() {
        dieCount = 0;
        if (GameManager.instance.IsTutorialCompleted) {
            Change3D();
        }
        else {
            ChangeAutoMode();
        }
    }
    private void Restart() {
        dieCount = 0;
        Debug.LogWarning(" 뒤졌으면 불러져야함");
        Change3D();
        base.PlayerRigid3D.position = transform.position;
        base.PlayerRigid2D.position = transform.position;
        isDieActionDone = false;
    }


    private void SwitchMode_Dead() {
        ConvertMode[] convertModes = FindObjectsOfType<ConvertMode>();
        foreach (ConvertMode mode in convertModes) {
            mode.ChangeLayerAllActiveTrue();
            mode.ChangeActiveWithLayer();
        }

        Change3D();
    }

    public void SwitchMode() {
        IsSwitchMode?.Invoke();

        ConvertMode[] convertModes = FindObjectsOfType<ConvertMode>();
        foreach (ConvertMode mode in convertModes) {
            mode.ChangeActiveWithLayer();
        }

        if (CurrentMode == PlayerMode.Player3D) {
            Change3D();
        }
        else if (CurrentMode == PlayerMode.Player2D) {
            Change2D();
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

    // Respawn
    public void Respawn() {
        if (CurrentMode == PlayerMode.Player3D) {
            base.Player3D.transform.position = respawnposition.position;
            base.PlayerRigid3D.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else {
            base.Player2D.transform.position = respawnposition.position;
            base.PlayerRigid2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        SetPlayerDieCount();
        Debug.Log("player Manager | die count" + dieCount);

    }

    public void Falling() {     // SetPlayerDieCount 증가하고 

        if (CurrentMode == PlayerMode.Player3D) {//TODO:[Test 필요] y값 위치 확인 후 떨어져야함

            base.PlayerRigid3D.constraints = RigidbodyConstraints.FreezeRotation;
            if (CheckPlayerYPosition(Player3D)) { Respawn(); }
            else {
                base.Ani3D.SetTrigger("IsFalling");
            }
        }
        else {

            base.PlayerRigid2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            if (CheckPlayerYPosition(Player2D)) { Respawn(); }
            else {
                base.Ani2D.SetTrigger("IsFalling");
            }
        }
    }

    private bool CheckPlayerYPosition(GameObject player) {
        return player.transform.position.y <= (transform.position.y - 20f);
    }


    public void SetPlantBomb(IBomb bomb) {
        groundBomb = bomb;
    }

    public IBomb GetPlantBomb() {
        IBomb bomb = groundBomb;
        groundBomb = null;
        return bomb;
    }


    public void Test() {
        Change3D();
    }

}
