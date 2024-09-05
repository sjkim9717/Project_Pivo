using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour {
    private GameObject triggerParent;
    private GameObject[] triggers;

    private void Awake() {
        triggerParent = GameObject.FindGameObjectWithTag("Triggers");

        if (triggerParent != null) {
            triggers = new GameObject[triggerParent.transform.childCount - 1];
            for (int i = 1; i < triggerParent.transform.childCount; i++) {
                triggers[i - 1] = triggerParent.transform.GetChild(i).gameObject;
            }
            Debug.Log(triggers.Length);
        }
    }


}
