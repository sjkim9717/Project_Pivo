using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public GameObject Static;

    public GameObject HoldingGroup;

    public GameObject Dynamic;
    public GameObject Tutorial;
    public GameObject StageClear;

    private void Awake() {
        Static = transform.GetChild(0).gameObject;

        HoldingGroup = Static.transform.GetChild(2).gameObject;

        Dynamic = transform.GetChild(1).gameObject;
        Tutorial = transform.GetChild(2).gameObject;
        StageClear = transform.GetChild(3).gameObject;
    }


    private void Option() {
    }

}
