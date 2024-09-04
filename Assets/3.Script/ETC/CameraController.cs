using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CameraController : MonoBehaviour {
    private CinemachineBrain main;

    private Animator camAni;
    private GameObject player;
    private PlayerManager playerManager;

    public void SetCameraSettingGameStart(bool camStart) { main.enabled = camStart; }

    private void Awake() {
        playerManager = FindObjectOfType<PlayerManager>();
        player = FindObjectOfType<Player3DController>().gameObject;
        main = GetComponentInChildren<CinemachineBrain>();

        ConvertCameraEnable();
        camAni = GetComponent<Animator>();

        if (main == null) Debug.LogWarning("CinemachineBrain not found.");
    }

    private void Update() {

        camAni.SetBool("Is3D", playerManager.GetPlayerMode());

        if (Camera.main != null) {
            Camera.main.orthographic = !playerManager.GetPlayerMode();
        }
        else {
            Debug.LogWarning("Main camera not found.");
        }
    }

    private void ConvertCameraEnable() {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.name == "GrassStage_Stage1") main.enabled = false;
        else main.enabled = true;
    }

}

