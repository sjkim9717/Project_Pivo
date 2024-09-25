using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour {
    private Animator camAni;
    private PlayerManage playerManager;

    private const int PRIORITY_ON = 20; // 활성화된 카메라의 우선순위
    private const int PRIORITY_OFF = 0; // 비활성화된 카메라의 우선순위

    private CinemachineBrain main;
    private CinemachineVirtualCamera[] cameras;
    private CinemachineStateDrivenCamera gameCam;
    private CinemachineVirtualCamera[] playerMode;
    private bool isCentered = false; // 플레이어를 한 번만 중앙에 고정하기 위한 플래그

    private enum CameraType { CanvasCamera, IntroCam1, IntroCam2, StageClearCam, GameCam }

    private void Awake() {
        camAni = GetComponent<Animator>();
        playerManager = FindObjectOfType<PlayerManage>();
        main = GetComponentInChildren<CinemachineBrain>();
        gameCam = GetComponentInChildren<CinemachineStateDrivenCamera>();

        // Initialize all cameras including gameCam
        cameras = new CinemachineVirtualCamera[4];
        InitSetting();

        StaticManager.Restart += SettingCamerasPriority_Game;
    }

    private void Start() {
        SettingCameraView();
        FindGameCameraPlayer();
    }

    private void Update() {

        if (FindGameModeCamera()) {

            if (Camera.main != null) {
                camAni.SetBool("Is3D", playerManager.CurrentMode == PlayerMode.Player3D);
                Camera.main.orthographic = playerManager.CurrentMode == PlayerMode.Player2D;
                if (playerManager.CurrentMode == PlayerMode.Player2D) {
                    if (!isCentered) {
                        isCentered = true;
                        Debug.Log(" 2D camera setting");

                        SettingGameCameraPlayer();
                    }
                }
                else {
                    if (isCentered) isCentered = false;
                }
            }
            else {
                Debug.LogWarning("Main camera not found.");
            }
        }
    }
    private void InitSetting() {
        if (GameManager.instance.currentStage == StageLevel.GrassStageLevel_1) {
            if (GameManager.isLoadTitle) {
                cameras[(int)CameraType.IntroCam1] = GameObject.Find("Intro1").GetComponent<CinemachineVirtualCamera>();
                cameras[(int)CameraType.IntroCam2] = GameObject.Find("Intro2").GetComponent<CinemachineVirtualCamera>();
                cameras[(int)CameraType.CanvasCamera] = GameObject.Find("CanvasCamera").GetComponent<CinemachineVirtualCamera>();
                cameras[(int)CameraType.StageClearCam] = GameObject.Find("StageClearCamera").GetComponent<CinemachineVirtualCamera>();
                DefaultCameraSetting();
            }
            else {
                SettingCamerasPriority_Game();
            }
        }
        else {
            cameras[(int)CameraType.CanvasCamera] = GameObject.Find("CanvasCamera").GetComponent<CinemachineVirtualCamera>();
            cameras[(int)CameraType.StageClearCam] = GameObject.Find("StageClearCamera").GetComponent<CinemachineVirtualCamera>();
            SettingCamerasPriority_Game();
        }
    }

    private void FindGameCameraPlayer() {
        playerMode = gameCam.GetComponentsInChildren<CinemachineVirtualCamera>();
        if (playerMode != null) {
            playerMode[0].Follow = PlayerManage.instance.Player3D.transform;
            playerMode[0].LookAt = PlayerManage.instance.Player3D.transform;

            playerMode[1].Follow = PlayerManage.instance.Player2D.transform;
            playerMode[1].LookAt = PlayerManage.instance.Player2D.transform;
        }
        else {
            Debug.LogWarning("CinemachineVirtualCamera is null");
        }
    }
    private void SettingCameraView() {
        playerMode = gameCam.GetComponentsInChildren<CinemachineVirtualCamera>();
        CinemachineTransposer transposer = playerMode[0].GetCinemachineComponent<CinemachineTransposer>();

        if (transposer != null) {
            // StageLevel에 따른 Follow Offset의 x 값을 설정
            float offsetX = (GameManager.instance.currentStage == StageLevel.GrassStageLevel_7) ? -35f : 70f;

            Vector3 newOffset = transposer.m_FollowOffset;
            newOffset.x = offsetX;
            transposer.m_FollowOffset = newOffset;
        }

    }

    private void SettingGameCameraPlayer() {

        // Dead Zone 비활성화 (카메라가 플레이어를 정확히 중앙에 위치시키도록)
        playerMode[1].Follow = PlayerManage.instance.Player2D.transform;
        playerMode[1].LookAt = PlayerManage.instance.Player2D.transform;

        CinemachineFramingTransposer composer = playerMode[1].GetCinemachineComponent<CinemachineFramingTransposer>();
        composer.m_DeadZoneWidth = 0f;
        composer.m_DeadZoneHeight = 0f;

        StartCoroutine(DeleteCameraFollow());

    }

    private IEnumerator DeleteCameraFollow() {
        yield return new WaitForSeconds(1f);
        playerMode[1].Follow = null;
        playerMode[1].LookAt = null;
    }


    // 모든 카메라의 우선순위를 OFF로 설정하는 메서드
    private void TurnOffAllCameras() {
        foreach (var cam in cameras) {
            if (cam == null) continue;
            cam.Priority = PRIORITY_OFF;
        }
        gameCam.Priority = PRIORITY_OFF;
    }

    // 원하는 카메라의 우선순위를 ON으로 설정하는 메서드
    private void SetCameraPriority(CameraType cameraType) {
        TurnOffAllCameras();  // 먼저 모든 카메라를 OFF로 설정

        if (cameraType == CameraType.GameCam) {
            gameCam.Priority = PRIORITY_ON;
        }
        else {
            cameras[(int)cameraType].Priority = PRIORITY_ON;
        }
    }

    // 기본 카메라 세팅
    public void DefaultCameraSetting() {
        SetCameraPriority(CameraType.CanvasCamera);  // CanvasCamera를 기본 카메라로 설정
    }

    // Tutorial 1번 카메라 설정
    public void SettingCamerasPriority_Tutorial_1() {
        SetCameraPriority(CameraType.IntroCam1);  // IntroCam1을 활성화
    }

    // Tutorial 2번 카메라 설정
    public void SettingCamerasPriority_Tutorial_2() {
        SetCameraPriority(CameraType.IntroCam2);  // IntroCam2를 활성화
    }

    // 게임 카메라 설정
    public void SettingCamerasPriority_Game() {
        SetCameraPriority(CameraType.GameCam);  // 게임 카메라 활성화
    }

    // StageClear 카메라 설정
    public void SettingCamerasPriority_StageClear() {
        Debug.LogWarning(" 뒤졌으면 불러져야함");
        SetCameraPriority(CameraType.StageClearCam);  // 게임 카메라 활성화
    }


    // tutorial 끝나고 시작해야함
    public void FindPlayerWhenStartGame() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in players) {
            if (item.layer == LayerMask.NameToLayer("2DPlayer")) {
                item.SetActive(true);
            }
        }
    }

    private bool FindGameModeCamera() {
        if (gameCam.Priority == PRIORITY_ON) return true;
        else return false;
    }



}
