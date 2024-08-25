using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class StageSaveData {
    public bool[] stage = new bool[3];
}

public class Save : MonoBehaviour
{
    public static Save instance = null;

    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitSave();
        }
        else {
            Destroy(gameObject);
        }

        SaveJsonFilePath = Path.Combine(Application.persistentDataPath, "Save/SaveData.json");
        if (!Directory.Exists(Path.GetDirectoryName(SaveJsonFilePath))) {
            Directory.CreateDirectory(Path.GetDirectoryName(SaveJsonFilePath));
        }
        Debug.Log("single file path :" + SaveJsonFilePath);
    }


    public StageSaveData SaveData = new StageSaveData();

    private string SaveJsonFilePath;

    //TODO:new play Ŭ���� Ȯ��
    public bool GetSaveExist() {                            // saveData �ִ��� Ȯ���ϴ� �뵵
        if (File.Exists(SaveJsonFilePath)) return true;
        return false;
    }
    
    //TODO: stage Ŭ����� �ش��ϴ� stage�� ���ٱ� ���� 
    

    public void MakeSave() {
        if (SaveData == null) {
            SaveData = new StageSaveData();
        }

        File.WriteAllText(SaveJsonFilePath, JsonUtility.ToJson(SaveData));  // �����
    }

    public StageSaveData Load() {
        if (GetSaveExist()) {
            return JsonUtility.FromJson<StageSaveData>(File.ReadAllText(SaveJsonFilePath));
        }
        return null;
    }

    public void InitSave() {
        for (int i = 0; i < SaveData.stage.Length; i++) {
            SaveData.stage[i] = false;
            //TODO: �� �������� �� ���ٱ� ���� init �߰�
        }
    }
}

/*
 1. ���� : ����

 2. ���� ���� 
    - �������� Ŭ���� ����
    - //TODO: �� �������� �� ���ٱ� ���� 

    - //TODO: �� �������� �� Ŭ���� ���� ����

 3. �ؾ��ϴ� ����
    - �̱���
    - �ڵ� ����

 4. ����
    - ���ÿ� ���� ���� ����
    - ���� ���� ����=> �������� Ŭ�����
    - new game �� ��� ���� �ִ� �� Ȯ��
    - Load -> stage ���ÿ��� �� �������� ���� �ſ� �÷��̾ �� ����
 */
