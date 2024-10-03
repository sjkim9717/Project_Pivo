using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBase : MonoBehaviour {
    public virtual void EnterState() { }

    public virtual void ExitState() { }
}