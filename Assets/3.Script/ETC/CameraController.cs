using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour {
    private CinemachineBrain main;

    private Animator camAni;
    private PlayerManager playerManager;

    private bool isGameStart;
    public void SetGameStart(bool gamestart) { isGameStart = gamestart; }

    private void Awake() {
        playerManager = FindObjectOfType<PlayerManager>();
        main = GetComponentInChildren<CinemachineBrain>();
        camAni = GetComponent<Animator>();
        isGameStart = false;

        if (main == null) Debug.LogWarning("CinemachineBrain not found.");
    }

    private void Update() {
        if (isGameStart) {
            if (main != null) {
                main.enabled = true;
            }
            else {
                Debug.LogWarning("CinemachineBrain is null.");
            }
            camAni.SetBool("Is3D", playerManager.GetPlayerMode());
        }

        if (Camera.main != null) {
            Camera.main.orthographic = !playerManager.GetPlayerMode();
        }
        else {
            Debug.LogWarning("Main camera not found.");
        }
    }

    //TODO: test용 삭제할 것
    public void OnclickTest() {
        isGameStart = true;
    }

}
