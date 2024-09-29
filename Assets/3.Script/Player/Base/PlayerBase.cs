using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour {
    private PlayerMode currentMode;
    private PlayerState currentState;

    protected GameObject player2D;
    protected GameObject player3D;

    private Rigidbody rigid3D;
    private Rigidbody2D rigid2D;

    private Animator ani2D;
    private Animator ani3D;

    private GameObject effect;

    public PlayerMode CurrentMode { get { return currentMode; } set { currentMode = value; } }
    public PlayerState CurrentState { get { return currentState; } set { currentState = value; } }

    public GameObject Player2D { get { return player2D; } }
    public GameObject Player3D { get { return player3D; } }

    public Rigidbody PlayerRigid3D { get { return rigid3D; } }
    public Rigidbody2D PlayerRigid2D { get { return rigid2D; } }

    public Animator Ani2D { get { return ani2D; } }
    public Animator Ani3D { get { return ani3D; } }
    public GameObject Effect { get { return effect; } }


    protected Vector3 moveposition = Vector3.zero;

    private Vector3 startSection = Vector3.zero;
    private Vector3 finishSection = Vector3.zero;

    public Vector3 StartSection { get { return startSection; } set { startSection = value; } }
    public Vector3 FinishSection { get { return finishSection; } set { finishSection = value; } }

    protected virtual void Awake() {
        player2D = base.transform.Find("Root2D").gameObject;
        player3D = base.transform.Find("Root3D").gameObject;

        moveposition = Vector3.zero;

        rigid3D = player3D.GetComponent<Rigidbody>();
        rigid2D = player2D.GetComponent<Rigidbody2D>();

        ani2D = player2D.GetComponent<Animator>();
        ani3D = player3D.GetComponentInChildren<Animator>();

        effect = base.transform.GetChild(5).gameObject;

        currentState = PlayerState.Idle;
    }

    private void OnEnable() {
        moveposition = Vector3.zero;
    }

    public virtual void Change2D() {
        moveposition = player3D.transform.position;

        effect.transform.position = moveposition;
        SettingEffectActiveTrue();

        currentMode = PlayerMode.Player2D;

        player3D.SetActive(false);
        player2D.SetActive(true);

        player2D.transform.position = moveposition;

    }

    public virtual void Change3D() {
        //TODO: player 사망시 player2d가 파괴되어있는 상태
        if (player2D != null) {
            moveposition = player2D.transform.position;
        }
        else {
            moveposition = Vector3.zero;
        }

        effect.transform.position = moveposition;
        SettingEffectActiveTrue();

        currentMode = PlayerMode.Player3D;

        player2D.SetActive(false);
        player3D.SetActive(true);

        player3D.transform.position = moveposition;
    }
    public virtual void ChangeAutoMode() {
        currentMode = PlayerMode.AutoMode;

        player2D.SetActive(false);  
        player3D.SetActive(false);

        moveposition = transform.position;
        player2D.transform.position = moveposition;
        player3D.transform.position = moveposition;
    }
    public virtual void ChangeStageClear() {
        currentMode = PlayerMode.AutoMode;

        player2D.SetActive(false);
        player3D.SetActive(false);

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
