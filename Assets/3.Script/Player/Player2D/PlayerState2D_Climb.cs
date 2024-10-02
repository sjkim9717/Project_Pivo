using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState2D_Climb : PlayerState2D {
    protected override void OnEnable() {
        base.OnEnable();
    }

    public override void EnterState() {
        Control2D.Ani2D.SetTrigger("IsClimb");
        AudioManager.instance.Corgi_Play(playerManage.PlayerAudio, "climb");
    }

    private void Update() {
        if (IsAnimationFinished()) {
            Control2D.ChangeState(PlayerState.Idle);
        }
    }

    public bool IsAnimationFinished() {
        AnimatorStateInfo stateInfo = Control2D.Ani2D.GetCurrentAnimatorStateInfo(0); // 레이어 0 기준
        return stateInfo.IsName("Climb") && stateInfo.normalizedTime >= 0.95f;
    }

}
