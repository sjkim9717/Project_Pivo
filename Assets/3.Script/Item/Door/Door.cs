using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    private float moveSpeed = 3.5f;
    private float journeyLength;
    private float startTime;

    [SerializeField] private float moveYpos = 7f;

    [SerializeField] private int password;
    [SerializeField] private int checkCount;
    [SerializeField] private int requireKeyNum;

    private bool isDoorNeddMove;
    private bool isBlueColor;

    public int GetPassword() { return password; }
    public void SetPassword(int _password) { password = _password; }
    public void SetColor(bool _isBlueColor) { isBlueColor = _isBlueColor; }
    public void SetRequireKeyNum(int _requireKeyNum) { requireKeyNum = _requireKeyNum; }

    private Vector3 originPos;
    private Vector3 posToMove;
    [SerializeField] private GameObject activeDoor;
    [SerializeField] private List<GameObject> activeDoorKeys = new List<GameObject>();

    private PlayerManage playerManage;


    private void Awake() {
        playerManage = FindObjectOfType<PlayerManage>();

        activeDoor = transform.Find("Root3D/Activate_Door/Activate_Door_Door").gameObject;

        Traverse(transform);

        originPos = activeDoor.transform.position;
        posToMove = new Vector3(activeDoor.transform.position.x, activeDoor.transform.position.y - moveYpos, activeDoor.transform.position.z);
        journeyLength = Vector3.Distance(originPos, posToMove);
    }

    private void Traverse(Transform parent) {
        foreach (Transform child in parent) {
            if (child.name.Contains("Activate_Door_Key")) {
                activeDoorKeys.Add(child.gameObject);
            }

            Traverse(child);
        }
    }


    private void Start() {
        HideKeyInRandom(requireKeyNum);
        SettingColor(isBlueColor);
    }

    private void Update() {
        if (playerManage.CurrentMode == PlayerMode.Player3D) {
            if (isDoorNeddMove) {
                // 현재 시간과 여정을 계산
                float distanceCovered = (Time.time - startTime) * moveSpeed;
                float fractionOfJourney = distanceCovered / journeyLength;

                // 문 위치를 보간하여 이동
                activeDoor.transform.position = Vector3.Lerp(originPos, posToMove, fractionOfJourney);

                // 이동 완료 체크
                if (fractionOfJourney >= 1f) {
                    activeDoor.transform.position = posToMove; // 최종 위치 설정
                    isDoorNeddMove = false; // 이동 완료 후 플래그 리셋
                }
            }
        }
    }

    public void CheckKeyCount() {
        checkCount++;
        // 문 filling 할수있으면 할 것
        // 키가 하나씩 생겨야함
        ShowKey();

        if (checkCount >= requireKeyNum) {
            // 문이 내려가야함
            isDoorNeddMove = true;
            startTime = Time.time;
            /*
            if (checkCount > requireKeyNum) {
                Debug.LogWarning("확인된 키가 필요한 키의 갯수보다 많음");
            }
            else if (checkCount == requireKeyNum) {
                Debug.LogWarning("확인된 키가 필요한 키의 개수와 동일");
            }
            */
        }
        else {
            //Debug.LogWarning("확인된 키가 필요한 키의 개수보다 부족함 | " + requireKeyNum + " | " + checkCount);
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
            activeDoor.transform.position = Vector3.Lerp(originPos, posToMove, fractionOfJourney);

            yield return null; // 다음 프레임까지 대기
        }
        activeDoor.transform.position = posToMove;

    }

    private void HideKeyInRandom(int keysToHide) {

        int randomNumber = Random.Range(0, activeDoorKeys.Count);
        int keysHidden = 0;

        while (keysHidden < keysToHide) {
            int indexToHide = (randomNumber + keysHidden) % activeDoorKeys.Count;
            activeDoorKeys[indexToHide].SetActive(false);
            keysHidden++;
        }
    }

    private void ShowKey() {
        for (int i = 0; i < activeDoorKeys.Count; i++) {
            if (!activeDoorKeys[i].activeSelf) {
                activeDoorKeys[i].SetActive(true);
                return;
            }
        }
    }
    
    public void SettingColor(bool isblueColor){

        Color targetColor = isblueColor ? Color.cyan : Color.red;

        foreach (GameObject each in activeDoorKeys) {    
            Renderer[] renderers = each.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers) {
                if (renderer.gameObject != each) {
                    renderer.material.color = targetColor;
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