using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState3D_Idle : PlayerState3D {

    float horizontalInput;
    float verticalInput;
    float skillSectionInput ;
    float interactionInput;

    public override void EnterState() {
        Control3D.Ani3D.Play("Idle");
    }

    private void Update() {

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        skillSectionInput = Input.GetAxis("SkillSection");
        interactionInput = Input.GetAxis("Climb");

        ChangeState();

    }

    private void ChangeState() {
        if( PlayerManage.instance.CurrentState == PlayerState.Dead) {
            return;
        }
        else if(PlayerManage.instance.CurrentState == PlayerState.Disable) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Control3D.ChangeState(PlayerState.Idle);
            }
            else {
                return;
            }
        }

        Control3D.Move(0, 0);

        if (Control3D.CheckGroundPointsEmpty(10f)) {    // 플레이어가 떨어지는지확인

            Control3D.ChangeState(PlayerState.Falling);
        }
        else if(horizontalInput != 0|| verticalInput != 0) {
            Control3D.Move(horizontalInput, verticalInput);
            Control3D.ChangeState(PlayerState.Move);

        }
        else if(skillSectionInput !=0) {
            Control3D.ChangeState(PlayerState.Skill);
        }
        else if(interactionInput != 0) {
            Control3D.ChangeState(PlayerState.Interaction);
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) {

            Control3D.ChangeState(PlayerState.Disable);
        }

    }
}
