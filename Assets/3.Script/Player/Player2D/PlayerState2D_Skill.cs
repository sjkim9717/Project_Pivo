using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState2D_Skill : PlayerState2D {

    private bool isSkillButtonPressed = false;
    private ConvertMode[] convertModes;
    private void Start() {
       convertModes = FindObjectsOfType<ConvertMode>();
    }
    private void Update() {
        skillSectionInput = Input.GetAxis("SkillSection");

        ChangeState();
    }

    private void ChangeState() {
        if (skillSectionInput != 0 && !isSkillButtonPressed) {                       // 2D 모드에서 스킬 버튼 입력 감지
            isSkillButtonPressed = true;                                             // 버튼이 눌린 상태로 표시
            Debug.Log("3D 모드로 전환됨");

            foreach (ConvertMode mode in convertModes) {
                mode.ChangeLayerAllActiveTrue();
            }

            playerManage.CurrentMode = PlayerMode.Player3D;
            playerManage.IsChangingModeTo3D = true;
            playerManage.SwitchMode();
        }

        if (skillSectionInput == 0 && isSkillButtonPressed) {                           // 스킬 버튼이 해제되었는지 감지
            isSkillButtonPressed = false;                                               // 버튼 눌림 상태를 초기화
        }

    }

}
