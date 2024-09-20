using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ObjectFinderTool : EditorWindow {
    private GameObject parentObject;
    private string targetName;

    [MenuItem("Utility/Object Finder Tool")]
    public static void ShowWindow() {
        GetWindow<ObjectFinderTool>("Object Finder Tool");
    }

    void OnGUI() {
        GUILayout.Label("Object Finder Tool", EditorStyles.boldLabel);
        parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(GameObject), true);
        targetName = EditorGUILayout.TextField("Child Name", targetName);

        if (GUILayout.Button("Find and Select Children")) {
            FindAndSelectChildren(parentObject.transform, targetName);
        }
    }

    void FindAndSelectChildren(Transform parent, string name) {
        if (parent == null) {
            Debug.LogWarning("Parent object is null!");
            return;
        }

        // 일치하는 오브젝트를 저장할 리스트
        List<GameObject> foundObjects = new List<GameObject>();

        // 첫 번째 단계 자식 검색
        foreach (Transform child in parent) {
            if (child.name == name) {
                foundObjects.Add(child.gameObject);
            }

            // 두 번째 단계 자식 검색
            foreach (Transform grandChild in child) {
                if (grandChild.name == name) {
                    foundObjects.Add(grandChild.gameObject);
                }
            }
        }

        // 발견된 오브젝트가 있는 경우
        if (foundObjects.Count > 0) {
            foreach (var obj in foundObjects) {
                Debug.Log("Found child: " + obj.name);
            }
            // Hierarchy에서 발견된 오브젝트 선택
            Selection.objects = foundObjects.ToArray();
        }
        else {
            Debug.LogWarning("Child not found");
        }
    }
}
