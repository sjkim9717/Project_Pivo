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

    private Tutorial_Camaera tutorial_Camaera;
    private PlayableDirector tutorial_1_Director;

    private Dictionary<AnimationTrack, GameObject> trackBindings = new Dictionary<AnimationTrack, GameObject>();


    private Canvas canvas;
    private Image tutorialImg;
    private Text tutorialText;
    public Sprite[] TutorialSprites;

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
    }

    private void Start() {
        SaveBindings();
    }

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
                        Debug.Log($"AnimationTrack {track.name}의 바인딩이 {animator.gameObject.name}으로 저장되었습니다.");
                    }
                }
            }
        }
    }

    // 바인딩 해제
    private void UnbindAnimationTracks() {
        foreach (var track in trackBindings.Keys) {
            tutorial_1_Director.SetGenericBinding(track, null);
            Debug.Log($"AnimationTrack {track.name}의 바인딩 해제!");
        }
    }

    // 바인딩 다시 설정
    private void BindAnimationTracks() {
        foreach (var track in trackBindings.Keys) {
            // 바인딩을 설정하기 전에 기존 바인딩된 GameObject의 위치를 가져옴
            var boundObject = trackBindings[track];

            if (boundObject != null) {
                // tutorialPlayer의 위치를 바인딩된 GameObject의 위치로 설정
                boundObject.transform.position = tutorialPlayer.transform.position;

                // 바인딩 다시 설정
                tutorial_1_Director.SetGenericBinding(track, tutorialPlayer);
                Debug.Log($"AnimationTrack {track.name}에 {tutorialPlayer.name} 바인딩하고 위치를 {tutorialPlayer.transform.position}으로 설정!");
            }
        }
    }

    private void OnEnable() {
        tutorial_1_Director.stopped += OnTimeline_1_Stopped;

    }

    private void OnDestroy() {
        // 메모리 누수를 방지하기 위해 이벤트 등록을 해제
        if (tutorial_1_Director != null) {
            tutorial_1_Director.stopped -= OnTimeline_1_Stopped;
        }
    }


    public void StartTutorial() {
        tutorial_Bg.SetActive(true);
        tutorial.SetActive(true);
        tutorial_1_Director.Play();
    }
    public void PauseTimeLine() {
        tutorial_1_Director.Pause();
        Debug.Log("Timeline_1 has paursed playing.");

        // Timeline이 끝났을 때 실행할 로직
        FindObjectOfType<Tutorial_Camaera>().SettingCamerasPriority_Tutorial_2();

        AddComponentWhenTimeLineEnd();

        UnbindAnimationTracks();            // 바인딩 해제 

        FindObjectOfType<Tutorial_PlayerMove>().SetPlayerMove(true);
    }

    //TODO: Timeline이 종료될 때 실행되는 메서드 : tutorial 끝, animation2번 연결
    private void OnTimeline_1_Stopped(PlayableDirector director) {
        Debug.Log("Timeline_1 has finished playing.");

        FindObjectOfType<Tutorial_PlayerMove>().SetPlayerMove(false);

        tutorialPlayer.SetActive(false);
        tutorial_Bg.SetActive(false);
        tutorial.SetActive(false);

        // Timeline이 끝났을 때 실행할 로직
        tutorial_Camaera.SettingCamerasPriority_Game();
        tutorial_Camaera.FindPlayerWhenStartGame();
        Save.instance.CompleteTutorial();

    }


    private void AddComponentWhenTimeLineEnd() {
        // BoxCollider2D 추가 후 isTrigger 설정
        var boxCollider = tutorialPlayer.AddComponent<BoxCollider2D>();
        boxCollider.isTrigger = true;

        // Rigidbody2D 추가 후 BodyType을 Kinematic으로 설정
        var rb2d = tutorialPlayer.AddComponent<Rigidbody2D>();
        rb2d.bodyType = RigidbodyType2D.Kinematic;
    }

    //==============================
    public void CheckTriggerSetting(TutorialTriggerSprite tutorialTrigger, bool spriteshow) {
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
                BindAnimationTracks();                  // 재 바인딩
                tutorial_1_Director.Play();
            }
        }
    }

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
