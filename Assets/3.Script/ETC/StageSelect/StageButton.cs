using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageButton : MonoBehaviour {

    public StageLevel ButtonStageLevel;

    bool isClear = false;
    private Color originColor;
    private Color blockColor = Color.red;

    private StageSelectController stageSelect;

    private void Awake() {
        originColor = GetComponent<MeshRenderer>().material.color;
        Debug.LogWarning("origin color | " + originColor);
        stageSelect = FindObjectOfType<StageSelectController>();
    }

    private void Start() {
        if (Save.instance.TryGetStageClear(ButtonStageLevel, out isClear)) {
            if (!isClear) {
                GetComponent<MeshRenderer>().material.color = blockColor;
            }
            else {
                GetComponent<MeshRenderer>().material.color = originColor;
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
