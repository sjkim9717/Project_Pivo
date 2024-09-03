using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public abstract class ConvertMode : MonoBehaviour {

    protected int activeTrueLayerIndex;
    protected int activeFalseLayerIndex;

    public Material BlockMaterial;
    public Material SelectMaterial;

    protected GameObject[] parentObject;
    [SerializeField]
    protected List<GameObject> AllObjects = new List<GameObject>();

    protected virtual void Awake() {
        activeTrueLayerIndex = LayerMask.NameToLayer("ActiveTrue");
        activeFalseLayerIndex = LayerMask.NameToLayer("ActiveFalse");

        // 자산 로딩을 지연시키고, 나중에 실행하도록 변경
        if (BlockMaterial == null || SelectMaterial == null) {
            StartCoroutine(LoadMaterialsAsync());
        }
    }

    protected abstract void Start();

    private IEnumerator LoadMaterialsAsync() {
        // 자산 로딩을 코루틴으로 지연
        yield return null;  // 프레임 대기

        string basePath = "ConvertModeMaterial";
        BlockMaterial = LoadMaterial(Path.Combine(basePath, "BlockMaterialDumy"));
        SelectMaterial = LoadMaterial(Path.Combine(basePath, "SelectMaterialDumy"));
    }

    private Material LoadMaterial(string path) {
        Material material = Resources.Load<Material>(path);
        if (material != null) {
            Debug.Log($"Loaded Material: {material.name}");
        }
        else {
            Debug.LogWarning($"Material not found at path: {path}");
        }
        return material;
    }

    protected void InitParentObjectWithTag(Tag tagName) {
        parentObject = new GameObject[GameObject.FindGameObjectsWithTag($"{tagName}").Length];
        for (int i = 0; i < parentObject.Length; i++) {
            parentObject[i] = GameObject.FindGameObjectsWithTag($"{tagName}")[i];
        }

        foreach (GameObject parenttile in parentObject) {
            foreach (Transform child in parenttile.transform) {
                if (!child.name.Contains("Root") && !child.name.Contains("3D"))
                    AllObjects.Add(child.gameObject);
            }
        }
    }


    public virtual void ChangeActiveWithLayer() {
        foreach (GameObject each in AllObjects) {
            if (each.layer == activeTrueLayerIndex) {
                each.SetActive(true);
            }
            else if (each.layer == activeFalseLayerIndex) {
                each.SetActive(false);
            }
        }
    }

    public void ChangeLayerActiveFalse(List<GameObject> objects) {
        foreach (GameObject each in AllObjects) {
            if (!objects.Contains(each)) each.layer = activeFalseLayerIndex;
        }
    }

    public void ChangeLayerAllActiveTrue() {
        foreach (GameObject each in AllObjects) {
            each.layer = activeTrueLayerIndex;
            //Debug.LogWarning(" ChangeLayer | " + each.name + " | " + each.layer);
        }
    }
}
