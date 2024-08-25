using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class FindMissingScripts : MonoBehaviour
{
    [MenuItem("Utility/Remove Missing Script")]
    private static void RemoveAllMissingScripts() {

        // ���� Ȱ��ȭ�� ���� ��� ��Ʈ(GameObject) ������Ʈ�� �����ɴϴ�.
        GameObject[] rootGameObjects = EditorSceneManager.GetActiveScene().GetRootGameObjects();

        // ��Ʈ ������Ʈ�� �Ʒ��� �ִ� ��� ������Ʈ�� �����մϴ�.
        Object[] allObjectsInHierarchy = EditorUtility.CollectDeepHierarchy(rootGameObjects);

        int componentCount = 0;  // ���ŵ� �̽� ��ũ��Ʈ ������Ʈ�� �� ������ ����ϱ� ���� ����
        int gameObjectCount = 0; // �̽� ��ũ��Ʈ ������Ʈ�� ������ �ִ� ���� ������Ʈ�� ���� ����ϱ� ���� ����

        // ���̾��Ű�� �ִ� ��� ������Ʈ�� ��ȸ�մϴ�.
        foreach (Object obj in allObjectsInHierarchy) {
            // ������Ʈ�� ���� ������Ʈ���� Ȯ���մϴ�.
            if (obj is GameObject go) {
                // �ش� ���� ������Ʈ�� �ִ� �̽� ��ũ��Ʈ ������Ʈ�� ���� �����ɴϴ�.
                int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);

                // �̽� ��ũ��Ʈ ������Ʈ�� �ϳ� �̻� ���� ���
                if (count > 0) {
                    // �۾��� �ǵ��� �� �ֵ��� �ش� ���� ������Ʈ�� ���� Undo�� ����մϴ�.
                    Undo.RegisterCompleteObjectUndo(go, "Remove Missing Scripts");

                    // �̽� ��ũ��Ʈ ������Ʈ�� �����մϴ�.
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);

                    // ī���͸� ������Ʈ�մϴ�.
                    componentCount += count;
                    gameObjectCount++;
                }
            }
        }

        // �ֿܼ� ����� ����մϴ�.
        Debug.Log($"�� {gameObjectCount}���� ���� ������Ʈ���� {componentCount}���� �̽� ��ũ��Ʈ ������Ʈ�� ����.");
    }
}
