using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState3D : StateBase {

    private Player3DControl control3D;

    public Player3DControl Control3D { get { return control3D; } }

    protected virtual void Awake() {
        control3D = base.transform.GetComponent<Player3DControl>();
    }
}
