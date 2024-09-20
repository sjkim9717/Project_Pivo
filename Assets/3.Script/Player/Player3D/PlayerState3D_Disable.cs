using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState3D_Disable : PlayerState3D {
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Control3D.ChangeState(PlayerState.Idle);
        }
        else {
            return;
        }
    }
}
