using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationState_2DClimb : StateMachineBehaviour {

    private GameObject player2D;
    private Transform player2DTranform;

    Vector3 movingPosition;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        player2D = animator.transform.gameObject;
        player2DTranform = player2D.transform;

        movingPosition = new Vector3(player2DTranform.localScale.x * 1.5f, 2f, 0f);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

        player2D.transform.position += movingPosition;

        }
    }

