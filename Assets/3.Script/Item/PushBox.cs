using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBox : MonoBehaviour, IPushBox {
    [SerializeField] private int moveMaxCount;
    [SerializeField] private int moveMinCount;
    [SerializeField] private float moveSpeed = 5f;

    private bool isMoveXpos;

    private List<GameObject> tileObject = new List<GameObject>();
    private GameObject pushBox;
    [SerializeField] private GameObject PipeObject;

    private Vector3 startPos = Vector3.zero;
    private Vector3 finishPos = Vector3.zero;
    private Vector3 BoxToMove;

    private Vector3 pipestartPos = Vector3.zero;
    private Vector3 pipefinishPos = Vector3.zero;

    private AudioSource pushboxAudio;

    private void Awake() {
        foreach (Transform child in transform) {
            if (child.tag == "PushBox") {
                pushBox = child.gameObject;
                BoxToMove = pushBox.transform.position;
                //Debug.Log("PushBox | " + pushBox.name);
            }
            else if (child.tag == "PushTile") {
                tileObject.Add(child.gameObject);
                //Debug.Log("PushTile | " + child.name);
            }
        }

        isPushBoxXMoving(ref isMoveXpos);
        SavePosition(isMoveXpos);
        pushboxAudio = GetComponentInChildren<AudioSource>();
    }
    private void Start() {
        FindMinMaxCount();

        var pipe = GetComponentInChildren<PipeObject>();

        if (pipe != null) {
            PipeObject = pipe.gameObject;
            pipestartPos = PipeObject.GetComponent<PipeObject>().Waypoint.StartPos;
            pipefinishPos = PipeObject.GetComponent<PipeObject>().Waypoint.EndPos;
        }
    }

    private void FindMinMaxCount() {
        if (isMoveXpos) {
            for (int i = 1; i < tileObject.Count; i++) {
                if (tileObject[i].transform.position.x >= pushBox.transform.position.x) {
                    moveMaxCount += 1;
                }
                else {
                    moveMinCount -= 1;
                }
            }
        }
        else {
            for (int i = 1; i < tileObject.Count; i++) {
                if (tileObject[i].transform.position.z >= pushBox.transform.position.z) {
                    moveMaxCount += 1;
                }
                else {
                    moveMinCount -= 1;
                }
            }
        }
    }

    private void isPushBoxXMoving(ref bool isMoveXpos) {

        for (int i = 0; i < tileObject.Count - 1; i++) {
            if (tileObject[i].transform.position.x != tileObject[i + 1].transform.position.x) {
                isMoveXpos = true;
                break;
            }
            else if (tileObject[i].transform.position.z != tileObject[i + 1].transform.position.z) {
                isMoveXpos = false;
                break;
            }
        }
    }

    private void SavePosition(bool isPushBoxXMoving) {

        startPos = tileObject[0].transform.position;
        finishPos = tileObject[0].transform.position;

        foreach (GameObject item in tileObject) {
            Vector3 pos = item.transform.position;

            if (isPushBoxXMoving) {
                if (pos.x > finishPos.x) finishPos = pos;
                if (pos.x < startPos.x) startPos = pos;
            }
            else {
                if (pos.z > finishPos.z) finishPos = pos;
                if (pos.z < startPos.z) startPos = pos;
            }
        }
    }

    private void Update() {
        if (pushBox.transform.position != BoxToMove) {
            pushBox.transform.position = Vector3.Lerp(pushBox.transform.position, BoxToMove, Time.deltaTime * moveSpeed);
        }
    }


    // player interaction 
    public void IInteractionPushBox(float horizontal, float vertical) {
        if (horizontal == 0 && vertical == 0) return;

        float up = (horizontal > 0 || vertical > 0) ? 1 : -1;

        if (!pushboxAudio.isPlaying) pushboxAudio.Play();

        if (PipeObject != null) {

            PipeWaypoint waypoint = PipeObject.GetComponent<PipeObject>().Waypoint;

            if (isMoveXpos) {
                Vector3 newPosStart = new Vector3(
                    Mathf.Clamp(waypoint.StartPos.x + up * 2, pipestartPos.x + moveMinCount * 2, pipestartPos.x + moveMaxCount * 2),
                  pipestartPos.y, pipestartPos.z);
                waypoint.StartPos = newPosStart;

                Vector3 newPosEnd = new Vector3(
                    Mathf.Clamp(waypoint.EndPos.x + up * 2, pipefinishPos.x + moveMinCount * 2, pipefinishPos.x + moveMaxCount * 2),
                  pipefinishPos.y, pipefinishPos.z);
                waypoint.EndPos = newPosEnd;
            }
            else {
                Vector3 newPosStart = new Vector3(pipestartPos.x, pipestartPos.y,
                    Mathf.Clamp(waypoint.StartPos.z + up * 2, pipestartPos.z + moveMinCount * 2, pipestartPos.z + moveMaxCount * 2));
                waypoint.StartPos = newPosStart;

                Vector3 newPosEnd = new Vector3(pipefinishPos.x, pipefinishPos.y,
                    Mathf.Clamp(waypoint.EndPos.z + up * 2, pipefinishPos.z + moveMinCount * 2, pipefinishPos.z + moveMaxCount * 2));
                waypoint.EndPos = newPosEnd;
            }
        }

        if (isMoveXpos) {
            BoxToMove.x = Mathf.Clamp(BoxToMove.x + up * (2), (int)(startPos.x), (int)(finishPos.x));
        }
        else {
            BoxToMove.z = Mathf.Clamp(BoxToMove.z + up * (2), (int)(startPos.z), (int)(finishPos.z));
        }
    }
}
