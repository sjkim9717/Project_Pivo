using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertMode_Destroy : ConvertMode {
    protected override void Start() {
        InitParentDestroyObject(Tag.Destroy);

        ChangeLayerAllActiveTrue();
    }

    public void DeleteDestroiedObject(GameObject deleteObject) {
        if (AllObjects.Contains(deleteObject)) {
            AllObjects.Remove(deleteObject);
        }
    }
}
