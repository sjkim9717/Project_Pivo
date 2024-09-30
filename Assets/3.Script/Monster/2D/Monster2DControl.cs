using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster2DControl : MonsterControl {
    protected override void Awake() {
        base.Awake();

        Monster = base.gameObject;
        NavMesh = base.transform.GetComponent<NavMeshAgent>();

        Idle2DState = new Monster2DState_Idle(mManager, MainCamera, NavMesh, Monster, originPos, radius);
        Chase2DState = new Monster2DState_Chase(mManager, MainCamera, NavMesh, Monster, Player2D, originPos, radius);
        Attack2DState = new Monster2DState_Attack(mManager, MainCamera, NavMesh, Monster, Player2D, mManager.PutPoint.position);
        PassOut2DState = new Monster2DState_PassOut(mManager, MainCamera, Monster);
    }

    protected override void Start() {
        ChangeState(Idle2DState);
    }
    private void OnEnable() {
        currentState?.CurrentEmotionUI(true);
        if (mManager.IsPassOutCalled) {
            Debug.LogWarning($" {Monster.name} | ispassout called");
            ChangeState(PassOut2DState);
        }
    }
    private void OnDisable() {
        currentState?.CurrentEmotionUI(false);
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
    private void OnDrawGizmos() {

        Gizmos.color = Color.blue; // 파란색으로 설정
        Gizmos.DrawWireSphere(originPos, radius); // 구 그리기
    }
}
