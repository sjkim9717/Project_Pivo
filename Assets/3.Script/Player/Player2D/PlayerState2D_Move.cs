using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState2D_Move : PlayerState2D {
    protected override void OnEnable() {
        base.OnEnable();
    }

    public override void EnterState() {
        Control2D.Ani2D.SetBool("IsMove", true);
    }
    private void Update() {
        horizontalInput = Input.GetAxis("Horizontal");

        if (Control2D.CheckGroundPointsEmpty(10f)) {    // 플레이어가 떨어지는지확인
            Control2D.ChangeState(PlayerState.Falling);
        }

        ChangeState();
    }

    private void ChangeState() {
        if (PlayerManage.instance.CurrentState == PlayerState.Dead) {
            return;
        }

         if (horizontalInput != 0) {

            PlayerManage.instance.IsChangingModeTo3D = false;
            Control2D.Move(horizontalInput);
        }
        else if (horizontalInput == 0) {
            Control2D.Move(0);
            Control2D.ChangeState(PlayerState.Idle);
        }
    }

    public override void ExitState() {
        Control2D.Ani2D.SetBool("IsMove", false);
    }
}
