using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState2D_Attacked : PlayerState2D {
    public override void EnterState() {
        // 몬스터와 거리가 가까울 시 플레이어 목숨 하나 줄어듬
        playerManage.SetPlayerDieCount();
    }
    public override void ExitState() {
    }
}
