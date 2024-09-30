using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertMode_Tile : ConvertMode {

    protected override void Start() {
        InitParentObjectWithTag(ConvertItem.ParentTile);

        ChangeLayerAllActiveTrue();
    }

    public override void ChangeLayerActiveFalseInSelectObjects() {

        foreach (GameObject item in AllObjects) {
            if (!SelectObjects.Contains(item)) {

                item.layer = activeFalseLayerIndex;
                foreach (Transform child in item.transform) {
                    child.gameObject.layer = activeFalseLayerIndex;
                }
            }
        }
    }

    public override void ChangeLayerAllActiveTrue() {
        foreach (GameObject each in AllObjects) {
            each.layer = activeTrueLayerIndex;

            // 하위 객체의 레이어 변경 => Ground로 바꿔서 지나갈 수 있어야함
            foreach (Transform child in each.transform) {
                child.gameObject.layer = groundLayerIndex;
            }
        }
    }

    public override void AddSelectObjects(GameObject selectCheck) {
        Transform parent = selectCheck.transform.parent;
        if (!SelectObjects.Contains(parent.gameObject)) {
            SelectObjects.Add(parent.gameObject);
        }
    }
}

/* 삭제 되지않는 타일이 아닌 오브젝트 
 - 해당하는 오브젝트들 
1. tile
2. tile_2
3. puzzle tile

*/
