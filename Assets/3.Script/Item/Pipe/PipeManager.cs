using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeManager : MonoBehaviour
{
    [SerializeField] private float radius;                          // 몬스터의 상태를 변경시키는 반경
    [SerializeField] private Transform start;
    [SerializeField] private Transform finish;
    [SerializeField] private Transform magicStrone;
    [SerializeField] private bool isFinished;
    private void Awake() {
        foreach(Transform child in transform) {
            if (child.name.Contains("Start")) start = child;
            else if (child.name.Contains("Finish")) finish = child;
            else if (child.name.Contains("MagicStone")) magicStrone = child;
        }

        PlayerManage.instance.IsSwitchMode += CheckEndObject;

    }


    private void Update() {
        if (isFinished) {
            if (PlayerManage.instance.CurrentMode is PlayerMode.Player3D) {
                Collider[] colliders = Physics.OverlapSphere(magicStrone.position, radius);
                foreach (Collider each in colliders) {
                    if (each.transform.TryGetComponent(out Monster3DControl monster3D)) {
                        monster3D.ChangeState(monster3D.PassOut3DState);
                    }
                }
            }
            else if (PlayerManage.instance.CurrentMode is PlayerMode.Player3D) {
                Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(magicStrone.position, radius);
                foreach(Collider2D each in collider2Ds) {
                    if(each.transform.TryGetComponent(out Monster2DControl monster2D)) {
                        monster2D.ChangeState(monster2D.PassOut2DState);
                    }
                }
            }
            }
    }

    private void CheckEndObject() {
        if (finish.TryGetComponent(out PipeObject pipeObject)) {
            if (pipeObject.Waypoint.IsStartConnect) {
                //TODO: 마지막 오브젝트가 연결됬을 경우 -> magic stone의 색 변경, monster 상태변화 => update
                isFinished = true;

                Renderer[] renderers = magicStrone.GetComponentsInChildren<Renderer>();
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
        }
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

    public void SettingPosition(GameObject gameObject, Direction direction, ref Vector3 position) {
        if (Waypoint.TryGetValue(direction, out Vector2 pos)) {
            position = gameObject.transform.position + (Vector3)pos;
        }
    }

    public void SettingConnection(Direction direction, ref bool isConnection) {
        if (direction == Start) IsStartConnect = true;
        else if (direction == End) IsEndConnect = true;
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
}
