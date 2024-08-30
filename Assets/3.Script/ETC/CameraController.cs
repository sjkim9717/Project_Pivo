using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] private PlayerManager playerManager;
    private Animator camAni;

    private bool isGameStart;
    public void SetGameStart(bool gamestart) { isGameStart = gamestart; }

    private void Awake() {
        if (playerManager == null) playerManager = FindObjectOfType<PlayerManager>();
        camAni = GetComponent<Animator>();
        isGameStart = false;
    }

    private void Update() {
        if (isGameStart) {
            camAni.SetBool("Is3D", playerManager.GetPlayerMode());
        }
    }

    //TODO: test용 삭제할 것
    public void OnclickTest() {
        isGameStart = true;
    }

}
