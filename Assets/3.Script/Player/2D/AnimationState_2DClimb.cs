using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationState_2DClimb : StateMachineBehaviour {

    private GameObject player2D;
    private GameObject player2DBone;

    private Player2DController player2DController;

    Vector3 boneInitPosition = Vector3.zero;
    Vector3 movingPosition = Vector3.zero;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        player2D = animator.transform.parent.gameObject;
        player2DBone = animator.transform.GetChild(0).gameObject;

        player2DController = player2D.GetComponent<Player2DController>();


        boneInitPosition = player2DBone.transform.position;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (player2DController.IsClimb) {

            Vector3 boneFinalPosition = player2DBone.transform.position;

            movingPosition = boneFinalPosition - boneInitPosition;

            player2D.transform.position += movingPosition;

            player2DController.IsClimb = false;
        }
    }



}
