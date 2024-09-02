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

        string basePath = "Assets/6.Materials/ConvertModeMaterial";

        BlockMaterial = LoadMaterial(Path.Combine(basePath, "BlockMaterialDumy.mat"));
        SelectMaterial = LoadMaterial(Path.Combine(basePath, "SelectMaterialDumy.mat"));
    }
    protected abstract void Start();

    private Material LoadMaterial(string path) {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null) {
            Debug.Log($"Loaded Material: {material.name}");
        }
        else {
            Debug.LogWarning($"Material not found at path: {path}");
        }
        return material;
    }

    protected void InitParentObjectWithTag(string tagName) {
        parentObject = new GameObject[GameObject.FindGameObjectsWithTag($"{tagName}").Length];
        for (int i = 0; i < parentObject.Length; i++) {
            parentObject[i] = GameObject.FindGameObjectsWithTag($"{tagName}")[i];
        }

        foreach (GameObject parenttile in parentObject) {
            foreach (Transform child in parenttile.transform) {
                AllObjects.Add(child.gameObject);
                if (child.name.Contains("Root")||child.name.Contains("3D"))
                    Debug.LogWarning(" tile list error" + child.name);
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
