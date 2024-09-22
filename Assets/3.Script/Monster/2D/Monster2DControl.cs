using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster2DControl : MonsterControl {
    protected override void Awake() {
        base.Awake();

        Monster = base.gameObject;
        NavMesh = base.transform.GetComponent<NavMeshAgent>();

        Idle2DState = new Monster2DState_Idle(MainCamera, NavMesh, Monster, originPos, radius);
        Chase2DState = new Monster2DState_Chase(MainCamera, NavMesh, Monster, Player2D, originPos, radius);
        Attack2DState = new Monster2DState_Attack(MainCamera, NavMesh, Monster, Player2D, MonsterManager.instance.PutPoint.position);
        PassOut2DState = new Monster2DState_PassOut(MainCamera, Monster);
    }
    private void OnEnable() {
        ChangeState(Idle2DState);
    }

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
