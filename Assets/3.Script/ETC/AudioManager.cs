using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour {
    public static AudioManager instance { get; private set; }

    // BGM을 위한 Dictionary
    public static readonly Dictionary<string, AudioClip> BGM = new Dictionary<string, AudioClip>();

    // SFX를 위한 Dictionary
    public static readonly Dictionary<string, List<AudioClip>> SFX = new Dictionary<string, List<AudioClip>>();

    // Corgi 효과음을 위한 Dictionary
    public static readonly Dictionary<string, List<AudioClip>> Corgi = new Dictionary<string, List<AudioClip>>();

    // Audio
    public AudioMixer audiomixer;
    public AudioSource InGameAudio { get; private set; }    // SFX
    public AudioSource WorldAudio { get; private set; }     // BGM 

    public float BGMValue { get; private set; }
    public float SFXValue { get; private set; }
    public Vector3 BGMPosition { get; private set; }
    public Vector3 SFXPosition { get; private set; }

    [SerializeField] private float maxX;
    [SerializeField] private float minX;
    private Image audioUIScroll;
    private GameObject selectScrollButton;
    private GameObject AudioUIScrollParent;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

        Init_BGM();
        Init_SFX();
        Init_Corgi();
        InGameAudio = FindObjectOfType<GameManager>().GetComponent<AudioSource>();


        // position init
        BGMPosition = new Vector3(170, 0, 0);
        SFXPosition = new Vector3(170, 0, 0);
        BGMValue = 1;
        SFXValue = 1;
    }
    private void OnEnable() {
        SceneManager.sceneLoaded += FindScenLevelWhenLevelChange;
    }


    private void OnDisable() {
        SceneManager.sceneLoaded -= FindScenLevelWhenLevelChange;
    }

    private void FindScenLevelWhenLevelChange(Scene arg0, LoadSceneMode arg1) {
        string sceneName = SceneManager.GetActiveScene().name;

        /*
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in audioSources) {
            // BGM 그룹에 할당
            if (audioSource.name.Contains("BGM")) {
                audioSource.outputAudioMixerGroup = audiomixer.FindMatchingGroups("BGM")[0];
            }
            // SFX 그룹에 할당
            else if (audioSource.name.Contains("SFX")) {
                audioSource.outputAudioMixerGroup = audiomixer.FindMatchingGroups("SFX")[0];
            }
        }
        */


        if (GameManager.instance.currentStage != StageLevel.StageSelect) {

            WorldAudio = FindObjectOfType<ConvertMode>().GetComponent<AudioSource>();

            // 오디오 BGM
            string[] include = null; // 초기화
            if (sceneName.Contains("Snow")) {
                include = new string[] { "Snow", "Loop" };
            }
            else {
                include = new string[] { "Grass", "Loop" };
            }

            if (include != null) {
                string playBgm = GetDictionaryKey<string, AudioClip>(AudioManager.BGM, include);

                if (playBgm != null) { // playBgm이 null이 아닐 경우에만 재생
                    BGM_Play(WorldAudio, playBgm);
                }
                else {
                    Debug.LogWarning($"No matching BGM found for {include}.");
                }
            }
        }
        else {
            WorldAudio = GameObject.Find("BGM").GetComponent<AudioSource>();
            WorldAudio.Play();
        }
    }

    private void Init_BGM() {
        AudioClip[] bgms = Resources.LoadAll<AudioClip>("Sounds/bgm");
        foreach (AudioClip _bgm in bgms) {
            string[] splitName = _bgm.name.Split('_');
            string key = "";
            for (int i = 0; i < splitName.Length; i++) {
                if (!splitName[i].Contains("Sound") && !splitName[i].Contains("BackGround")) {
                    key += splitName[i];
                    if (i < splitName.Length - 2) {
                        key += "_";                             // 분리 기호 추가
                    }
                }
            }

            if (!BGM.ContainsKey(key)) {
                BGM[key] = _bgm;  // AudioClip을 딕셔너리에 추가
            }
        }
    }
    private void Init_SFX() {
        AudioClip[] sfxs = Resources.LoadAll<AudioClip>("Sounds/sfx");
        foreach (AudioClip _sfx in sfxs) {
            string[] splitName = _sfx.name.Split('_');
            string key = "";
            string lastPart = splitName[splitName.Length - 1];
            if (char.IsDigit(lastPart[0])) {                    // 마지막이 숫자라면 그 앞까지 key로 사용
                for (int i = 0; i < splitName.Length - 1; i++) {
                    key += splitName[i];
                    if (i < splitName.Length - 2) {
                        key += "_";                             // 분리 기호 추가
                    }
                }
            }
            else {
                key = _sfx.name;
            }

            if (!SFX.ContainsKey(key)) {
                SFX[key] = new List<AudioClip>();
            }
            SFX[key].Add(_sfx);
        }
    }

    private void Init_Corgi() {
        string[] subFolder = { "climb", "move", "putend" };

        foreach (var folderName in subFolder) {
            AudioClip[] clips = Resources.LoadAll<AudioClip>($"Sounds/corgi/{folderName}");

            if (clips.Length > 0) {
                Corgi[folderName] = new List<AudioClip>(clips);
            }
        }
    }

    public void BGM_Play(AudioSource audioSource, string key) {
        if (BGM.TryGetValue(key, out AudioClip clip)) {
            // 현재 클립이 동일하고 재생 중이라면 아무 것도 하지 않고 메서드를 종료
            if (audioSource.clip == clip && audioSource.isPlaying) {
                Debug.Log("The same clip is already playing.");
                return;
            }

            // 클립이 다르거나 재생 중이지 않다면 클립을 변경하고 재생
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    public void SFX_Play(AudioSource audioSource, string key) {
        if (SFX.TryGetValue(key, out List<AudioClip> clips)) {
            int randomIndex = Random.Range(0, clips.Count);

            // 클립이 다르거나 재생 중이지 않다면 클립을 변경하고 재생
            audioSource.clip = clips[randomIndex];
            audioSource.Play();
        }
    }
    public void Corgi_Play(AudioSource audioSource, string key) {
        if (Corgi.TryGetValue(key, out List<AudioClip> clips)) {
            // 현재 클립이 동일하고 재생 중이라면 아무 것도 하지 않고 메서드를 종료
            if (audioSource.isPlaying && clips.Contains(audioSource.clip)) {
                return;
            }

            int randomIndex = Random.Range(0, clips.Count);

            // 클립이 다르거나 재생 중이지 않다면 클립을 변경하고 재생
            audioSource.clip = clips[randomIndex];
            audioSource.Play();
        }
    }

    public void StopPlaying(AudioSource audioSource) {
        if (audioSource.isPlaying) {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }
    public string GetDictionaryKey<TKey, Tvalue>(Dictionary<TKey, Tvalue> dictionary, string[] include) {
        TKey[] keys = new TKey[dictionary.Keys.Count];
        dictionary.Keys.CopyTo(keys, 0);


        foreach (TKey _key in keys) {
            bool allIncluded = true;

            foreach (var item in include) {
                if (!_key.ToString().Contains(item)) {
                    allIncluded = false;
                    break; // 하나라도 포함되지 않으면 중단
                }
            }

            if (allIncluded) {
                return _key.ToString(); ; // 모든 include 단어를 포함하면 반환
            }
        }
        return null; // 조건을 만족하는 키가 없으면 null 반환
    }


    // Audio Mixer 해당 스크롤에 연결
    public void Audio_OnPointerDown(GameObject gameObject) {
        selectScrollButton = gameObject;
        Debug.Log(" 버튼위치 확인 | " + gameObject.name + " | position | " + gameObject.GetComponent<RectTransform>().anchoredPosition);

        AudioUIScrollParent = selectScrollButton.transform.parent.gameObject;
        audioUIScroll = AudioUIScrollParent.GetComponentsInChildren<Image>()[1];

        RectTransform scrollRect = audioUIScroll.GetComponent<RectTransform>();

        float width = scrollRect.rect.width;
        float pivotX = scrollRect.pivot.x;
        float anchoredX = scrollRect.anchoredPosition.x;

        minX = anchoredX - (width * pivotX);
        maxX = anchoredX + (width * (1 - pivotX));
    }

    public void Audio_OnPointerUp() {
        AudioUIScrollParent = null;
        audioUIScroll = null;
        selectScrollButton = null;
    }

    public void Audio_OnDrag() {

        RectTransform audioUIRect = selectScrollButton.GetComponent<RectTransform>();

        if (audioUIRect != null) {

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle( audioUIScroll.GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);

            Vector3 newPos = audioUIRect.anchoredPosition;
            newPos.x = localPoint.x;
            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);

            audioUIRect.anchoredPosition = newPos;

            float sliderValue = (newPos.x <= minX) ? 0 : Mathf.Clamp01((newPos.x - minX) / (maxX - minX));

            audioUIScroll.fillAmount = sliderValue;

            if (AudioUIScrollParent.transform.parent.name.Contains("BGM")) {
                BGMPosition = newPos;
                SetBGMVolume(sliderValue);
            }
            else {
                SFXPosition = newPos;
                SetSFXVolume(sliderValue);
            }
        }
    }
    public void SetBGMVolume(float volume) {
        audiomixer.SetFloat("BGM_Volume", volume == 0 ? -80f : Mathf.Log10(volume) * 20);
    }
    public void SetSFXVolume(float volume) {
        audiomixer.SetFloat("SFX_Volume", volume == 0 ? -80f : Mathf.Log10(volume) * 20);
    }
}

/*
 1. 오디오 어떻게하징?
 */

