using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertMode_MoveTile : ConvertMode {
    private string[] parentName = { "BombSpawn", "MoveSwitch", "MagicStone", "Tile", "Bomb", "Biscuit"};

    protected override void Start() {
        InitParentObjectWithTag(ConvertItem.MoveTile);

        ChangeLayerAllActiveTrue();
    }

    protected override void InitParentObjectWithTag(ConvertItem tagName) {
        parentObject = GameObject.FindGameObjectsWithTag($"{tagName}");

        foreach (GameObject each in parentObject) {
            Renderer[] allChildRenderers = each.GetComponentsInChildren<Renderer>();

            foreach (Renderer item in allChildRenderers) {
                GameObject parentObject = item.transform.parent?.gameObject; // null 조건부 연산자 사용
                if (parentObject != null) {

                    if (!parentObject.name.Contains("3D")) {
                        if (!AllObjects.Contains(parentObject)) {
                            AllObjects.Add(parentObject);
                        }
                    }
                    else {
                        if (!AllObjects.Contains(parentObject.transform.parent.gameObject)) {
                            AllObjects.Add(parentObject.transform.parent.gameObject);
                        }
                    }

                }
            }
        }
    }

    // 부모 오브젝트의 이름에 따라 추가 여부를 결정하는 메서드
    private bool ShouldAddToAllObjects(GameObject parentObject) {
        string[] keywords = { "Tile", "Bomb", "BombSpawn", "Object", "MoveSwitch" };

        foreach (string keyword in keywords) {
            if (parentObject.name.Contains(keyword)) {
                return true;
            }
        }

        return false;
    }

    public override void ChangeLayerActiveFalseInSelectObjects() {
        foreach (GameObject item in AllObjects) {
            if (!SelectObjects.Contains(item)) {
                item.layer = activeFalseLayerIndex;

                if (HasMatchName(item)) {
                    ChangeLayerActiveWithAllChild(item.transform, activeFalseLayerIndex);
                }
                else {
                    // 부모 이름이 일치하지 않을 경우 하위 객체의 레이어 변경
                    foreach (Transform child in item.transform) {
                        child.gameObject.layer = activeFalseLayerIndex;
                    }
                }

            }
        }
    }

    //해당하는 이름들은 하위 오브젝트까지 돌아서 Layer 수정
    private bool HasMatchName(GameObject item) {
        // 하위 객체의 레이어 변경 => Root3D가 안보여야함
        foreach (string name in parentName) {
            if (item.name.Contains(name)) {
                return true;
            }
        }
        return false;
    }

    public override void ChangeLayerAllActiveTrue() {
        foreach (GameObject each in AllObjects) {

            each.layer = activeTrueLayerIndex;

            if (each.name.Contains("BombSpawn") || each.name.Contains("MoveSwitch")) {
                ChangeLayerActiveWithAllChild(each.transform, activeTrueLayerIndex);
            }
            else {
                // 하위 객체의 레이어 변경 => Ground로 바꿔서 지나갈 수 있어야함
                foreach (Transform child in each.transform) {
                    child.gameObject.layer = groundLayerIndex;
                }
            }
        }
    }

    public override void AddSelectObjects(GameObject selectCheck) {
        if (!SelectObjects.Contains(selectCheck)) {
            SelectObjects.Add(selectCheck);
        }
    }
}

/*
 - 해당 하는 오브젝트
1. moving_object
 - bomb
 - bomb spawner
 - magic stone
 - biscuit
 
 */