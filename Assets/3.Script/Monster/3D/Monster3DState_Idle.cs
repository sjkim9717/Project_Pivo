using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster3DState_Idle : IMonsterStateBase {
    private GameObject monster;

    private float radius;
    private float moveSpeed = 0.01f;
    private float iconDistance = 5f;

    private Vector3 originPos;
    private Vector3 emotionPos;
    private LayerMask layerMask;

    private Camera camera;
    private NavMeshAgent navMesh;
    private RectTransform emotionOriginPos;

    public Monster3DState_Idle(Camera camera, NavMeshAgent navMesh, GameObject monster, Vector3 originPos, float radius) {
        this.monster = monster;
        this.navMesh = navMesh;
        this.originPos = originPos;
        this.radius = radius;
        this.camera = camera;
        layerMask = LayerMask.GetMask("3DPlayer");
        emotionPos = MonsterManager.instance.EmotionPoint3D.position;
        emotionOriginPos = MonsterManager.instance.Emotion.transform.GetChild(1).GetComponent<RectTransform>();

    }

    public void EnterState(MonsterControl MControl) {
        MonsterManager.instance.Ani3D.Rebind();

        navMesh.isStopped = true;                               // 이동 중지
        navMesh.ResetPath();                                    // 경로 초기화
    }

    public void UpdateState(MonsterControl MControl) {
        if (MControl.transform.position != originPos) {
            MControl.transform.position = Vector3.Slerp(MControl.transform.position, originPos, moveSpeed * Time.deltaTime);
            MControl.transform.localPosition = Vector3.zero;
        }

        if (CheckMonsterInCamera(monster)) {
            SettingEmotion();

            if (!emotionOriginPos.gameObject.activeSelf) emotionOriginPos.gameObject.SetActive(true);

            Collider[] colliders = Physics.OverlapSphere(originPos, radius, layerMask);
            if (colliders.Length > 0) {
                foreach (Collider item in colliders) {
                    if (item.transform.position.y >= MControl.transform.position.y - 0.5f) {
                        MControl.ChangeState(MControl.Chase3DState);
                    }
                }
            }
        }
    }

    public void ExitState(MonsterControl MControl) {
        emotionOriginPos.gameObject.SetActive(false);
    }

    public bool CheckMonsterInCamera(GameObject gameObject) {
        if (camera == null) return false;
        if (PlayerManage.instance.CurrentMode != PlayerMode.Player3D) return false;

        Vector3 screenPoint = camera.WorldToViewportPoint(gameObject.transform.position);
        bool isInScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        return isInScreen;
    }

    public void SettingEmotion() {
        Vector3 wantToMovePos = camera.WorldToScreenPoint(emotionPos);                             // 3D 공간의 원하는 위치를 스크린 좌표로 변환

        emotionOriginPos.position = new Vector2(wantToMovePos.x, wantToMovePos.y + iconDistance);
        //Debug.Log($"emotionPos: {emotionPos}, wantToMovePos: {wantToMovePos},  emotionOriginPos: { emotionOriginPos.position}");

        /*
         
        // 캔버스가 Screen Space - Overlay 모드일 경우, 카메라는 null로 설정
        bool validConversion = RectTransformUtility.ScreenPointToWorldPointInRectangle(emotionOriginPos, wantToMovePos, null, out Vector3 convertPos);

        if (validConversion) {
            RectTransform parentRect = emotionOriginPos.parent.GetComponent<RectTransform>();
            Vector3 localPoint = parentRect.InverseTransformPoint(convertPos);

            Vector3 parentScale = emotionOriginPos.parent.localScale;           // 부모의 스케일을 가져옴

            localPoint.x *= parentScale.x;                                      // 부모의 스케일을 고려하여 convertPos 보정
            localPoint.y *= parentScale.y;
           
            emotionOriginPos.anchoredPosition = localPoint;                     // 보정된 값을 anchoredPosition에 적용
        }
        else {
            Debug.LogError("스크린 좌표에서 로컬 좌표로의 변환이 실패했습니다.");
        }
        Debug.DrawLine(emotionPos, camera.ScreenToWorldPoint(new Vector3(wantToMovePos.x, wantToMovePos.y, camera.nearClipPlane)), Color.red, 5f);

        Debug.Log($"emotionPos: {emotionPos}, wantToMovePos: {wantToMovePos}, convertPos: {convertPos},  anchoredPosition: { emotionOriginPos.anchoredPosition}");
         */
    }


}

