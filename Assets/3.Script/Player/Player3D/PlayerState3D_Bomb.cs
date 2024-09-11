using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState3D_Bomb : PlayerState3D {
    private bool isBombMoved;
    private IBomb bomb;
    protected override void OnEnable() {
        base.OnEnable();
        Input.ResetInputAxes();
        isBombMoved = false;
    }

    public override void EnterState() {

        Control3D.Ani3D.SetBool("IsBomb", true);

        if (Control3D.InteractionObject.TryGetComponent(out IBomb bombComponent)) {
            bomb = Control3D.InteractionObject.GetComponent<IBomb>();
        }
        else {
            GameObject bombObj = Control3D.InteractionObject.GetComponent<BombSpawner>().Bomb;
            bomb = bombObj.GetComponent<IBomb>();
        }
    }

    private void FixedUpdate() {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        interactionInput = Input.GetAxis("Interaction");
        ChangeState();
    }

    private void ChangeState() {
        if (PlayerManage.instance.CurrentState == PlayerState.Dead) {
            return;
        }


        if (Control3D.CheckGroundPointsEmpty(10f)) {    // 플레이어가 떨어지는지확인
            if (isBombMoved) {
                bomb.IBombMoveEnd();
            }
            Control3D.ChangeState(PlayerState.Falling);
        }
        else if (horizontalInput != 0 || verticalInput != 0) {
            Control3D.Ani3D.SetBool("IsMoveObj", true);
            isBombMoved = true;
            Control3D.Move(horizontalInput, verticalInput);
            bomb.IBombMoving();
        }
        else if (horizontalInput == 0 && verticalInput == 0) {
            Control3D.Ani3D.SetBool("IsMoveObj", false);
            Control3D.Move(0, 0);
        }

        if (isBombMoved) {
            if (interactionInput != 0) {
                bomb.IBombMoveEnd();
                PlayerManage.instance.SetPlantBomb(bomb);
                Control3D.ChangeState(PlayerState.Idle);
            }
        }
    }


    public override void ExitState() {
        Control3D.Ani3D.SetBool("IsBomb", false);
        bomb = null;
    }

}
