using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PipeObject : MonoBehaviour {
    public enum Terminal {Start, Mid, End };

    public Terminal State;

    public PipeWaypoint Waypoint = new PipeWaypoint();

    private void Awake() {
        Waypoint.SettingPosition(gameObject, Waypoint.Start, ref Waypoint.StartPos);
        Waypoint.SettingPosition(gameObject, Waypoint.End, ref Waypoint.EndPos);

        PlayerManage.instance.IsSwitchMode += SwitchMode;
    }

    #region 활성화
    private void SwitchMode() {
        gameObject.SetActive(IsInSelectArea());
    }

    private bool IsInSelectArea() {
        if (PlayerManage.instance.StartSection.z >= PlayerManage.instance.FinishSection.z) {
            if (transform.position.z <= PlayerManage.instance.StartSection.z && transform.position.z >= PlayerManage.instance.FinishSection.z) return true;
            else return false;
        }
        else {
            if (transform.position.z >= PlayerManage.instance.StartSection.z && transform.position.z <= PlayerManage.instance.FinishSection.z) return true;
            else return false;
        }
    }
    #endregion

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.TryGetComponent(out PipeObject pipe)) {
            if (State == Terminal.Start) {
                Waypoint.IsEndConnect=Waypoint.MatchConnection(State, pipe.Waypoint);
            }
            else if(State == Terminal.Mid) {
                if (Waypoint.IsStartConnect)
                    Waypoint.MatchConnection(State, pipe.Waypoint);
            }
        }
    } 
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.TryGetComponent(out PipeObject pipe)) {
            if (State == Terminal.Start) {
                if(Waypoint.MatchConnection(State, pipe.Waypoint)) {
                    Waypoint.IsEndConnect = true;
                    if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = true;
                }
            }
            else if (State == Terminal.Mid) {
                if (Waypoint.IsStartConnect) {
                    if (Waypoint.MatchConnection(State, pipe.Waypoint)) {
                        Waypoint.IsEndConnect = true;
                        if (pipe.State != Terminal.Start) pipe.Waypoint.IsStartConnect = true;

                    }
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision) {
        
    }
    private void OnCollisionExit2D(Collision2D collision) {
        
    }
}

