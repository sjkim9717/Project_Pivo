using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    [SerializeField] private float moveYpos = 7f;
    private float moveSpeed = 2f;
    private int password;
    private int checkCount;
    private int requireKeyNum;
    public int GetPassword() { return password; }
    public void SetPassword(int _password) { password = _password; }
    public void SetRequireKeyNum(int _requireKeyNum) { requireKeyNum = _requireKeyNum; }

    private Vector3 originPos;
    private Vector3 posToMove;
    private GameObject activeDoor;
    private List<GameObject> activeDoorKeys = new List<GameObject>();

    private HashSet<int> hideKeyIndex = new HashSet<int>();

    private void Awake() {
        activeDoor = transform.Find("Root3D/Activate_Door/Activate_Door_Door").gameObject;
        foreach (Transform child in transform) {
            if (child.Find("Activate_Door_Key")) {
                activeDoorKeys.Add(child.gameObject);
            }
        }

        HideKeyInRandom();

        originPos = activeDoor.transform.position;
        posToMove = new Vector3(activeDoor.transform.position.x, activeDoor.transform.position.y - moveYpos, activeDoor.transform.position.z);
    }

    public void CheckKeyCount() {
        checkCount++;
        if (checkCount > requireKeyNum) {
            Debug.LogWarning("확인된 키가 필요한 키의 갯수보다 많음");
        }
        else if (checkCount == requireKeyNum) {
            Debug.LogWarning("확인된 키가 필요한 키의 개수와 동일");
            // 문이 내려가야함
            StartCoroutine(MoveActiveDoorWhenKeyFound());
        }
        else {
            Debug.LogWarning("확인된 키가 필요한 키의 개수보다 부족함 | " + requireKeyNum + " | " + checkCount);
            // 문 filling 할수있으면 할 것
            // 키가 하나씩 생겨야함
            ShowKey();
        }
    }

    private IEnumerator MoveActiveDoorWhenKeyFound() {

        // 이동 시간 계산
        float journeyLength = Vector3.Distance(originPos, posToMove);
        float elapsedTime = 0f;

        // 이동 시작
        while (elapsedTime < journeyLength / moveSpeed) {
            elapsedTime += Time.deltaTime;
            float fractionOfJourney = elapsedTime * moveSpeed / journeyLength;
            activeDoor.transform.position = Vector3.Slerp(originPos, posToMove, fractionOfJourney);

            yield return null; // 다음 프레임까지 대기
        }
        activeDoor.transform.position = posToMove;

    }

    private void HideKeyInRandom() {
        while (hideKeyIndex.Count < 2) {
            int randomNumber = Random.Range(0, activeDoorKeys.Count);
            hideKeyIndex.Add(randomNumber);
        }

        for (int i = 0; i < activeDoorKeys.Count; i++) {
            if (hideKeyIndex.Contains(i)) {
                activeDoorKeys[i].SetActive(false);
            }
        }
    }

    private void ShowKey() {
        for (int i = 0; i < activeDoorKeys.Count; i++) {
            if (hideKeyIndex.Contains(i)) {
                if (!activeDoorKeys[i].activeSelf) {
                    activeDoorKeys[i].SetActive(true);
                    return;
                }
            }
        }
    }

}

/*
 1. 매니저에서 신호를 받고 
 2. 키의 갯수를 확인하고 
 3. 문이 열리는 애니메이션 실행하기 
 */