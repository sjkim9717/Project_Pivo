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


    public IEnumerator StartMove(Vector3 waypoint) {

        Vector3 startPosition = transform.position;
        Vector3 endPosition = waypoint;

        float elapsedTime = 0f;
        while (elapsedTime < 1f) {
            elapsedTime += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Slerp(startPosition, endPosition, elapsedTime);

            //Debug.Log("StartMove | " + transform.name);
            //Debug.Log("StartMove | " + transform.position);
            yield return null;
        }
        transform.position = waypoint;
    }
}
