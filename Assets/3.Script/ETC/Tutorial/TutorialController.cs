using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TutorialController : MonoBehaviour {
    private GameObject tutorialPlayer;

    private GameObject tutorial;
    private GameObject tutorial_Bg;

    private double signalTime;                          // 애니메이션 중지되는 타이밍
    private Tutorial_Camaera tutorial_Camaera;
    private PlayableDirector tutorial_1_Director;

    private Dictionary<AnimationTrack, GameObject> trackBindings = new Dictionary<AnimationTrack, GameObject>();

    private Canvas canvas;
    private Image tutorialImg;
    private Text tutorialText;
    public Sprite[] TutorialSprites;

    private StageClearController[] stageClearController;
    private void Awake() {
        tutorial_Bg = GameObject.Find("IntroBG");
        tutorial = GameObject.Find("IntroTimeLine");

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in players) {
            if (item.transform.parent.gameObject == tutorial) {
                tutorialPlayer = item;
                break;
            }
        }

        tutorial_1_Director = tutorial.GetComponent<PlayableDirector>();
        tutorial_Camaera = FindObjectOfType<Tutorial_Camaera>();

        canvas = transform.GetChild(0).GetComponent<Canvas>();
        tutorialImg = canvas.GetComponentsInChildren<Image>()[1];
        tutorialText = canvas.GetComponentInChildren<Text>();

        if (!GameManager.isLoadTitle) {
            StopTutorial();
        }
        else {  // title을 불러오는 경우 스테이지가 끝나야만 tutorial 종료됨
            stageClearController = FindObjectsOfType<StageClearController>();
            foreach (StageClearController each in stageClearController) {
                each.StageClear += StopTutorialWhenStageEnd;
            }
        }
    }

    private void Start() {
        SaveBindings();
    }
    private void OnEnable() {
        tutorial_1_Director.stopped += OnTimeline_1_Stopped;
    }

    private void OnDestroy() {
        if (tutorial_1_Director != null) {
            tutorial_1_Director.stopped -= OnTimeline_1_Stopped;
        }
    }

    // ============== playable director 연결 등록 해재, 재등록
    private void SaveBindings() {
        TimelineAsset timeline = (TimelineAsset)tutorial_1_Director.playableAsset;
        // 타임라인의 트랙을 순회
        Debug.LogWarning(timeline.name);
        foreach (var track in timeline.GetOutputTracks()) {
            if (track is AnimationTrack animationTrack) {
                // 현재 바인딩 상태 가져오기
                var binding = tutorial_1_Director.GetGenericBinding(animationTrack);

                // 현재 바인딩 상태 저장
                if (binding is Animator animator) {
                    // Animator의 주체 GameObject 확인
                    if (animator.gameObject == tutorialPlayer) {
                        trackBindings[animationTrack] = animator.gameObject;
                        //Debug.Log($"AnimationTrack {track.name}의 바인딩이 {animator.gameObject.name}으로 저장되었습니다.");
                    }
                }
            }
        }
    }

    // 바인딩 해제
    private void UnbindAnimationTracks() {
        foreach (var track in trackBindings.Keys) {
            tutorial_1_Director.SetGenericBinding(track, null);
            //Debug.Log($"AnimationTrack {track.name}의 바인딩 해제!");
        }
    }

    // 바인딩 다시 설정
    private void BindAnimationTracks() {
        foreach (var track in trackBindings.Keys) {
            // 바인딩을 설정하기 전에 기존 바인딩된 GameObject 가져옴
            var boundObject = trackBindings[track];

            if (boundObject != null) {
                // 바인딩 다시 설정
                tutorial_1_Director.SetGenericBinding(track, tutorialPlayer);
            }
        }

        tutorial_1_Director.time = signalTime + 0.1;

        // 애니메이션 트랙의 현재 상태를 업데이트
        tutorial_1_Director.Evaluate();

        // 애니메이션 재생
        tutorial_1_Director.Play();
    }


    // ============== tutorial 활성화 비활성화
    public void StartTutorial() {
        tutorial_Bg.SetActive(true);
        tutorial.SetActive(true);
        tutorial_1_Director.Play();
    }

    public void StopTutorial() {
        tutorialPlayer.SetActive(false);
        tutorial_Bg.SetActive(false);
        tutorial.SetActive(false);
    }

    private void StopTutorialWhenStageEnd() {
        GameManager.instance.IsTutorialCompleted = true;
    }

    // Timeline이 일시정지
    public void PauseTimeLine() {
        tutorial_1_Director.Pause();
        signalTime = tutorial_1_Director.time;
        Debug.Log("Timeline_1 has paursed playing.");

        // Timeline이 끝났을 때 실행할 로직
        FindObjectOfType<Tutorial_Camaera>().SettingCamerasPriority_Tutorial_2();
        AddComponentWhenTimeLineEnd();
        UnbindAnimationTracks();

        FindObjectOfType<Tutorial_PlayerMove>().SetPlayerMove(true);
    }


    // Timeline이 종료될 때 실행되는 메서드
    private void OnTimeline_1_Stopped(PlayableDirector director) {
        Debug.Log("Timeline_1 has finished playing.");

        FindObjectOfType<Tutorial_PlayerMove>().SetPlayerMove(false);
        StopTutorial();

        // Timeline이 끝났을 때 실행할 로직
        FindObjectOfType<PlayerManager>().transform.GetChild(0).gameObject.SetActive(true);
        tutorial_Camaera.SettingCamerasPriority_Game();
        tutorial_Camaera.FindPlayerWhenStartGame();
    }


    //==============================   tutorial trigger setting
    public void CheckTriggerSetting(TutorialTriggerSprite tutorialTrigger, bool spriteshow) {
        if (!GameManager.isLoadTitle) return;

        if (spriteshow) {
            canvas.gameObject.SetActive(true);
            switch (tutorialTrigger) {
                case TutorialTriggerSprite.Move2D:
                    SettingTutorialUI_Move(true);
                    break;
                case TutorialTriggerSprite.Move3D:
                    SettingTutorialUI_Move(false);
                    break;
                case TutorialTriggerSprite.Climb:
                    SettingTutorialUI_Climb();
                    break;
                case TutorialTriggerSprite.ViewChange:
                    SettingTutorialUI_ViewChange();
                    break;
            }
        }
        else {  // 튜토리얼 timeline 재진행
            canvas.gameObject.SetActive(false);

            if (tutorialTrigger == TutorialTriggerSprite.Move2D) {
                DeleteComponentWhenTimeLineEnd();
                BindAnimationTracks();                  // 재 바인딩
            }
        }
    }

    // Timeline 일시정지 시 플레이어 수동 조작 component연결
    private void AddComponentWhenTimeLineEnd() {
        var boxCollider = tutorialPlayer.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;

        var rb2d = tutorialPlayer.AddComponent<Rigidbody2D>();
        rb2d.bodyType = RigidbodyType2D.Kinematic;
    }
    private void DeleteComponentWhenTimeLineEnd() {
        var boxCollider = tutorialPlayer.GetComponent<BoxCollider2D>();
        Destroy(boxCollider);

        var rb2d = tutorialPlayer.GetComponent<Rigidbody2D>();
        Destroy(rb2d);
    }



    //==========UI

    public void SettingTutorialUI_Move(bool isMove2D) {
        tutorialImg.gameObject.SetActive(true);
        if (isMove2D) {
            tutorialImg.sprite = TutorialSprites[0];
        }
        else {
            tutorialImg.sprite = TutorialSprites[1];
        }
        tutorialImg.SetNativeSize();
        tutorialText.text = "MOVE";
    }

    public void SettingTutorialUI_Climb() {
        tutorialImg.gameObject.SetActive(false);
        tutorialText.text = "Climb";
    }
    public void SettingTutorialUI_ViewChange() {
        tutorialImg.gameObject.SetActive(false);
        tutorialText.text = "ViewChange";
    }

}

/*
 tutorial 관리
1. new game에 붙을 예정
 
 
tutorial 1이 끝났을 경우 
1. 카메라 변경
2. 입력받아야함

tutorial 2이 끝났을 경우 
1. 플레이어 찾아야함
2. 카메라 변경

 */
