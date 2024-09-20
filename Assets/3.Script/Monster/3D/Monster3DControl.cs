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
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

}
