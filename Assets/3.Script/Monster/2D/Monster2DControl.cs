using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster2DControl : MonsterControl {
    protected override void Start() {
        ChangeState(Idle2DState);
    }

    protected override void Update() {
        currentState?.UpdateState(this);
    }

    public override void ChangeState(IMonsterStateBase newState) {
        Debug.LogWarning("monster 2D state change");
        Debug.LogWarning("current 2D State | " + currentState + " | new State | " + newState);

        if (currentState == newState) return; // 동일 상태 체크
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

}