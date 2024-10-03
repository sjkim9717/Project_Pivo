using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserData : MonoBehaviour {

    protected Text saveScore;
    protected Text[] requireScore = new Text[3];

    protected StageLevel selectStageLevel = StageLevel.StageSelect;

    protected Dictionary<StageLevel, int[]> StageRequiementScore = new Dictionary<StageLevel, int[]> {
        {StageLevel.GrassStageLevel_1, new int[] { 0, 11, 15} },
        {StageLevel.GrassStageLevel_5, new int[] { 0, 8, 14} },
        {StageLevel.GrassStageLevel_7, new int[] { 0, 11, 15} },
        {StageLevel.SnowStageLevel_3, new int[] { 0, 10, 17} },
        {StageLevel.SnowStageLevel_4, new int[] { 0, 10, 17 } },
        {StageLevel.SnowStageLevel_6, new int[] { 0, 10, 23} },
        {StageLevel.SnowStageLevel_7, new int[] { 0, 8, 18} }
    };

    public void SelectStageLevel(StageLevel stageLevel) {
        selectStageLevel = stageLevel;
        int[] requirement;

        // 해당 레벨에 필요한 별 갯수
        if (StageRequiementScore.TryGetValue(stageLevel, out requirement)) {
            for (int i = 0; i < requirement.Length; i++) {
                if (requireScore[i].gameObject.activeSelf)
                    requireScore[i].text = requirement[i].ToString();
            }
        }

        // 플레이어가 획득한 별 갯수
        int _saveScore = 0;
        if (Save.instance.TryGetStageScore(stageLevel, out _saveScore)) {
            saveScore.text = string.Format($"x {_saveScore}");
        }
        else {
            saveScore.text = "x 0";
        }

    }

}
