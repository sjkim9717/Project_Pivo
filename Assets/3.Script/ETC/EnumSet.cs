using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
    Up,
    Down,
    Left,
    Right,
}

public enum ConvertItem {
    ParentTile,
    MoveTile,
    Objects,
    Objects_2,
    Destroy
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
