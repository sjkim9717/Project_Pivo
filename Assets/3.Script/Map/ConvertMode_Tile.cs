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

                // 하위 객체의 레이어 변경 => Root3D가 안보여야함
                if (item.name.Contains("BombSpawn") || item.name.Contains("MoveSwitch")) {
                    ChangeLayerActiveWithAllChild(item.transform, activeFalseLayerIndex);
                }
                else {
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

}

/* 삭제 되지않는 타일이 아닌 오브젝트 
 - 해당하는 오브젝트들 
1. tile
2. tile_2
3. moving_object

*/