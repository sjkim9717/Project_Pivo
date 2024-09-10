using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBox : MonoBehaviour, IPushBox {
    [SerializeField] private float moveSpeed = 5f;
    private bool isMoveXpos;

    private List<GameObject> tileObject = new List<GameObject>();
    private GameObject pushBox;

    private Vector3 startPos = Vector3.zero;
    private Vector3 finishPos = Vector3.zero;
    private Vector3 BoxToMove;

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

        if (isMoveXpos) {
            BoxToMove.x = Mathf.Clamp(BoxToMove.x + up * (2), startPos.x - 0.2f, finishPos.x + 0.2f);
        }
        else {
            BoxToMove.z = Mathf.Clamp(BoxToMove.z + up * (2), startPos.z - 0.2f, finishPos.z + 0.2f);
        }
    }
}
