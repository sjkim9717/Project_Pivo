using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState2D_Falling : PlayerState2D {
    protected override void OnEnable() {
        base.OnEnable();
    }
    public override void EnterState() {
        Control2D.Ani2D.SetBool("IsFalling", true);
    }
    private void Update() {
        ChangeState();
    }


    private void ChangeState() {
        if (playerManage.CurrentState == PlayerState.Dead) {
            return;
        }


        Control2D.PlayerRigid.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (Control2D.PlayerRigid.position.y <= -20f) {
            // respawn
            playerManage.SetPlayerDieCount();

            Control2D.PlayerRigid.position = playerManage.Respawnposition.position;
            Control2D.PlayerRigid.velocity = Vector3.zero;

            Control2D.GroundPoint.transform.localPosition = Vector3.zero;

            float distance = Control2D.PlayerRigid.position.y - playerManage.Respawnposition.position.y;

            if (distance <= 0.1f) {
                Control2D.ChangeState(PlayerState.Idle);
            }
        }
    }

    public override void ExitState() {
        Control2D.Ani2D.SetBool("IsFalling", false);
    }


}
