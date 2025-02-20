using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster2DState_Idle : IMonsterStateBase {
    private GameObject monster;

    private float radius;
    private float iconDistance = 5f;
    private Vector3 originPos;
    private Vector3 emotionPos;
    private Vector3 emoDirection;
    private LayerMask layerMask;

    private Camera camera;
    private NavMeshAgent navMesh;
    private MonsterManager mManager;
    private RectTransform emotionOriginPos;

    public Monster2DState_Idle(MonsterManager mManager, Camera camera, NavMeshAgent navMesh, GameObject monster, Vector3 originPos, float radius) {
        this.monster = monster;
        this.navMesh = navMesh;
        this.originPos = originPos;
        this.radius = radius;
        this.camera = camera;
        this.mManager = mManager;
        layerMask = LayerMask.GetMask("2DPlayer");
        emotionPos = mManager.EmotionPoint2D.position;
        emotionOriginPos = mManager.Emotion.transform.GetChild(1).GetComponent<RectTransform>();

        iconDistance = Vector3.Distance(monster.transform.position, emotionPos);
        emoDirection = (monster.transform.position - emotionPos).normalized;
    }

    public void EnterState(MonsterControl MControl) {
        mManager.Ani2D.Rebind();

        navMesh.isStopped = true;                               // 이동 중지
        navMesh.ResetPath();                                    // 경로 초기화
    }
    public void UpdateState(MonsterControl MControl) {
        if (MControl.transform.position != originPos) {
            navMesh.SetDestination(originPos); // 목표를 다시 설정
        }

        if (navMesh.remainingDistance <= navMesh.stoppingDistance) { // 몬스터가 목표에 도달했는지 확인 후 상태 변경
            MControl.transform.position = originPos;
            Quaternion originRotation = Quaternion.LookRotation(-Vector3.forward);
            MControl.transform.rotation = originRotation;
        }
    

        if (CheckMonsterInCamera(monster)) {
            SettingEmotion();

            if (!emotionOriginPos.gameObject.activeSelf) emotionOriginPos.gameObject.SetActive(true);

            Collider2D[] colliders = Physics2D.OverlapCircleAll(originPos, radius, layerMask);
            if (colliders.Length > 0) {
                foreach (Collider2D item in colliders) {
                    if (item.transform.position.y >= MControl.transform.position.y - 0.5f && item.transform.position.y <= MControl.transform.position.y + 1.7f) {
                        MControl.ChangeState(MControl.Chase2DState);
                    }
                }
            }
        }
        else emotionOriginPos.gameObject.SetActive(false);
    }

    public void ExitState(MonsterControl MControl) {
        emotionOriginPos.gameObject.SetActive(false);
    }

    public bool CheckMonsterInCamera(GameObject gameObject) {
        if (camera == null) return false;
        //if (PlayerManage.instance.CurrentMode != PlayerMode.Player2D) return false;

        Vector3 screenPoint = camera.WorldToViewportPoint(gameObject.transform.position);
        bool isInScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        return isInScreen;
    }

    public void SettingEmotion() {
        // emotion position 
        if (!emotionOriginPos.gameObject.activeSelf) emotionOriginPos.gameObject.SetActive(true);
        Vector3 targetPos = monster.transform.position - emoDirection * iconDistance;
        emotionPos = targetPos;

        Vector3 wantToMovePos = camera.WorldToScreenPoint(emotionPos);                             // 3D 공간의 원하는 위치를 스크린 좌표로 변환

        emotionOriginPos.position = new Vector2(wantToMovePos.x, wantToMovePos.y + iconDistance);
    }
    public void CurrentEmotionUI(bool active) {
        emotionOriginPos.gameObject.SetActive(active);
    }
}
