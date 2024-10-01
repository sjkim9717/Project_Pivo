using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertMode_Destroy : ConvertMode {
    protected override void Start() {
        InitParentObjectWithTag(ConvertItem.Destroy);

        ChangeLayerAllActiveTrue();
    }

    public void DeleteDestroiedObject(GameObject deleteObject) {
        if (AllObjects.Contains(deleteObject)) {
            AllObjects.Remove(deleteObject);
        }
    }

    public override void ChangeLayerAllActiveTrue() {
        foreach (GameObject each in AllObjects) {
            each.layer = activeTrueLayerIndex;

            // 하위 객체의 레이어 변경 
            foreach (Transform child in each.transform) {
                child.gameObject.layer = activeTrueLayerIndex;
            }
        }
    }

    public override void AddSelectObjects(GameObject selectCheck) {
        Transform parent = selectCheck.transform.parent;
        AddListIfNotSelected(SelectObjects, parent.gameObject);
    }
}

/* 삭제 되지않는 타일이 아닌 오브젝트 
 - 해당하는 오브젝트들 
1. biscuit
2. destroy tile

*/