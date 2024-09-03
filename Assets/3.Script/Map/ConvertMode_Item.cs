using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Tag {
    ParentTile,
    Triggers,
    Biscuit,
    Objects,
    Puzzle,
    Tree
}

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
