using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageButton : MonoBehaviour
{
    public StageLevel ButtonStageLevel;
    private StageSelectManager stageSelect;

    private void Awake() {
        stageSelect = FindObjectOfType<StageSelectManager>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Player")) {
            stageSelect.SelectStageLevel(ButtonStageLevel);
        }
    }
}
