using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState3D_Move : PlayerState3D {
    protected override void OnEnable() {
        base.OnEnable();
    }

    public override void EnterState() {
        Control3D.Ani3D.SetBool("IsMove", true);
    }

    private void FixedUpdate() {

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        ChangeState();
    }

    private void ChangeState() {
        if (playerManage.CurrentState == PlayerState.Dead) {
            return;
        }


        if (Control3D.CheckGroundPointsEmpty(10f)) {    // 플레이어가 떨어지는지확인

            Control3D.ChangeState(PlayerState.Falling);
        }
        else if (horizontalInput != 0 || verticalInput != 0) {

            playerManage.IsChangingModeTo3D = false;
            Control3D.Move(horizontalInput, verticalInput);
        }
        else if (horizontalInput == 0 && verticalInput == 0) {
            Control3D.Move(0, 0);
            Control3D.ChangeState(PlayerState.Idle);
        }
    }

    public override void ExitState() {
        Control3D.Ani3D.SetBool("IsMove", false);
    }
}
