using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageButton : MonoBehaviour {

    public StageLevel ButtonStageLevel;

    private bool isClear = false;
    private Color originColor;
    private Color blockColor = Color.red;

    private Material material;
    private UI_StageSelect stageSelect;

    private void Awake() {
        material = GetComponent<MeshRenderer>().material;
        originColor = material.color;
        stageSelect = FindObjectOfType<UI_StageSelect>();
    }

    private void Start() {
        if (Save.instance.TryGetStageClear(ButtonStageLevel, out isClear)) {
            if (!isClear) {
                material.SetFloat("_IsStageClear", 0);
                // 이전 레벨이 클리어되었다면 originColor
                bool isPreviousStageClear = false;
                if (ButtonStageLevel != StageLevel.GrassStageLevel_1) {
                    if (Save.instance.TryGetStageClear(ButtonStageLevel - 1, out isPreviousStageClear)) {
                        material.color = isPreviousStageClear ? originColor : blockColor;
                    }
                }
                else {
                    material.color = originColor;
                }

            }
            else {
                material.color = originColor;
                material.SetFloat("_IsStageClear", 1);
            }

        }

    }



    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Player")) {
            stageSelect.SelectStageLevel_Stage(ButtonStageLevel);
            stageSelect.SelectStageLevel(ButtonStageLevel);
        }
    }


}
