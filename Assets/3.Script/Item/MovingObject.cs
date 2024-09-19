using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    private MovingWaypoints[] movingobjects;
    private MovingSwitch[] switches;

    private void Awake() {
        movingobjects = GetComponentsInChildren<MovingWaypoints>();

        switches = GetComponentsInChildren<MovingSwitch>();
    }

    public void OderToMoveObjects(int password) {
        List<(MovingWaypoints obj, int order)> moveObjectWithOrder = new List<(MovingWaypoints, int)>();

        foreach (MovingWaypoints item in movingobjects) {
            for (int i = 0; i < item.waypointGroups.Count; i++) {
                if (item.waypointGroups[i].Password == password) {
                    // Add the GameObject and its MovingOrder to the list
                    moveObjectWithOrder.Add((item, item.waypointGroups[i].MovingOrder));
                }
            }
        }

        // Sort the list by MovingOrder
        moveObjectWithOrder.Sort((a, b) => a.order.CompareTo(b.order));

        // Now, you can extract the sorted GameObjects back into a separate list if needed
        List<MovingWaypoints> sortedObjects = moveObjectWithOrder.Select(x => x.obj).ToList();

        Debug.Log("OderToMoveObjects | sortedObjects length | " + sortedObjects.Count);

        // 각 객체 이동
        StartCoroutine(MoveSortedObjects(sortedObjects, password));

    }



    private IEnumerator MoveSortedObjects(List<MovingWaypoints> sortedObjects, int password) {
        for (int i = 0; i < sortedObjects.Count; i++) {
            MovingWaypoints currentObject = sortedObjects[i];

            for (int j = 0; j < currentObject.waypointGroups.Count; j++) {
                if (currentObject.waypointGroups[j].Password == password) {
                    for (int k = 0; k < currentObject.waypointGroups[j].MovingWaypoints.Count; k++) {
                        // 이동 시작
                        Debug.Log("MoveSortedObjects | currentObject.waypointGroups 코르틴 시작 | "+ currentObject.gameObject.name);
                        Debug.LogWarning("MoveSortedObjects | MovingWaypoints | " + currentObject.waypointGroups[j].MovingWaypoints[k]);
                        yield return StartCoroutine(currentObject.StartMove(currentObject.waypointGroups[j].MovingWaypoints[k]));
                        Debug.Log("MoveSortedObjects | currentObject.waypointGroups 코르틴 끝 | " + currentObject.gameObject.name);
                    }
                    break;
                }
            }
        }
    }

}
