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
                            if (!AllObjects.Contains(parentObj)) {
                                AllObjects.Add(parentObj);
                            }
                        }
                        else {
                            if (!AllObjects.Contains(pushbox)) {
                                AllObjects.Add(pushbox);
                            }
                        }
                    }
                    else {
                        if (!AllObjects.Contains(parentObj)) {
                            AllObjects.Add(parentObj);
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

                // 하위 객체의 레이어 변경 => Root3D가 안보여야함
                ChangeLayerActiveWithAllChild(item.transform, activeFalseLayerIndex);

            }
        }
    }

    public override void ChangeLayerAllActiveTrue() {
        foreach (GameObject item in AllObjects) {
            item.layer = activeTrueLayerIndex;

            // 하위 객체의 레이어 변경 
            ChangeLayerActiveWithAllChild(item.transform, activeTrueLayerIndex);
        }
    }

    public override void AddSelectObjects(GameObject selectCheck) {
        if (!SelectObjects.Contains(selectCheck)) {
            SelectObjects.Add(selectCheck);
        }

    }
}

