using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TutorialController : MonoBehaviour
{

    private GameObject tutorial;
    private GameObject tutorial_2;
    private GameObject tutorial_Bg;



    PlayableDirector tutorial_1_Director;
    PlayableDirector tutorial_2_Director;


    private void Awake() {

        tutorial_Bg = GameObject.Find("IntroBG");
        tutorial = GameObject.Find("IntroTimeLine");
        tutorial_2 = GameObject.Find("IntroTimeLine_2");

        tutorial_1_Director = tutorial.GetComponent<PlayableDirector>();
        tutorial_2_Director = tutorial_2.GetComponent<PlayableDirector>();
    }
    private void OnEnable() {
        tutorial_1_Director.stopped += OnTimeline_1_Stopped;
        tutorial_2_Director.stopped += OnTimeline_2_Stopped;
    }

    private void OnDestroy() {
        // 메모리 누수를 방지하기 위해 이벤트 등록을 해제
        if (tutorial_1_Director != null) {
            tutorial_1_Director.stopped -= OnTimeline_1_Stopped;
            tutorial_2_Director.stopped -= OnTimeline_2_Stopped;
        }
    }
    private void OnDisable() {
        tutorial_Bg.SetActive(false);
        tutorial_2.SetActive(false);
        tutorial.SetActive(false);
    }

    public void StartTutorial() {
        tutorial_Bg.SetActive(true);
        tutorial.SetActive(true);
        tutorial_1_Director.Play();
    }
    //TODO: Timeline이 종료될 때 실행되는 메서드 : tutorial 끝, animation2번 연결
    private void OnTimeline_1_Stopped(PlayableDirector director) {
        Debug.Log("Timeline_1 has finished playing.");
        // Timeline이 끝났을 때 실행할 로직
        tutorial_2.SetActive(true);

    }
    private void OnTimeline_2_Stopped(PlayableDirector director) {
        Debug.Log("Timeline_2 has finished playing.");
        // Timeline이 끝났을 때 실행할 로직
        FindPlayerWhenStartGame();
        Save.instance.CompleteTutorial();
    }

    // tutorial 끝나고 시작해야함
    public void FindPlayerWhenStartGame() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in players) {
            if (item.layer == LayerMask.NameToLayer("2DPlayer")) {
                item.SetActive(true);
            }
        }

        // 1번씬 카메라 플레이어 찾아야함
        FindObjectOfType<CameraController>().SetCameraSettingGameStart(true);
    }
}

/*
 tutorial 관리
1. new game에 붙을 예정
 
 
 */
