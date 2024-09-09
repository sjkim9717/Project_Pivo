using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour {
    private PlayerMode currentMode;
    private PlayerState currentState;

    private GameObject player2D;
    private GameObject player3D;

    private Rigidbody rigid3D;
    private Rigidbody2D rigid2D;

    private Animator ani2D;
    private Animator ani3D;

    public PlayerMode CurrentMode { get { return currentMode; } set { currentMode = value; } }
    public PlayerState CurrentState { get { return currentState; } set { currentState = value; } }

    public GameObject Player2D { get { return player2D; } }
    public GameObject Player3D { get { return player3D; } }

    public Rigidbody PlayerRigid3D { get { return rigid3D; } }
    public Rigidbody2D PlayerRigid2D { get { return rigid2D; } }

    public Animator Ani2D { get { return ani2D; } }
    public Animator Ani3D { get { return ani3D; } }


    private Vector3 moveposition;

    protected virtual void Awake() {
        player2D = base.transform.Find("Root2D").gameObject;
        player3D = base.transform.Find("Root3D").gameObject;

        moveposition = Vector3.zero;

        rigid3D = player3D.GetComponent<Rigidbody>();
        rigid2D = player2D.GetComponent<Rigidbody2D>();

        ani2D = player2D.GetComponent<Animator>();
        ani3D = player3D.GetComponentInChildren<Animator>();

        currentState = PlayerState.Idle;
    }

    public virtual void Change2D() {
        currentMode = PlayerMode.Player2D;

        player3D.SetActive(false);
        player2D.SetActive(true);

        moveposition = player3D.transform.position;
        player2D.transform.position = moveposition;
    }

    public virtual void Change3D() {
        currentMode = PlayerMode.Player3D;

        player2D.SetActive(false);
        player3D.SetActive(true);

        moveposition = player2D.transform.position;
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

}
