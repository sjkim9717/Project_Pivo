using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState3D_Idle : PlayerState3D {
    IBomb bomb;

    private float explosionInput;
    private GameObject interactionObj;
    protected override void OnEnable() {
        base.OnEnable();
        Input.ResetInputAxes();
    }

    public override void EnterState() {
        //Control3D.Ani3D.Play("Idle");
    }

    private void Update() {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        skillSectionInput = Input.GetAxis("SkillSection");
        interactionInput = Input.GetAxis("Interaction");
        explosionInput = Input.GetAxis("Explosion");

        if (playerManage.CurrentState == PlayerState.Dead) {
            return;
        }


        if (Control3D.CheckGroundPointsEmpty(10f)) {    // 플레이어가 떨어지는지확인
            if (Vector3.Distance(Control3D.PlayerRigid.position, playerManage.Respawnposition.position) <= 0.1f) {
                return;
            }
            else {
                if (playerManage.IsBombOnGround) {
                    bomb.IBombMoveEnd();
                }
                Debug.Log(Vector3.Distance(Control3D.PlayerRigid.position, playerManage.Respawnposition.position));
                Control3D.ChangeState(PlayerState.Falling);
            }
        }

        ChangeState();
    }

    private void ChangeState() {

        Control3D.Move(0, 0);

        if (horizontalInput != 0 || verticalInput != 0) {
            Control3D.ChangeState(PlayerState.Move);
        }
        else if (skillSectionInput != 0) {
            Control3D.ChangeState(PlayerState.Skill);
        }
        else if (interactionInput != 0) {
            interactionObj = Control3D.InteractionObject;
            if (interactionObj != null) {
                string tagName = interactionObj.transform.Find("Root3D").tag;

                if (tagName == "Climb") {
                    if (Control3D.CheckInteractObject()) {
                        Control3D.ChangeState(PlayerState.Climb);
                    }
                }
                else if (tagName == "PushSwitch") {
                    Control3D.ChangeState(PlayerState.PushBox);
                }
                else if (tagName == "BombSpawner") {
                    GameObject bombObj = Control3D.InteractionObject.GetComponent<BombSpawner>().Bomb;
                    bomb = bombObj.GetComponent<IBomb>();
                    if (bomb != null) {
                        bomb.IBombMoveStart();
                        playerManage.IsBombOnGround = true;
                        Control3D.ChangeState(PlayerState.Bomb);
                    }
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

        if (playerManage.IsBombOnGround) {
            if (explosionInput != 0) {
                playerManage.IsBombOnGround = false;
                bomb.IBombExplosion();
                bomb = null;
            }

        }

    }
}

//TODO: idle 상태에서 bomb 터트리는 상태 해야함