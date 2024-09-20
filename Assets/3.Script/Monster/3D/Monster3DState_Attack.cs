using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster3DState_Attack : IMonsterStateBase {
    private float moveSpeed = 0.5f;
    private float distanceToPlayer = 0.5f;

    private Transform player3d;

    private Vector3 putPoint;
    private Vector3 distance;

    public Monster3DState_Attack(Transform player3d, Vector3 putPoint) {
        this.player3d = player3d;
        this.putPoint = putPoint;
    }


    public void EnterState(MonsterControl MControl) {
        //MonsterManager.instance.Emotion.transform.GetChild(0).position = MonsterManager.instance.EmotionPoint3D.position;
        MonsterManager.instance.Emotion.transform.GetChild(0).gameObject.SetActive(true);

        player3d.GetComponent<Player3DControl>().ChangeState(PlayerState.Attacked);

        distance = (player3d.position - MControl.transform.position).normalized;

        //TODO: 플레이어 목숨 하나 줄어야함 => 플레이어쪽에서 할까
    }

    public void UpdateState(MonsterControl MControl) {
        Vector3 targetPlayerPosition = MControl.transform.position + distance * distanceToPlayer;

        if (MControl.transform.position != putPoint) {
            MControl.transform.position = Vector3.Slerp(MControl.transform.position, putPoint, moveSpeed);
            player3d.position = Vector3.Slerp(player3d.position, targetPlayerPosition, moveSpeed);
        }
        else {
            MControl.ChangeState(MControl.Idle3DState);
        }
    }

    public void ExitState(MonsterControl MControl) {
        MonsterManager.instance.Emotion.transform.GetChild(0).gameObject.SetActive(false);

        player3d.GetComponent<Player3DControl>().ChangeState(PlayerState.Idle);
    }

}
