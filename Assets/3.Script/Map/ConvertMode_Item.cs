using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ConvertMode_Item : ConvertMode {

    protected override void Start() {
        InitParentObjectWithTag(Tag.Triggers);
        InitParentObjectWithTag(Tag.Biscuit);
        InitParentObjectWithTag(Tag.Objects);
        InitParentObjectWithTag(Tag.Puzzle);
        InitParentObjectWithTag(Tag.Tree);

        ChangeLayerAllActiveTrue();
    }
}
