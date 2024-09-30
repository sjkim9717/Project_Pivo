using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster2DState_PassOut : IMonsterStateBase {
    private GameObject monster;

    private float iconDistance = 5f;
    private Vector3 emotionPos;

    private Camera camera;
    private MonsterManager mManager;
    private RectTransform emotionOriginPos;

    public Monster2DState_PassOut(MonsterManager mManager, Camera camera, GameObject monster) {
        this.mManager = mManager;
        this.monster = monster;
        this.camera = camera;
        emotionPos = mManager.EmotionPoint2D.position;
        emotionOriginPos = mManager.Emotion.transform.GetChild(2).GetComponent<RectTransform>();
    }

    public void EnterState(MonsterControl MControl) {
        emotionOriginPos.gameObject.SetActive(true);
        mManager.Ani2D.SetBool("IsDead", true);
    }

    public void UpdateState(MonsterControl MControl) {
        if (CheckMonsterInCamera(monster)) SettingEmotion();
    }

    public void ExitState(MonsterControl MControl) {
        emotionOriginPos.gameObject.SetActive(false);
        mManager.Ani2D.SetBool("IsDead", false);
    }

    public bool CheckMonsterInCamera(GameObject gameObject) {
        if (camera == null) return false;
        //if (PlayerManage.instance.CurrentMode != PlayerMode.Player2D) return false;

        Vector3 screenPoint = camera.WorldToViewportPoint(gameObject.transform.position);
        bool isInScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        return isInScreen;
    }

    public void SettingEmotion() {
        Vector3 wantToMovePos = camera.WorldToScreenPoint(emotionPos);                             // 3D 공간의 원하는 위치를 스크린 좌표로 변환

        emotionOriginPos.position = new Vector2(wantToMovePos.x, wantToMovePos.y + iconDistance);
        //Debug.Log($"emotionPos: {emotionPos}, wantToMovePos: {wantToMovePos},  emotionOriginPos: { emotionOriginPos.position}");
    }
    public void CurrentEmotionUI(bool active) {
        emotionOriginPos.gameObject.SetActive(active);
    }
}
