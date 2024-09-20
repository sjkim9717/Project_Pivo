using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster3DState_Idle : IMonsterStateBase {

    private float radius;
    private float moveSpeed = 0.01f;
    private LayerMask layerMask;
    private Vector3 originPos;

    public Monster3DState_Idle(Vector3 originPos, float radius) {
        this.originPos = originPos;
        this.radius = radius;
        layerMask = LayerMask.GetMask("3DPlayer");
    }

    public void EnterState(MonsterControl MControl) {
        //MonsterManager.instance.Emotion.transform.GetChild(1).position = MonsterManager.instance.EmotionPoint3D.position;
        MonsterManager.instance.Emotion.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void UpdateState(MonsterControl MControl) {
        if (MControl.transform.position != originPos) {
            MControl.transform.position = Vector3.Slerp(MControl.transform.position, originPos, moveSpeed);
        }

        Collider[] colliders = Physics.OverlapSphere(originPos, radius, layerMask);
        if (colliders.Length > 0) {
            foreach (Collider item in colliders) {
                if (item.transform.position.y >= MControl.transform.position.y - 0.5f) {
                    MControl.ChangeState(MControl.Chase3DState);
                }
            }
        }


    }

    public void ExitState(MonsterControl MControl) {
        MonsterManager.instance.Emotion.transform.GetChild(1).gameObject.SetActive(false);
    }
}


