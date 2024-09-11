using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState3D_Holding : PlayerState3D {

    [SerializeField] private float stopTime = 2f;
    [SerializeField] private float stayTime = 0.5f;

    private GameObject holdingGroup;
    private StaticManager staticManager;
    protected override void OnEnable() {
        base.OnEnable();
    }

    private void Start() {
        staticManager = FindObjectOfType<StaticManager>();
        holdingGroup = staticManager.gameObject.transform.GetChild(2).gameObject;
    }

    public override void EnterState() {
        PlayerManage.instance.isChangingModeTo3D = false;
        Control3D.Ani3D.SetBool("IsFalling", true);
        holdingGroup.SetActive(true);

        stayTime = 0.5f;
        stopTime = 2f;
    }

    private void Update() {
        ChangeState();
    }

    private void ChangeState() {
        Control3D.PlayerRigid.constraints = RigidbodyConstraints.FreezeAll;

        if (stayTime <= 0) {
            stopTime -= Time.deltaTime;

            if (stopTime >= 0) {
                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");
                float skillSectionInput = Input.GetAxis("SkillSection");

                if (horizontalInput != 0 || verticalInput != 0) {

                    Control3D.ChangeState(PlayerState.Falling);
                }
                else if (skillSectionInput != 0) {

                    PlayerManage.instance.CurrentMode = PlayerMode.Player2D;
                    PlayerManage.instance.SwitchMode();
                    Debug.Log("2D 모드로 전환됨");
                }
            }
            else {
                Control3D.ChangeState(PlayerState.Falling);
            }
        }
        else {
            stayTime -= Time.deltaTime;
        }
    }

    public override void ExitState() {
        holdingGroup.SetActive(false);
        Control3D.Ani3D.SetBool("IsFalling", false);
    }
}
