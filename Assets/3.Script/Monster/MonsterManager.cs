using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonsterBase {
    private static MonsterManager _instance;
    public static MonsterManager instance { get { return _instance; } }

    protected override void Awake() {

        if(_instance == null) {
            _instance = this;
        }
        else {
            Destroy(gameObject);
        }
        base.Awake();

    }

    private void Start() {
        Change3D();
        //TODO: 모드 변경
        PlayerManage.instance.IsSwitchMode += SwitchMode;
        //TODO: IMonsterStateBase 초기값 지정

    }

    public void SwitchMode() {
        Effect.SetActive(true);
        if (PlayerManage.instance.CurrentMode == PlayerMode.Player3D) {
            Change3D();
        }
        else if (PlayerManage.instance.CurrentMode == PlayerMode.Player2D) {
            Change2D();
        }
    }

}

