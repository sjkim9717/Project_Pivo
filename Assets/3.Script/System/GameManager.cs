using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance { get; private set; }
    [Space(5)]
    [Header("Scene 1 Tutorial Check")]
    public static bool isLoadTitle = true;
    public bool IsTutorialCompleted;                        // stage clear 후 UI 꺼져야할 경우
    public bool IsIntroCompleted;                           // 애니메이션만 꺼지면 되는 경우
    public StageLevel currentStage;                         // 현재 씬
    public StageLevel PreviousGameStage;                    // select scene에서 확인할 이전 게임 씬 이름
    private GameObject staticGroup;
    private GameObject loadingGroup;
    private GameObject fadeGroup;
    //private GameObject UI_Title;

    private PlayerManage playerManage;
    private PlayableDirector StageClear_Director;
    private ConvertMode[] convertMode;

    private StageClearControl[] stageClearController;



    public readonly Dictionary<string, StageLevel> stageMap = new Dictionary<string, StageLevel>() {
        { "GrassStage_Stage1", StageLevel.GrassStageLevel_1 },
        { "GrassStage_Stage5", StageLevel.GrassStageLevel_5 },
        { "GrassStage_Stage7", StageLevel.GrassStageLevel_7 },
        { "SnowStage_Stage3", StageLevel.SnowStageLevel_3 },
        { "SnowStage_Stage4", StageLevel.SnowStageLevel_4 },
        { "SnowStage_Stage6", StageLevel.SnowStageLevel_6 },
        { "SnowStage_Stage7", StageLevel.SnowStageLevel_7 },
        { "StageSelect_Grass", StageLevel.StageSelect }
    };


    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

        staticGroup = transform.GetChild(0).gameObject;
        fadeGroup = transform.GetChild(4).gameObject;
        loadingGroup = transform.GetChild(5).gameObject;
        //UI_Title = FindObjectOfType<MainTitleManager>().gameObject;
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += FindScenLevelWhenLevelChange;
        SceneManager.sceneLoaded += FindObjectsWhenLevelChange;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= FindScenLevelWhenLevelChange;
        SceneManager.sceneLoaded -= FindObjectsWhenLevelChange;
    }

    // 씬이 변경될때마다 씬 레벨 확인
    private void FindScenLevelWhenLevelChange(Scene scene, LoadSceneMode mode) {
        string sceneName = SceneManager.GetActiveScene().name;

        currentStage = SelectSceneLevelWithSceneName(sceneName);
        Debug.LogWarning($"Game Manager Scene loaded SceneName : {sceneName}");

        if (currentStage != StageLevel.StageSelect) {
            PreviousGameStage = currentStage;
        }
    }

    // 씬이 변경될때마다 스테이지 클리어 조건 확인
    private void FindObjectsWhenLevelChange(Scene scene, LoadSceneMode mode) {
        //Debug.LogWarning(" current scene | FindObjectsWhenLevelChange | " + currentStage);

        if (currentStage != StageLevel.StageSelect) {

            playerManage = FindObjectOfType<PlayerManage>();

            // Stage Clear에 필요한 convertMode 전체 할당 => 2D에서 Stage Clear되면 전체 Layer ActiveTrue돌려야함

            convertMode = new ConvertMode[FindObjectsOfType<ConvertMode>().Length];
            convertMode = FindObjectsOfType<ConvertMode>();

            stageClearController = FindObjectsOfType<StageClearControl>();
            // StageClear 이벤트 구독
            foreach (var controller in stageClearController) {
                if (controller != null) {
                    // 이벤트 핸들러가 이미 등록되어 있는지 확인하고, 중복 등록을 방지합니다.
                    if (!controller.StageClear.GetInvocationList().Contains((Action)OnStageClear)) {
                        controller.StageClear += OnStageClear;
                    }
                }
            }

            // StageClear시 재생될 Timeline 불러오기
            PlayableDirector[] playableDirectors = FindObjectsOfType<PlayableDirector>();
            foreach (PlayableDirector item in playableDirectors) {
                if (item.name.Contains("StageClear_TimeLine")) {
                    StageClear_Director = item.GetComponent<PlayableDirector>();
                    StageClear_Director.stopped += StageClear_Director_Finished;
                    break;
                }
            }

        }

    }

    private void StageClear_Director_Finished(PlayableDirector obj) {
        transform.GetChild(3).gameObject.SetActive(true);
    }

    // StageClear 이벤트가 호출될 때 실행될 메서드
    private void OnStageClear() {

        // 오디오 SFX
        string[] include = { "StageClear" };
        string playSFX = AudioManager.instance.GetDictionaryKey<string, List<AudioClip>>(AudioManager.SFX, include);

        if (playSFX != null) { // playBgm이 null이 아닐 경우에만 재생
            AudioManager.instance.BGM_Play(AudioManager.instance.InGameAudio, playSFX);
        }
        else {
            Debug.LogWarning($"No matching SFX found for {include}.");
        }

        playerManage.CurrentMode = PlayerMode.AutoMode;

        foreach (ConvertMode item in convertMode) {
            item.ChangeLayerAllActiveTrue();
        }

        FindObjectOfType<CameraManager>().SettingCamerasPriority_StageClear();          // 카메라 세팅을 먼저 하고 플레이어를 꺼야 위치를 잡을 수 있음

        playerManage.ChangeStageClear();
        StageClear_Director.Play();

        // 만약 세이브가 있다면 현재 점수랑 비교해서 높은 쪽 저장
        if (Save.instance.TryGetStageScore(currentStage, out int savescore)) {
            int currentscore = GetComponentInChildren<StaticManager>().GetBiscuitCount();

            int maxScore = Math.Max(savescore, currentscore);
            Save.instance.SetStageData(currentStage, true, maxScore);
        }

        Debug.LogWarning("save Data | " + Save.instance.GameData);
        Save.instance.SaveGame();
    }

    // scene이 로드될경우 해당씬 확인
    public StageLevel SelectSceneLevelWithSceneName(string sceneName) {
        if (stageMap.TryGetValue(sceneName, out StageLevel stage)) return stage;
        else return StageLevel.SnowStageLevel_4;
    }

    public string GetSceneNameFromStageLevel(StageLevel stageLevel) {
        foreach (var item in stageMap)
            if (item.Value == stageLevel) return item.Key;

        return null;
    }

    public void LoadSelectStage(StageLevel selectStageLevel) {
        string sceneName = GetSceneNameFromStageLevel(selectStageLevel);

        currentStage = selectStageLevel;
        //Debug.LogWarning("GameManager current scene | LoadSelect Stage | " + currentStage);

        //SceneManager.LoadScene(sceneName);
        StartCoroutine(LoadGameSceneAsync(sceneName));
    }
    IEnumerator LoadGameSceneAsync(string sceneName) {
        Image fade = fadeGroup.GetComponentInChildren<Image>(true);
        Image loading = loadingGroup.GetComponentInChildren<Image>();

        // 비동기적으로 GameScene 로드 시작
        staticGroup.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = true;  // 씬 활성화 허용

        fade.gameObject.SetActive(true);
        Color fadeAlpha = fade.color;
        Color loadingAlpha = loading.color;
        loadingGroup.SetActive(true);

        // 로드가 완료될 때까지 대기
        while (!asyncLoad.isDone) {
            fadeAlpha.a += Time.deltaTime * 0.6f; // Adjust speed if needed
            fadeAlpha.a = Mathf.Clamp01(0.2f + fadeAlpha.a); // Clamp between 0 and 1
            fade.color = fadeAlpha;

            loadingAlpha.a = Mathf.PingPong(Time.time * 5, 0.2f) + 0.7f; // Pulses between 0.8 and 1.0
            loading.color = loadingAlpha;

            yield return null;
        }

        loadingGroup.SetActive(false);
        fade.gameObject.SetActive(false);
        fadeAlpha.a = 0;
        fade.color = fadeAlpha;

        // 로드된 씬을 활성화
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        staticGroup.SetActive(!currentStage.Equals(StageLevel.StageSelect));
    }
}

/*  목적 : 씬 변경되는 시점의 stagelevel data 들고옴 + clear 시 정보저장
 1. 씬 로드시에 해당 레벨 들고있을 것
    - if : 레벨 선택 씬이라면 => 버튼에 씬 레벨 각각 넣을 것
 2. 스테이지 클리어시 save의 Data호출
    - 호출된 Data에 해당 레벨 클리어 bool값 + count 넣을 것 
 
 */