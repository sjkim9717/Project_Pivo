using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PipeObject : MonoBehaviour {
    public Action PipeLineFinish;
    public enum Terminal { Start, Mid, End };

    private Color originColor = Color.white;
    private Color midEmissionColor = new Color(0.5f, 0.5f, 0.7f);
    private Color endColor = new Color(0.67f, 0.996f, 0.996f);

    public Terminal State;
    private Renderer[] renderers;
    private Material midMaterial;
    private Material endMaterial;

    public PipeWaypoint Waypoint = new PipeWaypoint();

    private void Awake() {
        Waypoint.SettingPosition(gameObject, Waypoint.Start, ref Waypoint.StartPos);
        Waypoint.SettingPosition(gameObject, Waypoint.End, ref Waypoint.EndPos);

        renderers = transform.GetComponentsInChildren<Renderer>();

        midMaterial = renderers[0].material;
        if (renderers.Length > 1) {
            endMaterial = renderers[1].material;
        }

        PlayerManage.instance.IsSwitchMode += SwitchMode;

    }
    private void Start() {
        SwitchMode();
    }
    private void Update() {
        if (PlayerManage.instance.CurrentMode == PlayerMode.Player2D) {
            //TODO: 가운데는 모드가 변경될 경우 연결되어있는지 확인해서 불 밝혀야함
            if (gameObject.activeSelf) {
                if (Waypoint.IsStartConnect) {
                    if (State is Terminal.Mid) {
                        if (midMaterial.shader.name == "Shader Graphs/WorldObject") {
                            midMaterial.SetColor("_EmissionMask", midEmissionColor);
                        }
                    }
                    else if (State == Terminal.End) {
                        if (endMaterial.shader.name == "Shader Graphs/Color") {
                            endMaterial.SetColor("_Color", endColor);
                        }
                    }
                }
            }
        }
        else if (PlayerManage.instance.CurrentMode == PlayerMode.Player3D) {
            if (gameObject.activeSelf) {
                if (State == Terminal.Mid) {
                    if (!(Waypoint.IsStartConnect && Waypoint.IsEndConnect)) {
                        if (midMaterial.shader.name == "Shader Graphs/WorldObject") {
                            midMaterial.SetColor("_EmissionMask", originColor);
                        }
                    }
                }
                else if (State == Terminal.End) {
                    if (Waypoint.IsStartConnect) {
                        if (endMaterial.shader.name == "Shader Graphs/Color") {
                            endMaterial.SetColor("_Color", originColor);
                        }
                    }
                }
            }
        }

        // 현재 오브젝트가 mid일 경우 앞 커넥션 확인해서 끊겨있으면 뒤로 전달
        Waypoint.CheckObjectWhenIsConnectOff(this);

        if(State is Terminal.End) { if (Waypoint.IsStartConnect) PipeLineFinish?.Invoke(); }
    }

    #region active / component add

    private void SwitchMode() {
        if (PlayerManage.instance.CurrentMode is PlayerMode.Player2D) {
            gameObject.SetActive(IsInSelectArea());
        }
        else {
            gameObject.SetActive(true);
        }

        if (gameObject.activeSelf) {
            StartCoroutine(SwitchModeCoroutine());

        }
    }

    // component
    private IEnumerator SwitchModeCoroutine() {

        bool is3DMode = PlayerManage.instance.CurrentMode == PlayerMode.Player3D;

        if (is3DMode) {
            DestroyIfExists<Rigidbody2D>();
            DestroyIfExists<BoxCollider2D>();
            yield return null; // 다음 프레임까지 대기
            AddIfNotExists<Rigidbody>();
            AddIfNotExists<BoxCollider>();
        }
        else {
            DestroyIfExists<Rigidbody>();
            DestroyIfExists<BoxCollider>();
            yield return null; // 다음 프레임까지 대기
            AddIfNotExists<Rigidbody2D>();
            AddIfNotExists<BoxCollider2D>();
        }
    }

    private void DestroyIfExists<T>() where T : Component {
        T component = gameObject.GetComponent<T>();
        if (component != null) {
            Destroy(component);
        }
    }

    private void AddIfNotExists<T>() where T : Component {
        if (gameObject.GetComponent<T>() == null) {
            T component = gameObject.AddComponent<T>();
            ConfigureComponent(component);
        }
    }

    private void ConfigureComponent<T>(T component) where T : Component {
        if (component is BoxCollider boxCollider) {
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector3(2, 2, 2); // 3D 콜라이더의 크기 설정
        }
        else if (component is BoxCollider2D boxCollider2D) {
            boxCollider2D.isTrigger = true;
            boxCollider2D.size = new Vector2(2, 2); // 2D 콜라이더의 크기 설정
        }
        else if (component is Rigidbody) {
            ((Rigidbody)(object)component).isKinematic = true;
        }
        else if (component is Rigidbody2D) {
            ((Rigidbody2D)(object)component).isKinematic = true;
        }
    }


    //TODO: [수정 필요] 문제가 있구만
    private bool IsInSelectArea() {
        if (PlayerManage.instance.StartSection.z >= PlayerManage.instance.FinishSection.z) {
            if (transform.position.z <= PlayerManage.instance.StartSection.z && transform.position.z >= PlayerManage.instance.FinishSection.z) return true;
            else return false;
        }
        else {
            if (transform.position.z >= PlayerManage.instance.StartSection.z && transform.position.z <= PlayerManage.instance.FinishSection.z) return true;
            else return false;
        }
    }
    #endregion

    #region collision check
    // TODO: x, y축으로 겹칠 수 있음 z축 -쪽을 우선 확인해야함 근데 동시 처리가 안 됨

    private void OnTriggerEnter(Collider other) {
        if (other.transform.TryGetComponent(out PipeObject pipe)) {
            if (State == Terminal.Start)
                Waypoint.StartingPipeLine(this, pipe);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.transform.TryGetComponent(out PipeObject pipe)) {
            if (State == Terminal.Start)
                Waypoint.StartingPipeLine(this, pipe);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.transform.TryGetComponent(out PipeObject pipe)) {
            if (State == Terminal.Mid)
                Waypoint.StartingPipeLine(this, pipe);
        }
    }
    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.transform.TryGetComponent(out PipeObject pipe)) {
            if (State == Terminal.Mid)
                Waypoint.StartingPipeLine(this, pipe);
        }
    }


    private void OnTriggerExit(Collider other) {
        if (other.transform.TryGetComponent(out PipeObject pipe)) {
            Waypoint.EndingPipeLine(this, pipe);
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.transform.TryGetComponent(out PipeObject pipe)) {
            Waypoint.EndingPipeLine(this, pipe);
        }
    }


    #endregion
}

/*
 1. 목적 : 스킬이 발동되었을 경우 자기 자신의 노드 값을 확인하여 material 색상변경
 2. 
- 각 필요한 위치를 가지고 있음
- 스킬이 실행되었을 경우
 자신의 위치를 확인하고 
온오프된다.
활성화 되어있다면,
자기 자신의 노드 값을 확인하여 material 색상변경
 */