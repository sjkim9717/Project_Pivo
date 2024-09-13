using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ConvertMode_Item : ConvertMode {

    protected override void Start() {
        InitParentObjectWithTag(ConvertItem.Objects);
        InitParentObjectWithTag_Object2(ConvertItem.Objects_2);

        ChangeLayerAllActiveTrue();
    }

    public void DeleteDestroiedObject(GameObject deleteObject) {
        if (AllObjects.Contains(deleteObject)) {
            AllObjects.Remove(deleteObject);
        }
    }
}
