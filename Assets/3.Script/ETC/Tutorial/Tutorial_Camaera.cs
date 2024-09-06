using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Tutorial_Camaera : MonoBehaviour {

    private const int PRIORITY_ON = 20; // 활성화된 카메라의 우선순위
    private const int PRIORITY_OFF = 0; // 비활성화된 카메라의 우선순위

    private CinemachineBrain main;

    private CinemachineVirtualCamera[] cameras;
    private CinemachineStateDrivenCamera gameCam;
    private enum CameraType { CanvasCamera, IntroCam1, IntroCam2, GameCam }

    private void Awake() {
        main = GetComponentInChildren<CinemachineBrain>();

        // Initialize all cameras including gameCam
        cameras = new CinemachineVirtualCamera[3];
        cameras[(int)CameraType.IntroCam1] = GameObject.Find("Intro1").GetComponent<CinemachineVirtualCamera>();
        cameras[(int)CameraType.IntroCam2] = GameObject.Find("Intro2").GetComponent<CinemachineVirtualCamera>();
        cameras[(int)CameraType.CanvasCamera] = GameObject.Find("CanvasCamera").GetComponent<CinemachineVirtualCamera>();
        gameCam = GetComponentInChildren<CinemachineStateDrivenCamera>();

        DefaultCameraSetting();
    }

    // 모든 카메라의 우선순위를 OFF로 설정하는 메서드
    private void TurnOffAllCameras() {
        foreach (var cam in cameras) {
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


    // tutorial 끝나고 시작해야함
    public void FindPlayerWhenStartGame() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in players) {
            if (item.layer == LayerMask.NameToLayer("2DPlayer")) {
                item.SetActive(true);
            }
        }
    }


}
