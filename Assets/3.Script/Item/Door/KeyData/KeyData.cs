using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyData", menuName = "ScriptableObjects/KeyData", order = 3)]
public class KeyData : ScriptableObject {
    [Space(5)]
    [Header("Key Info")]
    public int Password;
    public GameObject KeyPrefab;

}
