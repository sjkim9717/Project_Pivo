using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerState2D_Idle : PlayerState2D {
    private GameObject interactionObj;

    protected override void OnEnable() {
        base.OnEnable();
        Debug.Log("!!!2DIdle!!!");
        Input.ResetInputAxes();
    }
    private void Update() {
        horizontalInput = Input.GetAxis("Horizontal");
        skillSectionInput = Input.GetAxis("SkillSection");
        interactionInput = Input.GetAxis("Interaction");

        ChangeState();
    }

    private void ChangeState() {
        if (PlayerManage.instance.CurrentState == PlayerState.Dead) {
            return;
        }
        else if (PlayerManage.instance.CurrentState == PlayerState.Disable) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Control2D.ChangeState(PlayerState.Idle);
            }
            else {
                return;
            }
        }


        if (Control2D.CheckGroundPointsEmpty(10f)) {    // 플레이어가 떨어지는지확인

            float distance = Control2D.PlayerRigid.position.y - PlayerManage.instance.Respawnposition.position.y;
            if (distance <= 0.1f) {
                return;
            }
            else {
                Debug.Log(distance);
                Control2D.ChangeState(PlayerState.Falling);
            }
        }
        else if (horizontalInput != 0 ) {
            Control2D.ChangeState(PlayerState.Move);
        }
        else if (skillSectionInput != 0) {
            Control2D.ChangeState(PlayerState.Skill);
        }
        else if (interactionInput != 0) {
            interactionObj = Control2D.CheckInteractObject();
            if (interactionObj != null) {
                string tagName = interactionObj.transform.Find("Root3D").tag;
                if (tagName == "ClimbObj") {
                    Control2D.ChangeState(PlayerState.Climb);
                }
                else if (tagName == "PushBox") {
                    Control2D.ChangeState(PlayerState.PushBox);
                }
                else if (tagName == "Bomb") {
                    Control2D.ChangeState(PlayerState.Bomb);
                }
                else if (tagName == "OpenPanel") {
                    Control2D.ChangeState(PlayerState.OpenPanel);
                }
                else {
                    Debug.LogWarning(tagName);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) {
            Control2D.ChangeState(PlayerState.Disable);
        }

    }

}
