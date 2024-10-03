using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonsterBase {

    protected override void Awake() {
        base.Awake();
    }

    private void Start() {
        Change3D();
        // 모드 변경
        playerManage.IsSwitchMode += SwitchMode;
        playerManage.IsSwitchMode += ChangeEmotion;
        //TODO: IMonsterStateBase 초기값 지정
    }

    public void SwitchMode() {
        SettingEffectActiveTrue();               // effect

        if (playerManage.CurrentMode == PlayerMode.Player3D) {
            Change3D();
        }
        else if (playerManage.CurrentMode == PlayerMode.Player2D) {
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
        float min = Mathf.Min(playerManage.StartSection.z, playerManage.FinishSection.z);
        float max = Mathf.Max(playerManage.StartSection.z, playerManage.FinishSection.z);

        float monsterPos = Monster3D.transform.position.z;

        if (monsterPos >=min && monsterPos <= max) return true;
        else return false;
    }

    // 선택된 범위 안일경우 z축범위 앞으로 아무것도없는지
    // monster 3D 오브젝트에서 z축 -방향으로 raycast를 쏴서 본인이 아니고 무언가 있다면 false
    private bool IsEmptyOnTheZAxis() {
        float layDistance = (playerManage.StartSection.z >= playerManage.FinishSection.z) ?
             (Monster3D.transform.position.z - playerManage.FinishSection.z) :
             (Monster3D.transform.position.z - playerManage.StartSection.z);

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
                if (each.transform.parent.name.Contains("Tile") || each.transform.parent.name.Contains("Box")) {
                    //Debug.LogWarning("Z축에 오브젝트가 있음 | each.name | " + each.transform.parent.name);
                    return false;
                }
            }
        }
        return true;
    }

    private void ChangeEmotion() {
        if(monster2D.layer == activeFalseLayerIndex || monster3D.layer== activeFalseLayerIndex) {
            foreach (Transform item in emotion.transform) {
                item.gameObject.SetActive(false);
            }
        }
    }


    private void OnDrawGizmos() {
        if (Monster3D == null || playerManage == null) return;

        Vector3 origin = new Vector3(Monster3D.transform.position.x, Monster3D.transform.position.y + 1f, Monster3D.transform.position.z);

        float layDistance = (playerManage.StartSection.z >= playerManage.FinishSection.z) ?
            (Monster3D.transform.position.z - playerManage.FinishSection.z) :
            (Monster3D.transform.position.z - playerManage.StartSection.z);

        // Z축 -방향으로 layDistance만큼의 Ray를 그려줍니다
        Debug.DrawRay(origin, -transform.forward * layDistance, Color.black);
    }
}

