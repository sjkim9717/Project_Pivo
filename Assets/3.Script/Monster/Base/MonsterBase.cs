using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBase : MonoBehaviour {

    private GameObject monster2D;
    private GameObject monster3D;
    private GameObject effect;
    private GameObject emotion;
    private Transform putPoint;
    private Transform emotionPoint2D;
    private Transform emotionPoint3D;

    private Animator ani2D;
    private Animator ani3D;
    private IMonsterStateBase currentState;
    public GameObject Monster2D { get { return monster2D; } }
    public GameObject Monster3D { get { return monster3D; } }
    public GameObject Effect { get { return effect; } }
    public GameObject Emotion { get { return emotion; } }
    public Transform EmotionPoint2D { get { return emotionPoint2D; } }
    public Transform EmotionPoint3D { get { return emotionPoint3D; } }
    public Transform PutPoint { get { return putPoint; } }
    public Animator Ani2D { get { return ani2D; } }
    public Animator Ani3D { get { return ani3D; } }
    public IMonsterStateBase CurrentState { get { return currentState; } set { currentState = value; } }

    private Vector3 moveposition;

    protected PlayerManage playerManage;

    protected virtual void Awake() {
        playerManage = FindObjectOfType<PlayerManage>();
        if (playerManage == null) Debug.LogWarning(" player manage null error in monster base");
        monster2D = base.transform.Find("Root2D").gameObject;
        monster3D = base.transform.Find("Root3D").gameObject;
        emotion = GetComponentInChildren<Canvas>().transform.GetChild(0).gameObject;
        ani2D = monster2D.GetComponent<Animator>();
        ani3D = monster3D.GetComponentInChildren<Animator>();

        putPoint = base.transform.GetChild(2);
        emotionPoint2D = base.transform.GetChild(3);
        emotionPoint3D = base.transform.GetChild(4);

        effect = base.transform.GetChild(5).gameObject;
    }

    public virtual void Change2D() {
        monster3D.SetActive(false);
        monster2D.SetActive(true);

        moveposition = monster3D.transform.position;
        monster2D.transform.position = moveposition;

        emotion.SetActive(true);
    }

    public virtual void Change3D() {
        monster2D.SetActive(false);
        monster3D.SetActive(true);

        moveposition = monster2D.transform.position;
        monster3D.transform.position = moveposition;

        emotion.SetActive(true);
    }
    public virtual void ChangeAutoMode() {
        emotion.SetActive(false);

        monster2D.SetActive(false);
        monster3D.SetActive(false);

        moveposition = transform.position;
        monster2D.transform.position = moveposition;
        monster3D.transform.position = moveposition;
    }

    public void SettingEffectActiveTrue() {
        effect.SetActive(true);

        Transform start = effect.transform.GetChild(0);
        Transform end = effect.transform.GetChild(1);

        foreach (Transform child in start) {
            if (child.TryGetComponent(out Collider col)) {
                col.gameObject.SetActive(true);
            }
            else if (child.TryGetComponent(out ParticleSystem particle)) {
                particle.Play();
            }
        }


        foreach (Transform child in end) {
            if (child.TryGetComponent(out ParticleSystem particle)) {
                particle.Play();
            }
        }

        StartCoroutine(EffectDelayTime(effect, start));
    }


    private IEnumerator EffectDelayTime(GameObject effect, Transform start) {

        yield return new WaitForSeconds(0.5f);

        effect.SetActive(false);

        foreach (Transform child in start) {
            if (child.TryGetComponent(out Collider col)) {
                col.gameObject.SetActive(false);
                break;
            }
        }
    }

}
