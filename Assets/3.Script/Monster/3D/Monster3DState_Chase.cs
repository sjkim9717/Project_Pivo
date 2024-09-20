using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster3DState_Chase : IMonsterStateBase {
    private float radius;
    private Vector3 originPos;
    private LayerMask layerMask;

    private Transform player3d;
    private NavMeshAgent navMesh;

    public Monster3DState_Chase(NavMeshAgent navMesh, Transform player3d, Vector3 originPos, float radius) {
        this.navMesh = navMesh;
        this.player3d = player3d;
        this.originPos = originPos;
        this.radius = radius;
        layerMask = LayerMask.GetMask("3DPlayer");
    }

    public void EnterState(MonsterControl MControl) {
        //TODO: 이모지 위치 맞춰야함
        //MonsterManager.instance.Emotion.transform.GetChild(0).position = MonsterManager.instance.EmotionPoint3D.position;
        MonsterManager.instance.Emotion.transform.GetChild(0).gameObject.SetActive(true);
    }
    public void UpdateState(MonsterControl MControl) {
        navMesh.SetDestination(player3d.position); // 목표를 다시 설정

        // 몬스터가 목표에 도달했는지 확인
        if (navMesh.remainingDistance <= navMesh.stoppingDistance) {
            // 도착했을 때 상태 변경
            MControl.ChangeState(MControl.Attack3DState);
        }


        // 플레이어가 멀어졌을 경우 idle 상태로 돌아가야 함
        Collider[] colliders = Physics.OverlapSphere(originPos, radius, layerMask);
        if (colliders.Length>0) {
            foreach (Collider item in colliders) {
                if (item.transform.position.y <= MControl.transform.position.y - 0.2f) {
                    Debug.Log("player is lower than monster | " + item.transform.position.y + " | " + MControl.transform.position.y);
                    navMesh.isStopped = true; // 이동을 중지
                    navMesh.ResetPath(); // 경로를 초기화
                    MControl.ChangeState(MControl.Idle3DState);
                    return;
                }
                else {

                    Debug.Log("player is upper than monster | " + item.transform.position.y + " | " + MControl.transform.position.y);
                }
            }
        }
        else {  // 범위내 아무것도 없어도 중지
            navMesh.isStopped = true; // 이동을 중지
            navMesh.ResetPath(); // 경로를 초기화
            MControl.ChangeState(MControl.Idle3DState);
        }
    }

    public void ExitState(MonsterControl MControl) {
        navMesh.isStopped = false;
        MonsterManager.instance.Emotion.transform.GetChild(0).gameObject.SetActive(false);

    }


}
