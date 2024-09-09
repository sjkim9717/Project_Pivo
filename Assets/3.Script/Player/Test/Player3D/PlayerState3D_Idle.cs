using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState3D_Idle : PlayerState3D {
    private GameObject interactionObj;
    protected override void OnEnable() {
        base.OnEnable();
        Debug.Log("!!!!!!");
        Input.ResetInputAxes();
    }

    public override void EnterState() {
        //Control3D.Ani3D.Play("Idle");
    }

    private void Update() {

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        skillSectionInput = Input.GetAxis("SkillSection");
        interactionInput = Input.GetAxis("Climb");

        ChangeState();


    }

    private void ChangeState() {
        if( PlayerManage.instance.CurrentState == PlayerState.Dead) {
            return;
        }
        else if(PlayerManage.instance.CurrentState == PlayerState.Disable) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Control3D.ChangeState(PlayerState.Idle);
            }
            else {
                return;
            }
        }


        if (Control3D.CheckGroundPointsEmpty(10f)) {    // 플레이어가 떨어지는지확인
            if (Vector3.Distance(Control3D.PlayerRigid.position, PlayerManage.instance.Respawnposition.position) <= 0.1f) {
                return;
            }
            else {
                Debug.Log(Vector3.Distance(Control3D.PlayerRigid.position, PlayerManage.instance.Respawnposition.position));
                Control3D.ChangeState(PlayerState.Falling);
            }
        }
        else if(horizontalInput != 0|| verticalInput != 0) {
            Control3D.ChangeState(PlayerState.Move);
        }
        else if(skillSectionInput !=0) {
            Control3D.ChangeState(PlayerState.Skill);
        }
        else if(interactionInput != 0) {
            interactionObj = Control3D.CheckInteractObject();
            if (interactionObj != null ) {
                string tagName = interactionObj.transform.Find("Root3D").tag;
                if (tagName == "ClimbObj") {
                    Control3D.ChangeState(PlayerState.Climb);
                }
                else if (tagName == "PushBox") {
                    Control3D.ChangeState(PlayerState.PushBox);
                }
                else if (tagName == "Bomb") {
                    Control3D.ChangeState(PlayerState.Bomb);
                }
                else if (tagName == "OpenPanel") {
                    Control3D.ChangeState(PlayerState.OpenPanel);
                }
                else {
                    Debug.LogWarning(tagName);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) {
            Control3D.ChangeState(PlayerState.Disable);
        }

    }
}
