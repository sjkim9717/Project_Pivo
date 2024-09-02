using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour {


    private Material defaltMaterial;
    [SerializeField] private Material blockTileMaterial;
    [SerializeField] private Material selectTileMaterial;

    private MeshRenderer tileRenderer;

    private void Awake() {
        tileRenderer = GetComponent<MeshRenderer>();
        defaltMaterial = tileRenderer.materials[0];

        blockTileMaterial = FindObjectOfType<ConvertMode>().BlockMaterial;
        selectTileMaterial = FindObjectOfType<ConvertMode>().SelectMaterial;
        //Debug.Log(defaltMaterial.name);
    }

    public void ChangeMaterial() {
        Material[] newMaterials = new Material[tileRenderer.materials.Length];
        newMaterials[0] = blockTileMaterial;

        tileRenderer.materials = newMaterials;
    }

    public void InitMaterial() {
        if (tileRenderer.materials[0] == defaltMaterial) return;
        Material[] newMaterials = new Material[tileRenderer.materials.Length];
        newMaterials[0] = defaltMaterial;

        tileRenderer.materials = newMaterials;
    }

    public void ChangeMaterial_select() {
        Material[] newMaterials = new Material[tileRenderer.materials.Length];
        newMaterials[0] = selectTileMaterial;

        tileRenderer.materials = newMaterials;

    }
}
