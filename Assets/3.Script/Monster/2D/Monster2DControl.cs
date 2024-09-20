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
        Debug.LogWarning("monster state change");
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

}