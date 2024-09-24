using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeManager : MonoBehaviour {
    [SerializeField] private float radius;                          // 몬스터의 상태를 변경시키는 반경
    [SerializeField] private bool isFinished;
    private bool isChangeState;

    [SerializeField] private Transform start;
    [SerializeField] private Transform finish;
    [SerializeField] private Transform magicStone;

    private PipeObject finishPipeObject;
    private void Awake() {
        foreach (Transform child in transform) {
            if (child.name.Contains("Start")) start = child;
            else if (child.name.Contains("Finish")) finish = child;        
            else if (child.name.Contains("MagicStone")) magicStone = child;
        }

        finishPipeObject = finish.GetComponent<PipeObject>();

        PlayerManage.instance.IsSwitchMode += ChangeMonsterMode;
    }

    private void ChangeMonsterMode() {
        isChangeState = false;
    }

    private void Update() {

        if (isFinished && !isChangeState) {
            isChangeState = true;

            if (PlayerManage.instance.CurrentMode is PlayerMode.Player3D) {
                Collider[] colliders = Physics.OverlapSphere(magicStone.position, radius);
                foreach (Collider each in colliders) {
                    if (each.transform.TryGetComponent(out Monster3DControl monster3D)) {
                        monster3D.ChangeState(monster3D.PassOut3DState);
                    }
                }
            }
            else if (PlayerManage.instance.CurrentMode is PlayerMode.Player2D) {
                Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(magicStone.position, radius);
                foreach (Collider2D each in collider2Ds) {
                    if (each.transform.TryGetComponent(out Monster2DControl monster2D)) {
                        monster2D.ChangeState(monster2D.PassOut2DState);
                    }
                }
            }
        }
        else {
            CheckEndObject(finishPipeObject);
        }
    }

    //TODO: 마지막 오브젝트가 연결됬을 경우 -> 처음부터 끝까지 연결됬는지 확인
    private void CheckEndObject(PipeObject pipeObject) {
        switch (pipeObject.State) {
            case PipeObject.Terminal.Start:
                ChangeMaterialWhenFinish();
                break;
            case PipeObject.Terminal.Mid:
            case PipeObject.Terminal.End:
                if (pipeObject.Waypoint.IsStartConnect) {
                    if (pipeObject.Waypoint.PrevObject != null) {
                        PipeObject prevPipeObject = pipeObject.Waypoint.PrevObject.GetComponent<PipeObject>();
                        CheckEndObject(prevPipeObject);
                    }
                }
                break;
            default:
                break;
        }
    }

    private void ChangeMaterialWhenFinish() {
        isFinished = true;

        Renderer[] renderers = magicStone.GetComponentsInChildren<Renderer>();
        foreach (Renderer item in renderers) {
            // 렌더러의 머티리얼을 가져옴
            Material material = item.material;

            // material.shader.name이 URP Lit인지 확인
            if (material.shader.name == "Universal Render Pipeline/Lit") {
                // Emission을 활성화
                material.EnableKeyword("_EMISSION");

                //// Emission Color 설정 (필요에 따라 값을 변경)
                //material.SetColor("_EmissionColor", Color.white * 1.5f);
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;        // Set the Gizmo color
        Gizmos.DrawWireSphere(magicStone.position, radius);
    }
}

/*
 1. 목적 : switch 될 때 마다 마지막 오브젝트 확인해서 연결됬으면 몬스터 상태 변화
 - 마지막 오브젝트를 확인하여
isstart가 true일 경우
magic strone을 색을 변경하고
monster의 상태를 변화해야함
 */

[Serializable]
public class PipeWaypoint {
    public readonly Dictionary<Direction, Vector2> Waypoint = new Dictionary<Direction, Vector2> {
        {Direction.Up , new Vector2(0,1) },
        {Direction.Down , new Vector2(0,-1) },
        {Direction.Left , new Vector2(-1,0) },
        {Direction.Right , new Vector2(1,0) }
    };


    public Direction Start;
    public Direction End;
    public Vector3 StartPos;
    public Vector3 EndPos;
    public bool IsStartConnect;
    public bool IsEndConnect;
    public GameObject PrevObject;
    public GameObject NextObject;
    public void SettingObject_Start(GameObject gameObject) { }

    public void SettingPosition(GameObject gameObject, Direction direction, ref Vector3 position) {
        if (Waypoint.TryGetValue(direction, out Vector2 pos)) {
            position = gameObject.transform.position + (Vector3)pos;
        }
    }

    public void SettingPositionAdditional(Vector3 addPosition) {
        StartPos = addPosition;
        EndPos = addPosition;
    }

    // 거리 확인해서 가까우면 true
    public bool MatchConnection(PipeObject.Terminal startTerminal, PipeWaypoint next) {
        float distance = 0f;
        switch (startTerminal) {
            case PipeObject.Terminal.Start:
                distance = Vector3.Distance((Vector2)EndPos, (Vector2)next.StartPos);
                break;
            case PipeObject.Terminal.Mid:
                distance = Vector3.Distance((Vector2)EndPos, (Vector2)next.StartPos);
                break;
            case PipeObject.Terminal.End:
                break;
            default:
                break;
        }

        return distance <= 0.1f;

    }

    // pipe line이 연결되면, 해당하는 gameobject에 담고 bool값을 바꿈
    public void StartingPipeLine(PipeObject Current, PipeObject nextTer) {

        switch (Current.State) {
            case PipeObject.Terminal.Start:
                SettingObjectWhenStartPipeLine(Current, nextTer);
                break;
            case PipeObject.Terminal.Mid:
                if (Current.Waypoint.IsStartConnect) {
                    SettingObjectWhenStartPipeLine(Current, nextTer);
                }
                break;
            case PipeObject.Terminal.End:
                break;
            default:
                break;
        }
    }

    private void SettingObjectWhenStartPipeLine(PipeObject Current, PipeObject nextTer) {
        if (Current.Waypoint.MatchConnection(Current.State, nextTer.Waypoint)) {
            Current.Waypoint.NextObject = nextTer.gameObject;
            Current.Waypoint.IsEndConnect = true;

            if (nextTer.State != PipeObject.Terminal.Start) {
                nextTer.Waypoint.PrevObject = Current.gameObject;
                nextTer.Waypoint.IsStartConnect = true;
            }
        }
    }

    // pipe line이 끊기면 해당하는 gameobject를 null로 변경하고 bool값 변경함 
    public void EndingPipeLine(PipeObject Current, PipeObject nextTer) {
        switch (Current.State) {
            case PipeObject.Terminal.Start:
                SettingObjectWhenEndingPipeLine(Current, nextTer);
                break;
            case PipeObject.Terminal.Mid:
                SettingObjectWhenEndingPipeLine(Current, nextTer);
                break;
            case PipeObject.Terminal.End:
                break;
            default:
                break;
        }
    }

    private void SettingObjectWhenEndingPipeLine(PipeObject Current, PipeObject nextTer) {
        if (Current.Waypoint.NextObject == nextTer.gameObject) {
            Current.Waypoint.IsEndConnect = false;
            nextTer.Waypoint.IsStartConnect = false;

            Current.Waypoint.NextObject = null;
            nextTer.Waypoint.PrevObject = null;
        }
    }

    public void CheckObjectWhenIsConnectOff(PipeObject Current) {
        if (Current.State is PipeObject.Terminal.Mid) {
            if (!Current.Waypoint.IsStartConnect && Current.Waypoint.IsEndConnect) {
                Current.Waypoint.IsEndConnect = false;

                PipeObject nextPipeobject = Current.Waypoint.NextObject.GetComponent<PipeObject>();
                nextPipeobject.Waypoint.IsStartConnect = false;
                nextPipeobject.Waypoint.PrevObject = null;

                Current.Waypoint.NextObject = null;
            }
        }
    }

}
