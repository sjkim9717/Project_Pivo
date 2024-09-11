using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Tag {
    ParentTile,
    Triggers,
    Biscuit,
    Objects,
    Puzzle,
    Tree,
    Bomb,
    Destroy,
    PushSwitch,
    OpenPanel
}

public enum ScreenMode {
    Window,
    FullScreen,
    FullScreenWindow
}

public enum TutorialTriggerSprite {
    Move2D,
    Move3D,
    Climb,
    ViewChange
}