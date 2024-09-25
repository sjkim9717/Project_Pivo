using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateManage : MonoBehaviour {
    public GateData[] GateDatas;
    private List<GameObject> door = new List<GameObject>();

    private void Awake() {
        int doorCounter = 1;  // 문 번호를 추적하기 위한 카운터
        foreach (GateData gate in GateDatas) {
            Debug.Log("GateData " + ": " + gate.DoorData.name);

            GameObject gateParent = new GameObject("door_" + doorCounter++);
            gateParent.transform.SetParent(transform);

            GameObject _door = Instantiate(gate.DoorData.DoorPrefab, gate.DoorPosition, Quaternion.identity, gateParent.transform);
            _door.GetComponent<Door>().SetColor(gate.IsBlueColor);
            _door.GetComponent<Door>().SetPassword(gate.DoorData.Password);
            _door.GetComponent<Door>().SetRequireKeyNum(gate.RequreKeyNum);
            door.Add(_door);

            for (int i = 0; i < gate.RequreKeyNum; i++) {
                Quaternion keyRotation = Quaternion.Euler(gate.KeyRotation[i]);
                GameObject key =  Instantiate(gate.KeyData.KeyPrefab, gate.KeyPosition[i], keyRotation, gateParent.transform);
                for (int j = 0; j < 2; j++) {
                    key.GetComponentsInChildren<Key>()[j].SetColor(gate.IsBlueColor);
                    key.GetComponentsInChildren<Key>()[j].SetPassword(gate.KeyData.Password);
                }
            }
        }
    }

    public void FindDoor(int keyPassword) {
        for (int i = 0; i < door.Count; i++) {
            Door _doorComponent = door[i].GetComponent<Door>();
            int passwordcheck = _doorComponent.GetPassword();
            if (passwordcheck == keyPassword) {
                Debug.Log(passwordcheck);
                _doorComponent.CheckKeyCount();
                return;
            }
        }

    }


}
