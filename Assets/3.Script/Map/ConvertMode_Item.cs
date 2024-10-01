using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertMode_Item : ConvertMode {

    protected override void Start() {
        InitParentObjectWithTag(ConvertItem.Objects_2);

        ChangeLayerAllActiveTrue();
    }
    protected override void InitParentObjectWithTag(ConvertItem tagName) {

        parentObject = new GameObject[GameObject.FindGameObjectsWithTag($"{tagName}").Length];
        for (int i = 0; i < parentObject.Length; i++) {
            parentObject[i] = GameObject.FindGameObjectsWithTag($"{tagName}")[i];
        }

        foreach (GameObject parent in parentObject) {
            Collider[] findRoot3D = parent.transform.GetComponentsInChildren<Collider>();

            foreach (Collider eachRoot3D in findRoot3D) {
                if (eachRoot3D.name.Contains("Root3D")) {
                    GameObject parentObj = eachRoot3D.transform.parent.gameObject;
                    if (parentObj.CompareTag("PushBox") || parentObj.CompareTag("Climb")|| parentObj.name.Contains("Pipe")) {
                        GameObject pushbox = parentObj.transform.parent.gameObject;
                        if (pushbox.CompareTag("Untagged")) {
                            AddListIfNotSelected(AllObjects, parentObj);
                        }
                        else {
                            AddListIfNotSelected(AllObjects, pushbox);
                        }
                    }
                    else {
                        AddListIfNotSelected(AllObjects, parentObj);

                        if (parentObj.name.Contains("BombSpawn")) {    // Bomb이 비활성화라 따로 담아야함
                            // BombSpawner일 경우 Bomb 추가
                            GameObject bomb = parentObj.transform.GetChild(2).gameObject;
                            AddListIfNotSelected(AllObjects, bomb);
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

                if (item.name.Contains("BombSpawn")) {              // Bomb Spawner는 bomb만 빼고 
                    for (int i = 0; i < item.transform.childCount - 1; i++) {
                        ChangeLayerActiveWithAllChild(item.transform.GetChild(i), activeFalseLayerIndex);
                    }
                }
                else {
                    // 하위 객체의 레이어 변경 => Root3D가 안보여야함
                    ChangeLayerActiveWithAllChild(item.transform, activeFalseLayerIndex);
                }
            }
        }
    }

    public override void ChangeLayerActiveTrueWhen3DModeCancle() {
        foreach (GameObject item in AllObjects) {
            if (SelectObjects.Contains(item)) {
                item.layer = activeTrueLayerIndex;

                if (item.name.Contains("BombSpawn")) {              // Bomb Spawner는 bomb만 빼고 바꿈
                    for (int i = 0; i < item.transform.childCount - 1; i++) {
                        ChangeLayerActiveWithAllChild(item.transform.GetChild(i), activeTrueLayerIndex);
                    }
                }
                else {
                    ChangeLayerActiveWithAllChild(item.transform, activeTrueLayerIndex);
                }
            }
            else {
                item.layer = activeFalseLayerIndex;

                if (item.name.Contains("BombSpawn")) {              // Bomb Spawner는 bomb만 빼고 
                    for (int i = 0; i < item.transform.childCount - 1; i++) {
                        ChangeLayerActiveWithAllChild(item.transform.GetChild(i), activeFalseLayerIndex);
                    }
                }
                else {
                    // 하위 객체의 레이어 변경 => Root3D가 안보여야함
                    ChangeLayerActiveWithAllChild(item.transform, activeFalseLayerIndex);
                }
            }
        }
    }

    public override void ChangeLayerAllActiveTrue() {
        foreach (GameObject item in AllObjects) {
            item.layer = activeTrueLayerIndex;

            if (item.name.Contains("BombSpawn")) {              // Bomb Spawner는 bomb만 빼고 바꿈
                for (int i = 0; i < item.transform.childCount - 1; i++) {
                    ChangeLayerActiveWithAllChild(item.transform.GetChild(i), activeTrueLayerIndex);
                }
            }
            else {
                ChangeLayerActiveWithAllChild(item.transform, activeTrueLayerIndex);
            }
        }
    }

    public override void AddSelectObjects(GameObject selectCheck) {
        if (selectCheck.name.Contains("Bomb")) {
            AddListIfNotSelected(SelectObjects, selectCheck);
        }
        else {
            Transform parent = selectCheck.transform.parent;
            if (parent.name.Contains("Group")) {
                AddListIfNotSelected(SelectObjects, selectCheck);
            }
            else {
                // 부모의 두 번째 레벨까지 확인
                Transform targetTransform = parent?.parent != null
                    && parent.parent.CompareTag("PushBox") ? parent.parent : parent;
                AddListIfNotSelected(SelectObjects, targetTransform.gameObject);
            }
        }
    }

    public override void AddBlockObject(GameObject blockCheck) {
        if (blockCheck.name.Contains("Box")) {
            blockObjects.Add(blockCheck);

            MeshRenderer tileRenderer = blockCheck.GetComponentInChildren<MeshRenderer>();
            defaltMaterial.Add(tileRenderer.materials[0]);
        }
    }
}

