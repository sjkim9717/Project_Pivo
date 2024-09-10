public enum PlayerMode {
    Player3D,
    Player2D,
    AutoMode
}

public enum PlayerState {
    Idle,
    Move,
    Climb,
    PushBox,
    Bomb,
    OpenPanel,
    Skill,
    Falling,
    Holding,
    Dead,
    Disable
}

public interface IPushBox {
    public void IInteractionPushBox(float horizontal, float vertical);
}

public interface IBomb {
    public void IBombMoveStart();
    public void IBombMoving();
    public void IBombMoveEnd();

    public void IBombExplosion();
}