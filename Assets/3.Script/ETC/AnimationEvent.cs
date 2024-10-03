using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    // public 메서드로 정의하여 애니메이션 이벤트에서 호출 가능
    public void OnAnimationEnd_PushBoxInit() {
        PlayerState3D_PushBox state3D_PushBox = GetComponentInParent<PlayerState3D_PushBox>();
        state3D_PushBox.isInitAnimationEnd = true;
    }

    public void OnAnimationEnd_PushBoxEnd() {
        PlayerState3D_PushBox state3D_PushBox = GetComponentInParent<PlayerState3D_PushBox>();
        state3D_PushBox.isEndAnimationEnd = true;
    }
}
