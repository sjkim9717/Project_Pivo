using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GateData", menuName = "ScriptableObjects/GateData", order = 1)]
public class GateData : ScriptableObject {

    [Space(5)]
    [Header("Scene Stage")]
    public StageLevel StageLevel;
    public bool IsBlueColor;

    [Space(5)]
    [Header("Door Info")]
    public DoorData DoorData;
    public int RequreKeyNum;
    public Vector3 DoorPosition;

    [Space(5)]
    [Header("Key Info")]
    public KeyData KeyData;
    public List<Vector3> KeyPosition;
    public List<Vector3> KeyRotation;

    // 에디터에서 값이 변경될 때 호출되는 메서드
    private void OnValidate() {


        // KeyPosition과 KeyRotation 리스트 초기화
        if (KeyPosition == null) {
            KeyPosition = new List<Vector3>();
        }
        if (KeyRotation == null) {
            KeyRotation = new List<Vector3>();
        }

        // TotalKeyNum에 맞게 KeyPosition의 크기를 맞춤
        if (RequreKeyNum > KeyPosition.Count) {
            // 부족한 만큼 빈 위치를 추가
            for (int i = KeyPosition.Count; i < RequreKeyNum; i++) {
                KeyPosition.Add(Vector3.zero);  // 기본값은 Vector3.zero로 설정
                KeyRotation.Add(Vector3.zero);
            }
        }
        else if (RequreKeyNum < KeyPosition.Count) {
            // 남는 위치를 제거
            KeyPosition.RemoveRange(RequreKeyNum, KeyPosition.Count - RequreKeyNum);
            KeyRotation.RemoveRange(RequreKeyNum, KeyRotation.Count - RequreKeyNum);
        }
    }
}
