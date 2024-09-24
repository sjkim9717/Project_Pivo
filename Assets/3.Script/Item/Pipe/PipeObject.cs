using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PipeObject : MonoBehaviour {
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
        if (renderers.Length >1) {
            endMaterial = renderers[1].material;
        }

        PlayerManage.instance.IsSwitchMode += SwitchMode;

    }

    private void Update() {
        if (PlayerManage.instance.CurrentMode == PlayerMode.Player2D) {
            //TODO: 가운데는 모드가 변경될 경우 연결되어있는지 확인해서 불 밝혀야함
            if (gameObject.activeSelf) {
                if (State == Terminal.Mid) {
                    if (Waypoint.IsStartConnect && Waypoint.IsEndConnect) {                        
                        if (midMaterial.shader.name == "Shader Graphs/WorldObject") {
                            midMaterial.SetColor("_EmissionMask", midEmissionColor);
                        }
                    }
                }
                else if(State == Terminal.End) {
                    if (Waypoint.IsStartConnect ) {
                        if (endMaterial.shader.name == "Shader Graphs/Color") {
                            endMaterial.SetColor("_Color", endColor);
                        }
                    }
                }
            }
        }
        else if(PlayerManage.instance.CurrentMode == PlayerMode.Player3D) {
            if (gameObject.activeSelf) {
                if (State == Terminal.Mid) {
                    if (!(Waypoint.IsStartConnect && Waypoint.IsEndConnect)) {
                        if (midMaterial.shader.name == "Shader Graphs/WorldObject") {
                            Debug.LogWarning(" 해제가 되어야하는 시점임" + gameObject.name);
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

    }

    #region active / component add

    private void SwitchMode() {
        if (PlayerManage.instance.CurrentMode is PlayerMode.Player2D) {
            if (IsInSelectArea()) {
                gameObject.SetActive(true);
                StartCoroutine(SwitchModeCoroutine());
            }
            else {
                gameObject.SetActive(false);
            }
        }
        else {
            gameObject.SetActive(true);
        }
    }

    // component
    private IEnumerator SwitchModeCoroutine() {

        Debug.Log("Switching mode...");
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
            Debug.Log("PipeObject | Trigger Enter | " + gameObject.name + " | " + other.name);
            if (State == Terminal.Start) {
                Debug.Log("PipeObject | Trigger Enter | Stat = Start | " + gameObject.name + " | " + other.name);
                if (Waypoint.MatchConnection(State, pipe.Waypoint)) {
                    Debug.Log("PipeObject | Trigger Enter | Stat = Start |  waypoint.MatchConnection | " + gameObject.name + " | " + other.name);
                    Waypoint.IsEndConnect = true;
                    if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = true;
                }
                else {
                    Debug.Log("PipeObject | Trigger Enter | Stat = Start |  waypoint.MatchConnection => error|  " + gameObject.name + " | " + other.name);
                }
            }
            else if (State == Terminal.Mid) {
                Debug.Log("PipeObject | Trigger Enter | Stat = Mid | " + gameObject.name + " | " + other.name);
                if (Waypoint.IsStartConnect) {
                    Debug.Log("PipeObject | Trigger Enter | Stat = Mid | Start? | " + gameObject.name + " | " + other.name);
                    if (Waypoint.MatchConnection(State, pipe.Waypoint)) {
                        Debug.Log("PipeObject | Trigger Enter |  Stat = Mid | Start? |  waypoint.MatchConnection | " + gameObject.name + " | " + other.name);
                        Waypoint.IsEndConnect = true;
                        if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = true;

                    }
                    else {
                        Debug.Log("PipeObject | Trigger Enter | Stat = Start |  waypoint.MatchConnection => error|  " + gameObject.name + " | " + other.name);
                    }
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.transform.TryGetComponent(out PipeObject pipe)) {
            Debug.Log("PipeObject | Trigger Enter | " + gameObject.name + " | " + collision.name);
            if (State == Terminal.Start) {
                Debug.Log("PipeObject | Trigger Enter | Stat = Start | " + gameObject.name + " | " + collision.name);
                if (Waypoint.MatchConnection(State, pipe.Waypoint)) {
                    Debug.Log("PipeObject | Trigger Enter | Stat = Start |  waypoint.MatchConnection | " + gameObject.name + " | " + collision.name);
                    Waypoint.IsEndConnect = true;
                    if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = true;
                }
                else {
                    Debug.Log("PipeObject | Trigger Enter | Stat = Start |  waypoint.MatchConnection => error|  " + gameObject.name + " | " + collision.name);
                }
            }
            else if (State == Terminal.Mid) {
                Debug.Log("PipeObject | Trigger Enter | Stat = Mid | " + gameObject.name + " | " + collision.name);
                if (Waypoint.IsStartConnect) {
                    Debug.Log("PipeObject | Trigger Enter | Stat = Mid | Start? | " + gameObject.name + " | " + collision.name);
                    if (Waypoint.MatchConnection(State, pipe.Waypoint)) {
                        Debug.Log("PipeObject | Trigger Enter |  Stat = Mid | Start? |  waypoint.MatchConnection | " + gameObject.name + " | " + collision.name);
                        Waypoint.IsEndConnect = true;
                        if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = true;

                    }
                    else {
                        Debug.Log("PipeObject | Trigger Enter | Stat = Start |  waypoint.MatchConnection => error|  " + gameObject.name + " | " + collision.name);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.transform.TryGetComponent(out PipeObject pipe)) {
            Debug.Log("PipeObject | Trigger Exit | " + gameObject.name + " | " + other.name);
            if (State == Terminal.Start) {
                if (Waypoint.MatchConnection(State, pipe.Waypoint)) {
                    Waypoint.IsEndConnect = false;
                    if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = false;
                }
            }
            else if (State == Terminal.Mid) {
                if (!Waypoint.IsStartConnect) {
                    if (Waypoint.MatchConnection(State, pipe.Waypoint)) {
                        Waypoint.IsEndConnect = false;
                        if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = false;

                    }
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.transform.TryGetComponent(out PipeObject pipe)) {
            Debug.Log("PipeObject | Trigger Exit | " + gameObject.name + " | " + collision.name);
            if (State == Terminal.Start) {
                if (Waypoint.MatchConnection(State, pipe.Waypoint)) {
                    Waypoint.IsEndConnect = false;
                    if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = false;
                }
            }
            else if (State == Terminal.Mid) {
                if (!Waypoint.IsStartConnect) {
                    if (Waypoint.MatchConnection(State, pipe.Waypoint)) {
                        Waypoint.IsEndConnect = false;
                        if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = false;

                    }
                }
            }
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