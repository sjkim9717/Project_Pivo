using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    int activeTrueLayerIndex;
    int activeFalseLayerIndex;


    private GameObject[] parentTileObject;
    private List<GameObject> AllMapTiles = new List<GameObject>();

    public Material blockTileMaterial;
    public Material selectTileMaterial;

    private void Awake() {
        activeTrueLayerIndex = LayerMask.NameToLayer("ActiveTrue");
        activeFalseLayerIndex = LayerMask.NameToLayer("ActiveFalse");

        parentTileObject = new GameObject[GameObject.FindGameObjectsWithTag("ParentTile").Length];
        for (int i = 0; i < parentTileObject.Length; i++) {
            parentTileObject[i] = GameObject.FindGameObjectsWithTag("ParentTile")[i];
        }

        foreach (GameObject parenttile in parentTileObject) {
            foreach (Transform child in parenttile.transform) {
                AllMapTiles.Add(child.gameObject);
                if (child.name.Contains("Root"))
                    Debug.LogWarning(" tile list error" + child.name);
            }
        }

        ChangeTileLayerAllActive();
    }

    public void ChangeActiveTile() {
        foreach (GameObject eachTile in AllMapTiles) {
            if (eachTile.layer == activeTrueLayerIndex) {
                eachTile.SetActive(true);
            }
            else if (eachTile.layer == activeFalseLayerIndex) {
                eachTile.SetActive(false);
            }
        }
    }

    public void ChangeTileLayer(List<GameObject> objects) {
        foreach (GameObject each in AllMapTiles) {
            if(!objects.Contains(each)) each.layer = activeFalseLayerIndex;
        }
    }

    public void ChangeTileLayerAllActive() {
        foreach (GameObject each in AllMapTiles) {
            each.layer = activeTrueLayerIndex;
        }
    }

}
