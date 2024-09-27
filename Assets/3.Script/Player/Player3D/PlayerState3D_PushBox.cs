using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState3D_PushBox : PlayerState3D {

    private bool isButtonPressed = false;
    private IPushBox pushBox;

    protected override void OnEnable() {
        base.OnEnable();
        Input.ResetInputAxes();
    }
    public override void EnterState() {
        SettingPlayerPosition();
        Control3D.Ani3D.SetBool("IsPushBox", true);
        isButtonPressed = false;

        pushBox = Control3D.InteractionObject.GetComponentInParent<IPushBox>();
    }


    private void Update() {

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        interactionInput = Input.GetAxis("Interaction");

        ChangeState();
    }

    private void ChangeState() {
        if (PlayerManage.instance.CurrentState == PlayerState.Dead) {
            return;
        }

        Control3D.Move(0, 0);


        if ((horizontalInput != 0 || verticalInput != 0) && !isButtonPressed) {
            isButtonPressed = true;

            if (pushBox != null) {
                pushBox.IInteractionPushBox(horizontalInput, verticalInput);
            }
        }
        else if (interactionInput != 0) {
            Control3D.ChangeState(PlayerState.Idle);
        }

        if ((horizontalInput == 0 && verticalInput == 0) && isButtonPressed) {                           // 스킬 버튼이 해제되었는지 감지
            isButtonPressed = false;                                               // 버튼 눌림 상태를 초기화
        }

    }

    private void SettingPlayerPosition() {
        Vector3 positionSet = Control3D.InteractionObject.transform.GetChild(1).position;

        Vector3 newPosition = new Vector3(positionSet.x, positionSet.y + 0.8f, positionSet.z);
        Control3D.PlayerRigid.MovePosition(newPosition);

        Transform switchTrans = Control3D.InteractionObject.transform.GetChild(1).transform;
        Vector3 oppositeDirection = -switchTrans.forward;  // z축 방향의 반대

        // 새로운 회전값을 계산하고 적용
        Quaternion targetRotation = Quaternion.LookRotation(oppositeDirection);
        Control3D.PlayerRigid.MoveRotation(targetRotation);
    }


    public override void ExitState() {
        Control3D.Ani3D.SetBool("IsPushBox", false);
        pushBox = null;
    }
}
