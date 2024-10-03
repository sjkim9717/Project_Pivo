using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState2D_Falling : PlayerState2D {
    private float cameraYzero;
    protected override void OnEnable() {
        base.OnEnable();

        // camera가 비추는 y축 하단 높이
        cameraYzero = Camera.main.transform.position.y - Camera.main.orthographicSize;
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

        if (Control2D.PlayerRigid.position.y <= cameraYzero) {
            // respawn
            playerManage.SetPlayerDieCount();

            Control2D.PlayerRigid.position = playerManage.Respawnposition.position;
            Control2D.PlayerRigid.velocity = Vector3.zero;

            Control2D.GroundPoint.transform.localPosition = Vector3.zero;
        }

        if (!Control2D.CheckGroundPointsEmpty(0.1f)) {
            Control2D.ChangeState(PlayerState.Idle);
        }
    }

    public override void ExitState() {
        Control2D.Ani2D.SetBool("IsFalling", false);
    }


}
