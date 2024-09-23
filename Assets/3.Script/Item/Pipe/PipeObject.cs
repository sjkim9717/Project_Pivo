using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PipeObject : MonoBehaviour {
    private Color originColor = Color.white;
    private Color emissionColor = new Color(0.5f, 0.5f, 0.7f);
    public enum Terminal { Start, Mid, End };

    public Terminal State;

    public PipeWaypoint Waypoint = new PipeWaypoint();

    private void Awake() {
        Waypoint.SettingPosition(gameObject, Waypoint.Start, ref Waypoint.StartPos);
        Waypoint.SettingPosition(gameObject, Waypoint.End, ref Waypoint.EndPos);

        PlayerManage.instance.IsSwitchMode += SwitchMode;

    }

    private void Update() {
        if (PlayerManage.instance.CurrentMode == PlayerMode.Player2D) {
            //TODO: 가운데는 모드가 변경될 경우 연결되어있는지 확인해서 불 밝혀야함
            if (gameObject.activeSelf) {
                if (State == Terminal.Mid) {
                    Material material = transform.GetComponentInChildren<Renderer>().material;
                    if (Waypoint.IsStartConnect && Waypoint.IsEndConnect) {
                        if (material.shader.name == "Shader Graph/WorldObject") {
                            material.SetColor("_EmissionMask", emissionColor);
                        }
                    }
                    else {
                        if (material.shader.name == "Shader Graph/WorldObject") {
                            material.SetColor("_EmissionMask", originColor);
                        }
                    }
                }
            }
        }
    }

    #region 활성화

    private void SwitchMode() {
        gameObject.SetActive(IsInSelectArea());
        if (gameObject.activeSelf) {
            StartCoroutine(SwitchModeCoroutine());
        }
    }
    //TODO: 박스들이 도망다님
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
            gameObject.AddComponent<T>();
        }
    }

    /*
    private void SwitchMode() {
        gameObject.SetActive(IsInSelectArea());
        if (gameObject.activeSelf) {
            Debug.Log("????????????");
            bool is3DMode = PlayerManage.instance.CurrentMode == PlayerMode.Player3D;

            if (is3DMode) {
                // 3D 컴포넌트 처리
                if (gameObject.GetComponent<Rigidbody2D>() != null) Destroy(gameObject.GetComponent<Rigidbody2D>());
                if (gameObject.GetComponent<BoxCollider2D>() != null) Destroy(gameObject.GetComponent<BoxCollider2D>());

                if (gameObject.GetComponent<Rigidbody>() == null) gameObject.AddComponent<Rigidbody>();
                if (gameObject.GetComponent<BoxCollider>() == null) gameObject.AddComponent<BoxCollider>();
            }
            else {
                if (gameObject.GetComponent<Rigidbody>() != null) Destroy(gameObject.GetComponent<Rigidbody>());
                if (gameObject.GetComponent<BoxCollider>() != null) Destroy(gameObject.GetComponent<BoxCollider>());

                // 2D 컴포넌트 처리
                if (gameObject.GetComponent<Rigidbody2D>() == null) gameObject.AddComponent<Rigidbody2D>();
                if (gameObject.GetComponent<BoxCollider2D>() == null) gameObject.AddComponent<BoxCollider2D>();
            }
        }
    }
    */
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
            if (State == Terminal.Start) {
                if (Waypoint.MatchConnection(State, pipe.Waypoint)) {
                    Waypoint.IsEndConnect = false;
                    if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = false;
                }
            }
            else if (State == Terminal.Mid) {
                if (Waypoint.IsStartConnect) {
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
            if (State == Terminal.Start) {
                if (Waypoint.MatchConnection(State, pipe.Waypoint)) {
                    Waypoint.IsEndConnect = false;
                    if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = false;
                }
            }
            else if (State == Terminal.Mid) {
                if (Waypoint.IsStartConnect) {
                    if (Waypoint.MatchConnection(State, pipe.Waypoint)) {
                        Waypoint.IsEndConnect = false;
                        if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = false;

                    }
                }
            }
        }
    }


    /*
     
    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.TryGetComponent(out PipeObject pipe)) {
            if (State == Terminal.Start) {
                if (Waypoint.MatchConnection(State, pipe.Waypoint)) {
                    Waypoint.IsEndConnect = true;
                    if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = true;
                }
            }
            else if (State == Terminal.Mid) {
                if (Waypoint.IsStartConnect) {
                    if (Waypoint.MatchConnection(State, pipe.Waypoint)) {
                        Waypoint.IsEndConnect = true;
                        if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = true;

                    }
                }
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.TryGetComponent(out PipeObject pipe)) {
            if (State == Terminal.Start) {
                if (Waypoint.MatchConnection(State, pipe.Waypoint)) {
                    Waypoint.IsEndConnect = true;
                    if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = true;
                }
            }
            else if (State == Terminal.Mid) {
                if (Waypoint.IsStartConnect) {
                    if (Waypoint.MatchConnection(State, pipe.Waypoint)) {
                        Waypoint.IsEndConnect = true;
                        if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = true;

                    }
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision) {
        if (collision.transform.TryGetComponent(out PipeObject pipe)) {
            if (State == Terminal.Start) {
                if (Waypoint.MatchConnection(State, pipe.Waypoint)) {
                    Waypoint.IsEndConnect = false;
                    if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = false;
                }
            }
            else if (State == Terminal.Mid) {
                if (Waypoint.IsStartConnect) {
                    if (Waypoint.MatchConnection(State, pipe.Waypoint)) {
                        Waypoint.IsEndConnect = false;
                        if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = false;

                    }
                }
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.transform.TryGetComponent(out PipeObject pipe)) {
            if (State == Terminal.Start) {
                if (Waypoint.MatchConnection(State, pipe.Waypoint)) {
                    Waypoint.IsEndConnect = false;
                    if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = false;
                }
            }
            else if (State == Terminal.Mid) {
                if (Waypoint.IsStartConnect) {
                    if (Waypoint.MatchConnection(State, pipe.Waypoint)) {
                        Waypoint.IsEndConnect = false;
                        if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = false;

                    }
                }
            }
        }
    }
     
     */
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