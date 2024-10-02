using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    public static AudioManager instance { get; private set; }

    public AudioMixer audiomixer;
    public AudioSource WorldAudio;

    // BGM을 위한 Dictionary
    public static readonly Dictionary<string, AudioClip> BGM = new Dictionary<string, AudioClip>();

    // SFX를 위한 Dictionary
    public static readonly Dictionary<string, List<AudioClip>> SFX = new Dictionary<string, List<AudioClip>>();

    // Corgi 효과음을 위한 Dictionary
    public static readonly Dictionary<string, List<AudioClip>> Corgi = new Dictionary<string, List<AudioClip>>();

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }

        WorldAudio = GetComponent<AudioSource>();

        Init_BGM();
        Init_SFX();
        Init_Corgi();
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
        string[] subFolder = Directory.GetDirectories(Application.dataPath + "/Resources/Sounds/corgi");
        foreach (var folder in subFolder) {
            string folderName = Path.GetFileName(folder);
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
        if (SFX.TryGetValue(key, out List< AudioClip > clips)) {
            // 현재 클립이 동일하고 재생 중이라면 아무 것도 하지 않고 메서드를 종료
            if (audioSource.isPlaying) {
                Debug.Log("The clip is already playing.");
                return;
            }

            int randomIndex = Random.Range(0, clips.Count);

            // 클립이 다르거나 재생 중이지 않다면 클립을 변경하고 재생
            audioSource.clip = clips[randomIndex];
            audioSource.Play();
        }
    }
    public void Corgi_Play(AudioSource audioSource, string key) {
        if (Corgi.TryGetValue(key, out List<AudioClip> clips)) {
            // 현재 클립이 동일하고 재생 중이라면 아무 것도 하지 않고 메서드를 종료
            if (audioSource.isPlaying) {
                Debug.Log("The clip is already playing.");
                return;
            }

            int randomIndex = Random.Range(0, clips.Count);

            // 클립이 다르거나 재생 중이지 않다면 클립을 변경하고 재생
            audioSource.clip = clips[randomIndex];
            audioSource.Play();
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


}

/*
 1. 오디오 어떻게하징?
 */

