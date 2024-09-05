using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseGroup : MonoBehaviour {
    //TODO: SceneSelect Game Button에 달려있는 이벤트 트리거 => 첫번째 씬에서 튜토리얼 완료 안했다면 트리거 삭제
    private GameObject stageSelet;
    private EventTrigger SceneSelecteventTrigger;
    private List<EventTrigger.Entry> SceneSelectstoredEntries;

    private void Awake() {
        stageSelet = transform.GetChild(8).gameObject;
        Debug.Log("stage Selet" + stageSelet.name);
    }

    private void OnEnable() {
        // Scene 1번이 클리어 됬는지 확인해서 StageSelet Game button의 이벤트 트리거 들고옴
        SceneSelecteventTrigger = stageSelet.GetComponent<EventTrigger>();
        SceneSelectstoredEntries = new List<EventTrigger.Entry>(SceneSelecteventTrigger.triggers);

        if (CheckStage1Clear()) {
            // 기존 이벤트가 중복되지 않도록 클리어하고 다시 추가
            SceneSelecteventTrigger.triggers.Clear();
            SceneSelecteventTrigger.triggers.AddRange(SceneSelectstoredEntries);
        }
        else {
            SceneSelecteventTrigger.triggers.Clear();
        }
    }

    private bool CheckStage1Clear() {
        bool isStage1Clear;
        if (Save.instance.TryGetStageClear(StageLevel.StageLevel_1, out isStage1Clear)) {
            Debug.Log(" stage 1 clear" + isStage1Clear);
            return isStage1Clear;
        }
        return false;
    }


}
