using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState2D : StateBase {

    protected float horizontalInput;
    protected float skillSectionInput;
    protected float interactionInput;

    private Player2DControl control2D;
    public Player2DControl Control2D { get { return control2D; } }

    protected virtual void Awake() {
        control2D = base.transform.GetComponent<Player2DControl>();
    }
    protected virtual void OnEnable() {
        horizontalInput = 0;
        skillSectionInput = 0;
        interactionInput = 0;
    }


}
