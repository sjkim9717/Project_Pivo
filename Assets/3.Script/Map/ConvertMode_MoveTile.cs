using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertMode_MoveTile : ConvertMode {
    private string[] parentName = { "BombSpawn", "MoveSwitch", "MagicStone"};

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

                    if (parentObject.name.Contains("Models")) {
                        // magic ston
                        GameObject findMagic = parentObject.transform.parent?.parent?.gameObject; // null 조건부 연산자 사용
                        if (!AllObjects.Contains(findMagic)) {
                            AllObjects.Add(findMagic);
                        }
                    }
                    else if (!parentObject.name.Contains("3D")) {
                        // 일반 오브젝트 - tile, object 등
                        if (!AllObjects.Contains(parentObject)) {
                            AllObjects.Add(parentObject);
                        }
                    }
                    else {
                        // move switch
                        if (!AllObjects.Contains(parentObject.transform.parent.gameObject)) {
                            AllObjects.Add(parentObject.transform.parent.gameObject);
                        }
                    }

                }
            }
        }
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

            if (HasMatchName(each)) {
                ChangeLayerActiveWithAllChild(each.transform, activeTrueLayerIndex);
            }
            else {
                // 하위 객체의 레이어 변경 => tile만 Ground로 바꿔서 지나갈 수 있어야함
                if (each.name.Contains("Tile")) {
                    foreach (Transform child in each.transform) {
                        child.gameObject.layer = groundLayerIndex;
                    }

                }
                else { // 그 외 그냥 보이기만 하면됨
                    ChangeLayerActiveWithAllChild(each.transform, activeTrueLayerIndex);
                }
            }
        }
    }

    public override void AddSelectObjects(GameObject selectCheck) {

        Transform parent = selectCheck.transform.parent;
        if (parent.name.Contains("Models")) {
            // magic ston
            GameObject findMagic = parent.transform.parent?.parent?.gameObject; // null 조건부 연산자 사용
            if (!SelectObjects.Contains(findMagic)) {
                SelectObjects.Add(findMagic);
            }
        }
        else if (!parent.name.Contains("3D")) {
            // 일반 오브젝트 - tile, object 등
            if (!SelectObjects.Contains(parent.gameObject)) {
                SelectObjects.Add(parent.gameObject);
            }
        }
        else {
            // move switch
            if (!SelectObjects.Contains(parent.transform.parent.gameObject)) {
                SelectObjects.Add(parent.transform.parent.gameObject);
            }
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