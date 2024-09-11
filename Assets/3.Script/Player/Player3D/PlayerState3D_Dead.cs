using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState3D_Dead : PlayerState3D {
    private DynamicManager UI_Dynamic;
    private GameObject deadGroup;
    protected override void Awake() {
        base.Awake();
        UI_Dynamic = FindObjectOfType<DynamicManager>();
        deadGroup = UI_Dynamic.transform.GetChild(2).gameObject;
    }

    public override void EnterState() {

        PlayerManage.instance.Respawn();

        if (Control3D != null && Control3D.Ani3D != null) {
            Control3D.Ani3D.SetTrigger("IsDie");
        }
        else {
            if (Control3D == null) {
                Debug.LogError("Control3D is null.");
            }
            else if(Control3D.Ani3D == null) {
                Debug.LogError("Control3D.Ani3D is null.");
            }

        }

        deadGroup.SetActive(true);
        deadGroup.GetComponent<Animator>().enabled = true;
    }


}
