using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class BiscuitController : MonoBehaviour {

    public Action BiscuiEat;

    private GameObject biscuit;
    private GameObject eatingEffect;

    private void Awake() {
        eatingEffect = transform.parent.GetChild(0).gameObject;
        biscuit = transform.parent.gameObject;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {

            BiscuiEat?.Invoke();

            eatingEffect.SetActive(true);

            // biscuit의 자식 객체들을 돌면서 스크립트 있으면 비활성화
            for (int i = 0; i < biscuit.transform.childCount; i++) {
                Transform child = biscuit.transform.GetChild(i);

                if(child.TryGetComponent(out BiscuitController biscuitController)) {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {

            BiscuiEat?.Invoke();

            eatingEffect.SetActive(true);

            for (int i = 0; i < biscuit.transform.childCount; i++) {
                Transform child = biscuit.transform.GetChild(i);

                if (child.TryGetComponent(out BiscuitController biscuitController)) {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }




}
