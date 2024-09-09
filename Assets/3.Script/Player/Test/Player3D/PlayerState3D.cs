using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState3D : StateBase {

    protected float horizontalInput;
    protected float verticalInput;
    protected float skillSectionInput;
    protected float interactionInput;

    private Player3DControl control3D;

    public Player3DControl Control3D { get { return control3D; } }

    protected virtual void Awake() {
        control3D = base.transform.GetComponent<Player3DControl>();
    }

    protected virtual void OnEnable() {        
        horizontalInput = 0;
        verticalInput = 0;
        skillSectionInput = 0;
        interactionInput = 0;        
    }
}
