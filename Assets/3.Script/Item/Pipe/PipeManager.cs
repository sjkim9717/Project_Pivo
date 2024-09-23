using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeManager : MonoBehaviour
{
    [SerializeField] private Transform start;
    [SerializeField] private Transform finish;
    [SerializeField] private Transform magicStrone;

    private void Awake() {
        foreach(Transform child in transform) {
            if (child.name.Contains("Start")) start = child;
            else if (child.name.Contains("Finish")) finish = child;
            else if (child.name.Contains("MagicStone")) magicStrone = child;
        }
    }


}



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
