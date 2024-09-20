using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class MonsterControl : MonoBehaviour {
    [SerializeField] private float radius;
    private Vector3 originPos;
    public NavMeshAgent NavMesh { get; private set; }
    public Transform Player2D { get { return PlayerManage.instance.Player2D.transform; } }
    public Transform Player3D { get { return PlayerManage.instance.Player3D.transform; } }

    protected IMonsterStateBase currentState;
    public IMonsterStateBase CurrentState { get { return currentState; } }

    #region 3D
    public Monster3DState_Idle Idle3DState { get; private set; }
    public Monster3DState_Chase Chase3DState { get; private set; }
    public Monster3DState_Attack Attack3DState { get; private set; }
    public Monster3DState_PassOut PassOut3DState { get; private set; }
    #endregion

    #region 2D

    public Monster2DState_Idle Idle2DState { get; private set; }
    public Monster2DState_Chase Chase2DState { get; private set; }
    public Monster2DState_Attack Attack2DState { get; private set; }
    public Monster2DState_PassOut PassOut2DState { get; private set; }
    #endregion

    protected virtual void Awake() {
        originPos = base.transform.position;
        Debug.LogWarning("origin position | " + originPos);

        NavMesh = base.transform.GetComponentInParent<NavMeshAgent>();
        radius = Vector3.Distance(transform.position, MonsterManager.instance.PutPoint.position) - 0.5f;

        // 상태 인스턴스 생성 및 캐싱
        Idle3DState = new Monster3DState_Idle(originPos, radius);
        Chase3DState = new Monster3DState_Chase(NavMesh, Player3D, originPos, radius);
        Attack3DState = new Monster3DState_Attack(Player3D, MonsterManager.instance.PutPoint.position);
        PassOut3DState = new Monster3DState_PassOut();


        Idle2DState = new Monster2DState_Idle();
        Chase2DState = new Monster2DState_Chase();
        Attack2DState = new Monster2DState_Attack();
        PassOut2DState = new Monster2DState_PassOut();

    }


    protected abstract void Start();
    protected abstract void Update();
    public abstract void ChangeState(IMonsterStateBase newState);

}
