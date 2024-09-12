using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DoorData", menuName = "ScriptableObjects/DoorData", order = 2)]

public class DoorData : ScriptableObject {

    [Space(5)]
    [Header("Door Info")]
    public int Password;
    public GameObject DoorPrefab;

}
