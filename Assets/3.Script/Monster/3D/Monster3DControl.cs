using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster3DControl : MonsterControl {

    protected override void Awake() {
        base.Awake();

        Monster = base.gameObject;
        NavMesh = base.transform.GetComponent<NavMeshAgent>();

        // 상태 인스턴스 생성 및 캐싱
        Idle3DState = new Monster3DState_Idle(mManager, MainCamera, NavMesh, Monster, originPos, radius);
        Chase3DState = new Monster3DState_Chase(mManager, MainCamera, NavMesh, Monster, Player3D, originPos, radius);
        Attack3DState = new Monster3DState_Attack(mManager, MainCamera, NavMesh, Monster, Player3D, mManager.PutPoint.position);
        PassOut3DState = new Monster3DState_PassOut(mManager, MainCamera, Monster);
    }

    protected override void Start() {
        ChangeState(Idle3DState);
    }
    private void OnEnable() {
        currentState?.CurrentEmotionUI(true);
        if (mManager.IsPassOutCalled) ChangeState(PassOut3DState);
    }
    private void OnDisable() {
        currentState?.CurrentEmotionUI(false);
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

    private void OnDrawGizmos() {

        Gizmos.color = Color.blue; // 파란색으로 설정
        Gizmos.DrawWireSphere(originPos, radius); // 구 그리기
    }
}
