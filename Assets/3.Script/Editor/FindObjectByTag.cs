using UnityEngine;
using UnityEditor;

public class FindObjectByTag : EditorWindow {
    [SerializeField] private string targetTag;
    private string[] tags;
    private int selectedTagIndex;

    [MenuItem("Utility/Object Finder By Tag")]
    public static void ShowWindow() {
        GetWindow<FindObjectByTag>("Object Finder By Tag");
    }

    private void OnEnable() {
        // 태그 목록을 가져옵니다.
        tags = UnityEditorInternal.InternalEditorUtility.tags;
        selectedTagIndex = GetTagIndex(targetTag);
    }

    void OnGUI() {
        GUILayout.Label("Object Finder By Tag", EditorStyles.boldLabel);
        // 태그 드롭다운 메뉴
        selectedTagIndex = EditorGUILayout.Popup("Find Tag", selectedTagIndex, tags);
        targetTag = tags[selectedTagIndex]; // 선택된 태그 업데이트

        if (GUILayout.Button("Find Objects With Tag")) {
            FindAndHighlightAll();
        }
    }

    // 태그 이름의 인덱스를 반환하는 함수
    private int GetTagIndex(string tag) {
        for (int i = 0; i < tags.Length; i++) {
            if (tags[i] == tag) {
                return i;
            }
        }
        return 0; // 기본적으로 첫 번째 태그를 선택
    }

    // Finds all objects by tag and highlights them in the hierarchy
    public void FindAndHighlightAll() {
        GameObject[] targetObjects = GameObject.FindGameObjectsWithTag(targetTag);

        if (targetObjects.Length > 0) {
            // Select all found objects in the Unity Editor hierarchy
            Selection.objects = targetObjects;
            Debug.Log($"Found {targetObjects.Length} object(s) with tag '{targetTag}'.");

            foreach (var obj in targetObjects) {
                Debug.Log($"Object found: {obj.name}");
            }
        }
        else {
            Debug.LogError($"No objects with tag '{targetTag}' found.");
        }
    }
}
