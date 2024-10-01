using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ConvertMode_Object : ConvertMode {
    private string[] parentName = { "Cube"};
    private string[] exception = { "start", "end", "text"};


    protected override void Start() {
        InitParentObjectWithTag(ConvertItem.Objects);

        ChangeLayerAllActiveTrue();
    }

    protected override void InitParentObjectWithTag(ConvertItem tagName) {
        parentObject = GameObject.FindGameObjectsWithTag($"{tagName}");

        foreach (GameObject each in parentObject) {
            Renderer[] allChildRenderers = each.GetComponentsInChildren<Renderer>();

            foreach (Renderer item in allChildRenderers) {
                GameObject parentObject = item.transform.parent?.gameObject; // null 조건부 연산자 사용
                if (parentObject != null) {
                    if (!HasExceptionName(parentObject)) {
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
    }

    private bool HasExceptionName(GameObject item) {
        // 하위 객체의 레이어 변경 => Root3D가 안보여야함
        string itemNameLower = item.name.ToLower(); // 소문자로 변환

        foreach (string name in exception) {
            if (itemNameLower.Contains(name.ToLower())) { // 소문자로 변환하여 비교
                return true;
            }
        }
        return false;
    }

    public void DeleteDestroiedObject(GameObject deleteObject) {
        if (AllObjects.Contains(deleteObject)) {
            AllObjects.Remove(deleteObject);
        }
    }
    public override void ChangeLayerActiveFalseInSelectObjects() {
        foreach (GameObject item in AllObjects) {
            if (!SelectObjects.Contains(item)) {

                item.layer = activeFalseLayerIndex;

                if (HasMatchName(item, parentName)) {
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
    public override void ChangeLayerActiveTrueWhen3DModeCancle() {
        foreach (GameObject each in AllObjects) {
            if (SelectObjects.Contains(each)) {
                each.layer = activeTrueLayerIndex;

                // 하위 객체의 레이어 변경 => Ground로 바꿔서 지나갈 수 있어야함
                foreach (Transform child in each.transform) {
                    child.gameObject.layer = activeTrueLayerIndex;
                }
            }
            else {
                each.layer = activeFalseLayerIndex;

                // 하위 객체의 레이어 변경 => Root3D가 안보여야함
                foreach (Transform child in each.transform) {
                    child.gameObject.layer = activeFalseLayerIndex;
                }
            }
        }
    }

    public override void ChangeLayerAllActiveTrue() {
        foreach (GameObject each in AllObjects) {
            each.layer = activeTrueLayerIndex;

            if (HasMatchName(each, parentName)) {
                ChangeLayerActiveWithAllChild(each.transform, activeTrueLayerIndex);
            }
            else {
                // 하위 객체의 레이어 변경 
                foreach (Transform child in each.transform) {
                    child.gameObject.layer = activeTrueLayerIndex;
                }
            }

        }
    }



    public override void AddSelectObjects(GameObject selectCheck) {
        // cube 
        if (selectCheck.name.Contains("Cube")) {
            AddListIfNotSelected(SelectObjects, selectCheck);
        }
        else {
            Transform parent = selectCheck.transform.parent;

            if (parent.name.Contains("Tree")) {
                BoxCollider boxcol = selectCheck.GetComponentInChildren<BoxCollider>();
                if (boxcol != null) {
                    // 선택한 object가 나무일 경우에 중심부가 skill section안에 들어오는지 확인
                    Bounds bounds = boxcol.bounds;

                    float minSectionZ = Mathf.Min(playerManage.StartSection.z, playerManage.FinishSection.z);
                    float maxSectionZ = Mathf.Max(playerManage.StartSection.z, playerManage.FinishSection.z);

                    bool isBoundInside = (bounds.center.z >= minSectionZ && bounds.center.z <= maxSectionZ);

                    if (isBoundInside) {
                        AddListIfNotSelected(SelectObjects, parent.gameObject);
                    }
                }
            }
            else {
                AddListIfNotSelected(SelectObjects, parent.gameObject);
            }
        }

    }
}

/* 삭제 되지않는 타일이 아닌 오브젝트 
 - 해당하는 오브젝트들 
1. object
2. cube 
*/