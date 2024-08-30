using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationState_2DClimb : StateMachineBehaviour {

    private GameObject player2D;
    private Transform player2DTranform;

    private Player2DController player2DController;

    Vector3 movingPosition;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        player2D = animator.transform.gameObject;
        player2DTranform = player2D.transform;
        player2DController = player2D.GetComponent<Player2DController>();

        movingPosition = new Vector3(player2DTranform.localScale.x * 1f, 2f, 0f);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

        player2D.transform.position += movingPosition;

        player2DController.IsClimb = false;
        if (player2DController.IsClimb) {

        }
    }



}
