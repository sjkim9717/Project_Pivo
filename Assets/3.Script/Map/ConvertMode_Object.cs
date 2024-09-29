using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ConvertMode_Object : ConvertMode {

    protected override void Start() {
        InitParentObjectWithTag(ConvertItem.Objects);

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
        if (selectCheck.name.Contains("Tree")) {
            BoxCollider boxcol = selectCheck.GetComponentInChildren<BoxCollider>();
            if (boxcol !=null) {
                // 선택한 object가 나무일 경우에 중심부가 skill section안에 들어오는지 확인
                Bounds bounds = boxcol.bounds;

                float minSectionZ = Mathf.Min(playerManage.StartSection.z, playerManage.FinishSection.z);
                float maxSectionZ = Mathf.Max(playerManage.StartSection.z, playerManage.FinishSection.z);

                bool isBoundInside = (bounds.center.z >= minSectionZ && bounds.center.z <= maxSectionZ);

                if (isBoundInside) {
                    if (!SelectObjects.Contains(selectCheck)) {
                        SelectObjects.Add(selectCheck);
                    }
                }
            }
        }
        else {
            if (!SelectObjects.Contains(selectCheck)) {
                SelectObjects.Add(selectCheck);
            }
        }
    }
}

/* 삭제 되지않는 타일이 아닌 오브젝트 
 - 해당하는 오브젝트들 
1. object

*/