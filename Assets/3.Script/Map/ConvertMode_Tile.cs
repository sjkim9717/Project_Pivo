using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertMode_Tile : ConvertMode {

    protected override void Start() {
        InitParentObjectWithTag(Tag.ParentTile);

        ChangeLayerAllActiveTrue();
    }

}
