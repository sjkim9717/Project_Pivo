using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerState2D_Idle : PlayerState2D {

    private float explosionInput;
    private GameObject interactionObj;
    private GameObject holdingGroup;
    protected override void Awake() {
        base.Awake();
        holdingGroup = FindObjectOfType<GameManager>().transform.GetChild(0).GetChild(2).gameObject;
    }
    protected override void OnEnable() {
        base.OnEnable();
        Debug.Log("!!!2DIdle!!!");
        Input.ResetInputAxes();
    }
    public override void EnterState() {
        base.EnterState();
        if (holdingGroup.activeSelf)
            holdingGroup.SetActive(false);
    }
    private void Update() {
        horizontalInput = Input.GetAxis("Horizontal");
        skillSectionInput = Input.GetAxis("SkillSection");
        interactionInput = Input.GetAxis("Interaction");
        explosionInput = Input.GetAxis("Explosion");

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

            Control2D.ChangeState(PlayerState.Falling);

            //float distance = Control2D.PlayerRigid.position.y - PlayerManage.instance.Respawnposition.position.y;
            //if (distance <= 0.1f) {
            //    return;
            //}
            //else {
            //    Debug.Log(distance);
            //    Control2D.ChangeState(PlayerState.Falling);
            //}
        }

        ChangeState();
    }

    private void ChangeState() {
        Control2D.Move(0);
        if (horizontalInput != 0) {
            Control2D.ChangeState(PlayerState.Move);
        }
        else if (skillSectionInput != 0) {
            if (!PlayerManage.instance.IsChangingModeTo3D) {
                Control2D.ChangeState(PlayerState.Skill);
            }
        }
        else if (interactionInput != 0) {
            interactionObj = Control2D.CheckInteractObject();
            if (interactionObj != null) {
                string tagName = interactionObj.transform.Find("Root3D").tag;
                if (tagName == "Climb") {
                    Control2D.ChangeState(PlayerState.Climb);
                }
                else {
                    Debug.LogWarning(tagName);
                }
            }
            else {
                Debug.Log(" 상호작용 오브젝트 null ");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) {
            Control2D.ChangeState(PlayerState.Disable);
        }


        if (PlayerManage.instance.IsBombOnGround) {
            if (explosionInput != 0) {
                PlayerManage.instance.IsBombOnGround = false;
                IBomb bomb = PlayerManage.instance.GetPlantBomb();
                bomb.IBombExplosion();
            }

        }

    }

}
