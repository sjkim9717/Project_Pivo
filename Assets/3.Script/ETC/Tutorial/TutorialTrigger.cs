using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour {

    public bool TutorialShow;
    public TutorialTriggerSprite TutorialTriggerSet;

    private TutorialController tutorialController;

    private void Awake() {
        tutorialController = FindObjectOfType<TutorialController>();
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Debug.Log("collider enter | " + gameObject.name + " | trigger bool | " + TutorialShow);
            tutorialController.CheckTriggerSetting(TutorialTriggerSet, TutorialShow);
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            tutorialController.CheckTriggerSetting(TutorialTriggerSet, TutorialShow);           
        }
    }
}

/*
 트리거에 붙어서 
 */