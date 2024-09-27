using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertMode_Object : ConvertMode {

    protected override void Start() {
        InitParentObjectWithTag_Object(ConvertItem.Objects_2);

        ChangeLayerAllActiveTrue();
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

}

