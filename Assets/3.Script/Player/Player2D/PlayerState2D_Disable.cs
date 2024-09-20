using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState2D_Disable : PlayerState2D {
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Control2D.ChangeState(PlayerState.Idle);
        }
        else {
            return;
        }
    }
}
