using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState3D_Climb : PlayerState3D {

    protected override void OnEnable() {
        base.OnEnable();
    }

    public override void EnterState() {
        Control3D.Ani3D.SetTrigger("IsClimb");
        AudioManager.instance.Corgi_Play(playerManage.PlayerAudio, "climb");
    }

    private void Update() {
        if (IsAnimationFinished()) {
            Control3D.ChangeState(PlayerState.Idle);
        }
    }

    public bool IsAnimationFinished() {
        AnimatorStateInfo stateInfo = Control3D.Ani3D.GetCurrentAnimatorStateInfo(0); // 레이어 0 기준
        return stateInfo.IsName("Climb") && stateInfo.normalizedTime >= 0.95f;
    }


}
