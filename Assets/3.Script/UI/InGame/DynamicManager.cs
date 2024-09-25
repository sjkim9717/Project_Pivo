using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DynamicManager : MonoBehaviour {

    private PlayerManage playerManager;

    public GameObject BombGroup;
    public GameObject DeadGroup;

    private void Awake() {
        BombGroup = transform.GetChild(1).gameObject;
        DeadGroup = transform.GetChild(2).gameObject;
    }

    /*
     
    private void OnEnable() {
        SceneManager.sceneLoaded += FindObjectsWhenLevelChange;
    }
    private void OnDisable() {
        SceneManager.sceneLoaded -= FindObjectsWhenLevelChange;
    }

    // stage 클리어시 변경되는 오브젝트 다시 받아와야함(초기화 다시해야함)
    private void FindObjectsWhenLevelChange(Scene scene, LoadSceneMode mode) {
        playerManager = FindObjectOfType<PlayerManager>();

        //  플레이어 사망이벤트 구독
        PlayerManager.PlayerDead += ActiveWhenPlayerDie;
    }


    private void ActiveWhenPlayerDie() {
        DeadGroup.SetActive(true);
        DeadGroup.GetComponent<Animator>().enabled = true;
    }

     
     */
}
