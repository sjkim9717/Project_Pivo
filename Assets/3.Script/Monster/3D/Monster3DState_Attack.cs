using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster3DState_Attack : IMonsterStateBase {
    private GameObject monster;

    private float moveSpeed = 0.5f;
    private float currentT = 0f;
    private float distanceToPlayer = 0.5f;
    private float iconDistance = 5f;

    private Transform player3d;

    private Vector3 distance;
    private Vector3 putPoint;
    private Vector3 emotionPos;

    private Camera camera;
    private NavMeshAgent navMesh;
    private MonsterManager mManager;
    private RectTransform emotionOriginPos;

    public Monster3DState_Attack(MonsterManager mManager, Camera camera, NavMeshAgent navMesh, GameObject monster, Transform player3d, Vector3 putPoint) {
        this.monster = monster;
        this.navMesh = navMesh;
        this.player3d = player3d;
        this.putPoint = putPoint;
        this.camera = camera;
        this.mManager = mManager;

        emotionPos = mManager.EmotionPoint3D.position;
        emotionOriginPos = mManager.Emotion.transform.GetChild(0).GetComponent<RectTransform>();
    }


    public void EnterState(MonsterControl MControl) {
        //MonsterManager.instance.Emotion.transform.GetChild(0).position = MonsterManager.instance.EmotionPoint3D.position;
        emotionOriginPos.gameObject.SetActive(true);
        mManager.Ani3D.SetBool("IsAttack", true);

        player3d.GetComponent<Player3DControl>().ChangeState(PlayerState.Attacked);

        distance = (player3d.position - MControl.transform.position).normalized;

        navMesh.isStopped = true;                               // 이동 중지
        navMesh.ResetPath();                                    // 경로 초기화        
    }

    public void UpdateState(MonsterControl MControl) {
        if (CheckMonsterInCamera(monster)) SettingEmotion();
        else emotionOriginPos.gameObject.SetActive(false);

        Vector3 targetPlayerPosition = putPoint + distance * distanceToPlayer;

        float tIncrement = moveSpeed * Time.deltaTime;
        currentT += tIncrement;
        //Debug.Log($"CurrentT: {currentT}, Player Position: {player3d.position}");

        currentT = Mathf.Clamp(currentT, 0f, 1f);

        // Lerp를 사용하여 위치 이동
        MControl.transform.position = Vector3.Lerp(MControl.transform.position, putPoint, currentT);
        player3d.position = Vector3.Lerp(player3d.position, targetPlayerPosition, currentT);

        // 목표에 도달했을 때 상태 변경
        if (currentT >= 1f) {
            MControl.ChangeState(MControl.Idle3DState);
        }
    }

    public void ExitState(MonsterControl MControl) {
        emotionOriginPos.gameObject.SetActive(false);

        player3d.GetComponent<Player3DControl>().ChangeState(PlayerState.Idle);
        mManager.Ani3D.SetBool("IsAttack", false);
    }
    public bool CheckMonsterInCamera(GameObject gameObject) {
        if (camera == null) return false;
        //if (PlayerManage.instance.CurrentMode != PlayerMode.Player3D) return false;

        Vector3 screenPoint = camera.WorldToViewportPoint(gameObject.transform.position);
        bool isInScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        return isInScreen;
    }

    public void SettingEmotion() {
        if (!emotionOriginPos.gameObject.activeSelf) emotionOriginPos.gameObject.SetActive(true);
        Vector3 wantToMovePos = camera.WorldToScreenPoint(emotionPos);                             // 3D 공간의 원하는 위치를 스크린 좌표로 변환

        emotionOriginPos.position = new Vector2(wantToMovePos.x, wantToMovePos.y + iconDistance);
        //Debug.Log($"emotionPos: {emotionPos}, wantToMovePos: {wantToMovePos},  emotionOriginPos: { emotionOriginPos.position}");

    }
    public void CurrentEmotionUI(bool active) {
        emotionOriginPos.gameObject.SetActive(active);
    }
}
