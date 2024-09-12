using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    private int password;
    private bool isTouched;
    public void SetPassword(int _password) { password = _password; }
    private GateManage gateManage;
    private void Awake() {
        gateManage = FindObjectOfType<GateManage>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (!isTouched) {
                isTouched = true;  
                gateManage.FindDoor(password);
                StartCoroutine(DeleteTimeDelay());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            if (!isTouched) {
                isTouched = true;
                gateManage.FindDoor(password);
                StartCoroutine(DeleteTimeDelay());
            }
        }
    }

    private IEnumerator DeleteTimeDelay() {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}

/*
 1. 키가 플레이어와 닿았다면
 2. 매니저에 신호를 주기
 
 */