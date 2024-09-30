using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster3DState_Chase : IMonsterStateBase {
    private GameObject monster;

    private float radius;
    private float iconDistance; 


    private Vector3 originPos;
    private Vector3 emotionPos;
    private Vector3 emoDirection;

    private LayerMask layerMask;

    private Camera camera;
    private Transform player3d;
    private NavMeshAgent navMesh;
    private MonsterManager mManager;
    private RectTransform emotionOriginPos;

    public Monster3DState_Chase(MonsterManager mManager, Camera camera, NavMeshAgent navMesh, GameObject monster, Transform player3d, Vector3 originPos, float radius) {
        this.monster = monster;
        this.navMesh = navMesh;
        this.player3d = player3d;
        this.originPos = originPos;
        this.radius = radius;
        this.camera = camera;
        this.mManager = mManager;
        layerMask = LayerMask.GetMask("3DPlayer");

        emotionPos = mManager.EmotionPoint3D.position;
        emotionOriginPos = mManager.Emotion.transform.GetChild(0).GetComponent<RectTransform>();

        iconDistance = Vector3.Distance(monster.transform.position, emotionPos);
        emoDirection = ( monster.transform.position - emotionPos).normalized;
    }

    public void EnterState(MonsterControl MControl) {
        navMesh.isStopped = false;
        emotionOriginPos.gameObject.SetActive(true);
        mManager.Ani3D.SetBool("IsMove", true);
    }
    public void UpdateState(MonsterControl MControl) {

        if (CheckMonsterInCamera(monster)) SettingEmotion();
        else emotionOriginPos.gameObject.SetActive(false);

        navMesh.SetDestination(player3d.position); // 목표를 다시 설정

        if (!navMesh.pathPending) {
            if (navMesh.remainingDistance <= navMesh.stoppingDistance) { // 몬스터가 목표에 도달했는지 확인 후 상태 변경
                MControl.ChangeState(MControl.Attack3DState);
            }

            // 플레이어가 멀어졌을 경우 idle 상태로 돌아가야 함
            Collider[] colliders = Physics.OverlapSphere(originPos, radius, layerMask);
            if (colliders.Length > 0) {
                foreach (Collider item in colliders) {
                    if (item.transform.position.y <= MControl.transform.position.y - 0.5f || item.transform.position.y >= MControl.transform.position.y + 1.7f) {
                        //Debug.Log("player is lower than monster | " + item.transform.position.y + " | " + MControl.transform.position.y);

                        MControl.ChangeState(MControl.Idle3DState);
                        return;
                    }
                }
            }
            else {
                MControl.ChangeState(MControl.Idle3DState);
                return;
            }
        }

    }

    public void ExitState(MonsterControl MControl) {
        navMesh.isStopped = true;                               // 이동 중지
        navMesh.ResetPath();                                    // 경로 초기화
        emotionOriginPos.gameObject.SetActive(false);
        mManager.Ani3D.SetBool("IsMove", false);
    }

    public bool CheckMonsterInCamera(GameObject gameObject) {
        if (camera == null) return false;
        //if (PlayerManage.instance.CurrentMode != PlayerMode.Player3D) return false;

        Vector3 screenPoint = camera.WorldToViewportPoint(gameObject.transform.position);
        bool isInScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        return isInScreen;
    }

    public void SettingEmotion() {
        // emotion position 
        Vector3 targetPos = monster.transform.position - emoDirection * iconDistance;
        emotionPos = targetPos;

        Vector3 wantToMovePos = camera.WorldToScreenPoint(emotionPos);                             // 3D 공간의 원하는 위치를 스크린 좌표로 변환

        emotionOriginPos.position = new Vector2(wantToMovePos.x, wantToMovePos.y + 5f);
        //Debug.Log($"emotionPos: {emotionPos}, wantToMovePos: {wantToMovePos},  emotionOriginPos: { emotionOriginPos.position}");

    }
    public void CurrentEmotionUI(bool active) {
        emotionOriginPos.gameObject.SetActive(active);
    }
}
