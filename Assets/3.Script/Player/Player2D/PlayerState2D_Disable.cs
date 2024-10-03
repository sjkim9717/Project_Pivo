using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState2D_Disable : PlayerState2D {
    private StaticManager UI_Static;
    private GameObject PauseGroup;
    public GameObject[] ActiveCheck;

    protected override void OnEnable() {
        UI_Static = FindObjectOfType<StaticManager>();

        PauseGroup = UI_Static.transform.GetChild(4).gameObject;

        ActiveCheck = new GameObject[3];
        for (int i = 5; i < UI_Static.transform.childCount; i++) {
            ActiveCheck[i - 5] = UI_Static.transform.GetChild(i).gameObject;
        }
    }

    private void Update() {
        if (!PauseGroup.activeSelf) {
            foreach (GameObject item in ActiveCheck) {
                if (item.activeSelf) {
                    return;
                }
            }
            Control2D.ChangeState(PlayerState.Idle);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Control2D.ChangeState(PlayerState.Idle);
        }
    }
}
