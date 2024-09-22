using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster3DControl : MonsterControl {
    protected override void Start() {
        ChangeState(Idle3DState);
    }

    protected override void Update() {
        currentState?.UpdateState(this);
    }

    public override void ChangeState(IMonsterStateBase newState) {
        Debug.LogWarning("monster state change");
        Debug.LogWarning("current State | " + currentState + " | new State | " + newState);

        if (currentState == newState) return; // 동일 상태 체크
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

}
