using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
    Up,
    Down,
    Left,
    Right
}


public class Waypoint : MonoBehaviour {

    public Transform[] LevelWaypoint;    // 버튼이 들고있는 위치들

    // 방향에 따른 레벨 연결
    private readonly Dictionary<StageLevel, Dictionary<Direction, StageLevel>> LevelConnections = new Dictionary<StageLevel, Dictionary<Direction, StageLevel>> {
        {
            StageLevel.GrassStageLevel_1,
            new Dictionary<Direction, StageLevel> {
                { Direction.Right, StageLevel.GrassStageLevel_5 }
            }
        },
        {
            StageLevel.GrassStageLevel_5,
            new Dictionary<Direction, StageLevel> {
                { Direction.Left, StageLevel.GrassStageLevel_1 },
                { Direction.Down, StageLevel.GrassStageLevel_7 }
            }
        },
        {
            StageLevel.GrassStageLevel_7,
            new Dictionary<Direction, StageLevel> {
                { Direction.Up, StageLevel.GrassStageLevel_5 },
                { Direction.Right, StageLevel.SnowStageLevel_3 }
            }
        },
        {
            StageLevel.SnowStageLevel_3,
            new Dictionary<Direction, StageLevel> {
                { Direction.Left, StageLevel.GrassStageLevel_7 },
                { Direction.Right, StageLevel.SnowStageLevel_4 }
            }
        },
        {
            StageLevel.SnowStageLevel_4,
            new Dictionary<Direction, StageLevel> {
                { Direction.Down, StageLevel.SnowStageLevel_3 },
                { Direction.Right, StageLevel.SnowStageLevel_6 }
            }
        },
        {
            StageLevel.SnowStageLevel_6,
            new Dictionary<Direction, StageLevel> {
                { Direction.Left, StageLevel.SnowStageLevel_4 },
                { Direction.Down, StageLevel.SnowStageLevel_7 }
            }
        },
        {
            StageLevel.SnowStageLevel_7,
            new Dictionary<Direction, StageLevel> {
                { Direction.Left, StageLevel.SnowStageLevel_6 }
            }
        }

    };

    private void Awake() {
        LevelWaypoint = new Transform[GetComponentsInChildren<StageButton>().Length];
        int index = 0;
        foreach (Transform item in transform) {
            if (item.TryGetComponent(out StageButton stageButton)) {
                LevelWaypoint[index] = item;
                index++;
            }
        }
    }


    public Transform FindCurrentPosition(StageLevel currentLevel) {
        foreach (Transform item in LevelWaypoint) {
            StageLevel buttonlevel = item.GetComponent<StageButton>().ButtonStageLevel;
            if (buttonlevel == currentLevel) {
                return item.transform;
            }
        }
        return null;
    }


    // 현재 플레이어가 위치한 레벨과 방향키의 입력을 받아서 방향키의 레벨위치를 반환
    public bool CanMoveTo(StageLevel currentLevel, Direction direction, out StageLevel selectLevel) {
        // 현재 스테이지에 따른 변경가능한 방향 배열을 가져옴
        if (LevelConnections.TryGetValue(currentLevel, out Dictionary<Direction, StageLevel> validDirections)) {
            // 변경가능한 방향 배열 중 입력된 방향의 스테이지를 들고옴
            if (validDirections.TryGetValue(direction, out StageLevel stage)) {
                selectLevel = stage;
                Debug.Log(" 입력된 방향 스테이지 확인 | " + stage);
                return true;
            }
        }
        selectLevel = currentLevel;
        return false;
    }

    // 선택된 방향의 레벨이 출입 가능한지확인
    public bool CanEnterTo(StageLevel currentLevel, StageLevel selectLevel, ref Transform nextWaypoint) {
        bool isClear = false;
        if (Save.instance.TryGetStageClear(currentLevel, out isClear)) {            // 고른 스테이지가 현재 스테이지보다 높은 경우
            if (isClear) {
                nextWaypoint = FindCurrentPosition(selectLevel);
                Debug.Log("  고른 스테이지가 현재 스테이지보다 높은 경우 스테이지 확인 | " + selectLevel);
                return true;
            }
            else {
                if (Save.instance.TryGetStageClear(selectLevel, out isClear)) {     // 고른 스테이지가 현재 스테이지보다 낮은 경우
                    if (isClear) {
                        nextWaypoint = FindCurrentPosition(selectLevel);
                        Debug.Log(" 고른 스테이지가 현재 스테이지보다 낮은 경우 스테이지 확인 | " + selectLevel);
                        return true;
                    }
                }
            }
        }

        return false;
    }
}

/*
 1. 목적 : 플레이어가 클리어한 스테이지 정보에 따라 지정된 방향키를 확인하고 움직일 수 있게 하기 위함
 
 */