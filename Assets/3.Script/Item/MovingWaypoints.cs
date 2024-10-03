using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class WaypointGroup {
    public int Password;
    public int MovingOrder;
    public List<Vector3> MovingWaypoints = new List<Vector3>();
}


public class MovingWaypoints : MonoBehaviour {
    public List<WaypointGroup> waypointGroups = new List<WaypointGroup>();
    private float moveSpeed = 1f;
    public Action IsMoved;


    public IEnumerator StartMove(Vector3 waypoint) {

        string[] include = { "Movetile" };
        string key = AudioManager.instance.GetDictionaryKey<string, List<AudioClip>>(AudioManager.SFX, include);
        AudioManager.instance.SFX_Play(AudioManager.instance.InGameAudio, key);

        Vector3 startPosition = transform.position;
        Vector3 endPosition = waypoint;

        float elapsedTime = 0f;
        while (elapsedTime < 1f) {
            elapsedTime += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Slerp(startPosition, endPosition, elapsedTime);

            yield return null;
        }
        transform.position = waypoint;

        AudioManager.instance.StopPlaying(AudioManager.instance.InGameAudio);

        IsMoved?.Invoke();
    }
}
