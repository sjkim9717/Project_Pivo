using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
   private PlayerManager playerManager;
    private Animator camAni;

    private bool isGameStart;
    public void SetGameStart(bool gamestart) { isGameStart = gamestart; }

    private void Awake() {
        playerManager = FindObjectOfType<PlayerManager>();
        camAni = GetComponent<Animator>();
        isGameStart = false;
    }

    private void Update() {
        if (isGameStart) {
            camAni.SetBool("Is3D", playerManager.GetPlayerMode());
        }

        Camera.main.orthographic = !playerManager.GetPlayerMode();

    }

    //TODO: test용 삭제할 것
    public void OnclickTest() {
        isGameStart = true;
    }

}
