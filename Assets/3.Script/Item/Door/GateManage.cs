using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateManage : MonoBehaviour {
    public GateData[] GateDatas;
    private List<GameObject> door = new List<GameObject>();

    private void Awake() {
        int doorCounter = 1;  // 문 번호를 추적하기 위한 카운터
        foreach (GateData gate in GateDatas) {

            GameObject gateParent = new GameObject("door_" + doorCounter++);
            gateParent.transform.SetParent(transform);

            GameObject _door = Instantiate(gate.DoorData.DoorPrefab, gate.DoorPosition, Quaternion.identity, gateParent.transform);
            _door.GetComponent<Door>().SetPassword(gate.DoorData.Password);
            _door.GetComponent<Door>().SetRequireKeyNum(gate.RequreKeyNum);
            door.Add(_door);

            for (int i = 0; i < gate.RequreKeyNum; i++) {
                Quaternion keyRotation = Quaternion.Euler(gate.KeyRotation[i]);
                GameObject key =  Instantiate(gate.KeyData.KeyPrefab, gate.KeyPosition[i], keyRotation, gateParent.transform);
                key.GetComponentInChildren<Key>().SetPassword(gate.KeyData.Password);
            }
        }
    }

    public void FindDoor(int keyPassword) {
        foreach (GameObject _door in door) {
            Door _doorComponent = _door.GetComponent<Door>();
            int passwordcheck = _doorComponent.GetPassword();
            if (passwordcheck == keyPassword) {
                _doorComponent.CheckKeyCount();
                return;
            }
        }
    }


}
