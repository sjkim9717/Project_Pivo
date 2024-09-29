using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StageClearControl : MonoBehaviour {

    public Action StageClear = delegate { };

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Debug.Log("StageClearController | " + StageClear);
            StageClear?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            StageClear?.Invoke();
        }
    }

    //TODO: stage clear test 지울것
    public void StageClearTest() {
        StageClear?.Invoke();
    }
}

/*
 1. 클리어 발판에 플레이어가 올라왔을 경우 이벤트 발생
 -> 필요한 스크립트에 들고가서 사용
 */

