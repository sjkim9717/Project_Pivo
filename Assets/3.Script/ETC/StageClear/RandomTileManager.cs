using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class RandomTileManager : MonoBehaviour {
    [SerializeField] private float movingSpeed = 0.1f;                              // y좌표를 0으로 이동시킬 시간
    private Transform randomTileParent;
    [SerializeField]private List<GameObject> tileGroups = new List<GameObject>();

    private void Awake() {
        randomTileParent = transform.GetChild(1);

        foreach (Transform child in randomTileParent) {
            tileGroups.Add(child.gameObject);
        }

        // 이름에서 숫자를 추출하여 정렬
        tileGroups.Sort((x, y) => GetNumberFromName(x.name).CompareTo(GetNumberFromName(y.name)));
    }

    // palyable director - random tile 활성화 타임 + signal 추가해서 메소드 연결
    public void MoveRandomTile() {
        StartCoroutine(MoveTilesSequentially());
    }


    private IEnumerator MoveTilesSequentially() {
        foreach (var tileGroup in tileGroups) {
            yield return StartCoroutine(MoveTileToZeroY(tileGroup));
        }
    }

    public IEnumerator MoveTileToZeroY(GameObject tileGroup) {
        float elapsedTime = 0f;
        Vector3 startPos = tileGroup.transform.localPosition; // 시작 위치

        while (elapsedTime < movingSpeed) {
            elapsedTime += Time.deltaTime;
            float newY = Mathf.Lerp(startPos.y, 0, elapsedTime / movingSpeed);
            tileGroup.transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
            yield return null;
        }
        tileGroup.transform.localPosition = new Vector3(startPos.x, 0, startPos.z);
    }



    // 이름에서 숫자를 추출하는 함수
    private int GetNumberFromName(string name) {
        // "TileGroup (23)" 같은 이름에서 마지막 괄호 안의 숫자를 인덱스로 찾음
        int startIndex = name.IndexOf('(');
        int endIndex = name.IndexOf(')');

        if (startIndex != -1 && endIndex != -1) {
            string numberStr = name.Substring(startIndex + 1, endIndex - startIndex - 1);
            if (int.TryParse(numberStr, out int result)) {
                return result;
            }
        }

        return 0; // 괄호 안의 숫자가 없거나 파싱에 실패하면 0 반환
    }

}

/*
 1. 목적: 활성화 되었을 경우에만 1번 실행 -> 하위 객체들을 번호 순으로 y값을 0으로 만들어서 올릴것
 
 */
