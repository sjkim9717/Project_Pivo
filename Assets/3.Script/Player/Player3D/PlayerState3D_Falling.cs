using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState3D_Falling : PlayerState3D {
    protected override void OnEnable() {
        base.OnEnable();
    }
    public override void EnterState() {
        Control3D.Ani3D.SetBool("IsFalling", true);
    }

    private void Update() {
        ChangeState();
    }


    private void ChangeState() {
        if (PlayerManage.instance.CurrentState == PlayerState.Dead) {
            return;
        }


        if (PlayerManage.instance.IsChangingModeTo3D) {          // 2d 에서 3 d 돌아왔을 때 
            Control3D.ChangeState(PlayerState.Holding);
        }
        else {
            Falling();
        }
    }
    public void Falling() {

        Control3D.PlayerRigid.constraints = RigidbodyConstraints.FreezeRotation;

        if (Control3D.PlayerRigid.position.y <= -15f) {
            // respawn
            PlayerManage.instance.SetPlayerDieCount();

            Control3D.PlayerRigid.position = PlayerManage.instance.Respawnposition.position;
            Control3D.PlayerRigid.velocity = Vector3.zero;

            Control3D.GroundPoint.transform.localPosition = Vector3.zero;

            if (Vector3.Distance(Control3D.PlayerRigid.position, PlayerManage.instance.Respawnposition.position) <= 0.1f) {
                Control3D.ChangeState(PlayerState.Idle);
            }
        }
    }

    public override void ExitState() {
        Control3D.Ani3D.SetBool("IsFalling", false);
    }
}
