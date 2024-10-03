using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState3D_Holding : PlayerState3D {

    [SerializeField] private float stopTime = 2f;
    [SerializeField] private float stayTime = 0.5f;

    private GameObject holdingGroup;

    private ConvertMode[] convertMode;

    protected override void OnEnable() {
        base.OnEnable();
    }

    protected override void Awake() {
        base.Awake();
        holdingGroup = FindObjectOfType<GameManager>().transform.GetChild(0).GetChild(2).gameObject;

        convertMode = new ConvertMode[FindObjectsOfType<ConvertMode>().Length];

        convertMode[0] = FindObjectOfType<ConvertMode_Tile>();
        convertMode[1] = FindObjectOfType<ConvertMode_Object>();
        convertMode[2] = FindObjectOfType<ConvertMode_Destroy>();
        convertMode[3] = FindObjectOfType<ConvertMode_Item>();
        convertMode[4] = FindObjectOfType<ConvertMode_MoveTile>();
    }

    public override void EnterState() {
        playerManage.IsChangingModeTo3D = false;
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
                    playerManage.IsChangingModeTo3D = true;
                                        
                    playerManage.CurrentMode = PlayerMode.Player2D;
                    foreach (ConvertMode item in convertMode) {                     // 2D로 변경되면 잘려있는 친구들은 남아있어야함
                        item.ChangeLayerActiveTrueWhen3DModeCancle();
                    }
                    playerManage.Change2D();
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
