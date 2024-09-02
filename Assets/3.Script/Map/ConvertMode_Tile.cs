using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertMode_Tile : ConvertMode {

    private string useTagName = "ParentTile";

    protected override void Start() {
        InitParentObjectWithTag(useTagName);

        ChangeLayerAllActiveTrue();
    }

}
