using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster3DState_PassOut : IMonsterStateBase {
    public void EnterState(MonsterControl MControl) {
        MonsterManager.instance.Emotion.transform.GetChild(2).position = MonsterManager.instance.EmotionPoint3D.position;
        MonsterManager.instance.Emotion.transform.GetChild(2).gameObject.SetActive(true);
    }

    public void ExitState(MonsterControl MControl) {
    }

    public void UpdateState(MonsterControl MControl) {
        MonsterManager.instance.Emotion.transform.GetChild(2).gameObject.SetActive(false);
    }
}
