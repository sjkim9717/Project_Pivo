using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationState_3DClimb : StateMachineBehaviour {

    private GameObject player3D;
    private GameObject player3DBone;
    private Obstacle3DCheck obstacle3DCheck;
    private Player3DController player3DController;

    Vector3 boneInitPosition = Vector3.zero;
    Vector3 movingPosition = Vector3.zero;

    // 상태에 들어왔을 경우 각 초기화
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        player3D = animator.transform.parent.gameObject;
        player3DBone = animator.transform.GetChild(0).gameObject;

        obstacle3DCheck = player3D.GetComponent<Obstacle3DCheck>();
        player3DController = player3D.GetComponent<Player3DController>();


        boneInitPosition = player3DBone.transform.position;
    }

    // 애니메이션이 끝나서 상태가 종료될 때 호출됨
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

        if (obstacle3DCheck != null && player3DController.IsClimb) {

            Vector3 boneFinalPosition = player3DBone.transform.position;

            movingPosition = boneFinalPosition - boneInitPosition;

            player3D.transform.position += movingPosition;

            obstacle3DCheck.ClimbObstacle = null;
            player3DController.IsClimb = false;
        }
    }

}


/*
 1. 플레이어가 climb 모션을 끝내고 위치가 전환되어야함
//TODO: 위치가 부드럽게 움직여야함
 
1. animator는 player3DBone의 위치를 변경함 
2. player3DBone의 초기 중심 위치가 player3D의 중심과 달라서 해당 위치를 bonePosition 에 위치 차이를 담음
3. animator가 player3DBone의 위치를 변하게 한 값 만큼 exit에서 climbPosition = player3DBone.transform.position - bonePosition;로 차이를 담아서 
4. 변경하고싶은 오브젝트인 player3D의 위치에 
            player3D.transform.position += climbPosition;더해줌 
5. 결과적으로 animation가 끝났던 최종 위치와 player3D의 위치와 다름

 */
