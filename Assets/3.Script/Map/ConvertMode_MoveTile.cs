using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertMode_MoveTile : ConvertMode {
    private string[] parentName = { "BombSpawn", "Bomb", "MoveSwitch", "MagicStone"};

    protected override void Start() {
        InitParentObjectWithTag(ConvertItem.MoveTile);

        ChangeLayerAllActiveTrue();
    }

    protected override void InitParentObjectWithTag(ConvertItem tagName) {
        parentObject = GameObject.FindGameObjectsWithTag($"{tagName}");
        for (int i = 0; i < parentObject.Length; i++) {
            parentObject[i] = GameObject.FindGameObjectsWithTag($"{tagName}")[i];
        }

        foreach (GameObject each in parentObject) {
            Renderer[] allChildRenderers = each.GetComponentsInChildren<Renderer>();

            foreach (Renderer item in allChildRenderers) {
                GameObject parentObject = item.transform.parent?.gameObject; // null 조건부 연산자 사용
                if (parentObject != null) {

                    if (parentObject.name.Contains("Models")) {
                        // magic ston
                        GameObject findMagic = parentObject.transform.parent?.parent?.gameObject; // null 조건부 연산자 사용
                        AddListIfNotSelected(AllObjects, findMagic);
                    }
                    else if (!parentObject.name.Contains("3D")) {
                        // 일반 오브젝트 - tile, object 등 + Bombspawner
                        AddListIfNotSelected(AllObjects, parentObject);

                        if (parentObject.name.Contains("BombSpawn")) {      // Bomb이 비활성화라 따로 담아야함
                            // BombSpawner일 경우 Bomb 추가
                            GameObject bomb = parentObject.transform.GetChild(2).gameObject;
                            AddListIfNotSelected(AllObjects, bomb);
                        }
                    }
                    else {
                        GameObject mainObject = parentObject.transform.parent.gameObject;
                        // move switch
                        AddListIfNotSelected(AllObjects, mainObject);
                    }

                }
            }
        }
    }

    public override void ChangeLayerActiveFalseInSelectObjects() {
        foreach (GameObject item in AllObjects) {
            if (!SelectObjects.Contains(item)) {
                item.layer = activeFalseLayerIndex;

                if (HasMatchName(item, parentName)) {
                    if (item.name.Contains("BombSpawn")) {              // Bomb Spawner는 bomb만 빼고 
                        for (int i = 0; i < item.transform.childCount - 1; i++) {
                            ChangeLayerActiveWithAllChild(item.transform.GetChild(i), activeFalseLayerIndex);
                        }
                    }
                    else {
                        ChangeLayerActiveWithAllChild(item.transform, activeFalseLayerIndex);
                    }
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

    public override void ChangeLayerAllActiveTrue() {
        foreach (GameObject each in AllObjects) {

            each.layer = activeTrueLayerIndex;

            if (HasMatchName(each, parentName)) {
                if (each.name.Contains("BombSpawn")) {              // Bomb Spawner는 bomb만 빼고 바꿈
                    for (int i = 0; i < each.transform.childCount -1; i++) {
                        ChangeLayerActiveWithAllChild(each.transform.GetChild(i), activeTrueLayerIndex);
                    }
                }
                else {
                    ChangeLayerActiveWithAllChild(each.transform, activeTrueLayerIndex);
                }
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

    // 선택된 오브젝트 리스트에 담음
    public override void AddSelectObjects(GameObject selectCheck) {
        if (selectCheck.name.Contains("Bomb")) {
            AddListIfNotSelected(SelectObjects, selectCheck);
        }
        else {
            Transform parent = selectCheck.transform.parent;
            if (parent.name.Contains("Models")) {
                // magic ston
                GameObject findMagic = parent.transform.parent?.parent?.gameObject; // null 조건부 연산자 사용
                AddListIfNotSelected(SelectObjects, findMagic);
            }
            else if (!parent.name.Contains("3D")) {
                // 일반 오브젝트 - tile, object 등
                AddListIfNotSelected(SelectObjects, parent.gameObject);
            }
            else {
                // move switch
                AddListIfNotSelected(SelectObjects, parent.transform.parent.gameObject);
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