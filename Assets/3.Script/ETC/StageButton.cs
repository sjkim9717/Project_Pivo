using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageButton : MonoBehaviour
{
    public StageLevel ButtonStageLevel;
    private StageSelectController stageSelect;

    private void Awake() {
        stageSelect = FindObjectOfType<StageSelectController>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Player")) {
            stageSelect.SelectStageLevel_Stage(ButtonStageLevel);
            stageSelect.SelectStageLevel(ButtonStageLevel);
        }
    }
}
