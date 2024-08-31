using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour {
    public static InGameManager instance { get; private set; }

    public GameObject Static;
    public GameObject Dynamic;
    public GameObject Tutorial;
    public GameObject StageClear;

    private void Awake() {
        Static = transform.GetChild(0).gameObject;
        Dynamic = transform.GetChild(1).gameObject;
        Tutorial = transform.GetChild(2).gameObject;
        StageClear = transform.GetChild(3).gameObject;

        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }


    private void Option() {
    }

}
