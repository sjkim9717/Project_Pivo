using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class MonsterControl : MonoBehaviour {
    [SerializeField] protected float radius;
    protected Vector3 originPos;

    public Camera MainCamera { get; protected set; }
    public GameObject Monster { get; protected set; }
    public NavMeshAgent NavMesh { get; protected set; }
    public Transform Player2D;
    public Transform Player3D;

    public MonsterManager mManager;
    protected PlayerManage playerManage;

    protected IMonsterStateBase currentState;
    public IMonsterStateBase CurrentState { get { return currentState; } }

    #region 3D
    public Monster3DState_Idle Idle3DState { get; protected set; }
    public Monster3DState_Chase Chase3DState { get; protected set; }
    public Monster3DState_Attack Attack3DState { get; protected set; }
    public Monster3DState_PassOut PassOut3DState { get; protected set; }
    #endregion

    #region 2D
    public Monster2DState_Idle Idle2DState { get; protected set; }
    public Monster2DState_Chase Chase2DState { get; protected set; }
    public Monster2DState_Attack Attack2DState { get; protected set; }
    public Monster2DState_PassOut PassOut2DState { get; protected set; }
    #endregion

    protected virtual void Awake() {
        playerManage = FindObjectOfType<PlayerManage>();
        if (playerManage == null) Debug.LogWarning(" player manage null error in MonsterControl");

        CinemachineBrain brain = GameObject.Find("CameraGroup/MainCamera").GetComponent<CinemachineBrain>();
        if (brain == null) {
            Debug.LogError("CinemachineVirtualCamera not found!");
        }

        MainCamera = brain.GetComponent<Camera>();
        if (MainCamera == null) {
            Debug.LogError("Camera not found on CinemachineVirtualCamera!");
        }

        originPos = base.transform.position;

        Player2D = playerManage.Player2D.transform;
        Player3D = playerManage.Player3D.transform;

        mManager = GetComponentInParent<MonsterManager>();

        radius = Vector3.Distance(transform.position, mManager.PutPoint.position) - 0.7f;
    }

    protected abstract void Start();
    protected abstract void Update();
    public abstract void ChangeState(IMonsterStateBase newState);

    public void IsPassOut() {
        Debug.Log(" Is pass out invoked and monster control");
    }

}
