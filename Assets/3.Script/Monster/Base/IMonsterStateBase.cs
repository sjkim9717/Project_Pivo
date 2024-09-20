using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonsterStateBase {
    public void EnterState(MonsterControl MControl);
    public void UpdateState(MonsterControl MControl);
    public void ExitState(MonsterControl MControl);

}
