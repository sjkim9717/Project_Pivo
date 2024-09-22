using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonsterBase {
    private static MonsterManager _instance;
    public static MonsterManager instance { get { return _instance; } }

    protected override void Awake() {
        if (_instance == null) {
            _instance = this;
        }
        else {
            Destroy(gameObject);
        }
        base.Awake();
    }

    private void Start() {
        Change3D();
        //TODO: 모드 변경
        PlayerManage.instance.IsSwitchMode += SwitchMode;
        //TODO: IMonsterStateBase 초기값 지정
    }

    public void SwitchMode() {
        //TODO: effect
        Effect.SetActive(true);
        if (PlayerManage.instance.CurrentMode == PlayerMode.Player3D) {
            Change3D();
        }
        else if (PlayerManage.instance.CurrentMode == PlayerMode.Player2D) {
            if (IsInSelectArea()) {
                if (IsEmptyOnTheZAxis()) {
                    Change2D();
                }
                else {
                    ChangeAutoMode();
                }
            }
            else {
                ChangeAutoMode();
            }
        }
    }

    // 플레이어가 선택한 범위 안에 있는지
    private bool IsInSelectArea() {
        if (PlayerManage.instance.StartSection.z >= PlayerManage.instance.FinishSection.z) {
            if (Monster3D.transform.position.z <= PlayerManage.instance.StartSection.z && Monster3D.transform.position.z >= PlayerManage.instance.FinishSection.z) return true;
            else return false;
        }
        else {
            if (Monster3D.transform.position.z >= PlayerManage.instance.StartSection.z && Monster3D.transform.position.z <= PlayerManage.instance.FinishSection.z) return true;
            else return false;
        }
    }

    // 선택된 범위 안일경우 z축범위 앞으로 아무것도없는지
    private bool IsEmptyOnTheZAxis() {
        // monster 3D 오브젝트에서 z축 -방향으로 raycast를 쏴서 본인이 아니고 무언가 있다면 false
        float layDistance = (PlayerManage.instance.StartSection.z >= PlayerManage.instance.FinishSection.z) ?
             (Monster3D.transform.position.z - PlayerManage.instance.FinishSection.z) :
             (Monster3D.transform.position.z - PlayerManage.instance.StartSection.z);

        Vector3 origin = new Vector3(Monster3D.transform.position.x, Monster3D.transform.position.y + 1f, Monster3D.transform.position.z);

        RaycastHit[] hits = Physics.RaycastAll(origin, -transform.forward, layDistance);
        List<GameObject> ZAxisObject = new List<GameObject>();

        foreach (RaycastHit hit in hits) {
            if (hit.collider.gameObject != Monster3D) {
                ZAxisObject.Add(hit.collider.gameObject);
            }
        }

        foreach (GameObject each in ZAxisObject) {
            if (each.transform.position.z < Monster3D.transform.position.z) {
                //Debug.LogWarning("Z축에 오브젝트가 있음 | each.name | " + each.transform.parent.name);
                return false;
            }
        }
        return true;
    }

    private void OnDrawGizmos() {
        if (Monster3D == null || PlayerManage.instance == null) return;

        Vector3 origin = new Vector3(Monster3D.transform.position.x, Monster3D.transform.position.y + 1f, Monster3D.transform.position.z);

        float layDistance = (PlayerManage.instance.StartSection.z >= PlayerManage.instance.FinishSection.z) ?
            (Monster3D.transform.position.z - PlayerManage.instance.FinishSection.z) :
            (Monster3D.transform.position.z - PlayerManage.instance.StartSection.z);

        // Z축 -방향으로 layDistance만큼의 Ray를 그려줍니다
        Debug.DrawRay(origin, -transform.forward * layDistance, Color.black);
    }
}

