using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public abstract class ConvertMode : MonoBehaviour {

    protected int activeTrueLayerIndex;
    protected int activeFalseLayerIndex;
    protected int groundLayerIndex;

    public Material BlockMaterial;
    public Material SelectMaterial;

    protected GameObject[] parentObject;
    [SerializeField]
    protected List<GameObject> AllObjects = new List<GameObject>();
    public List<GameObject> SelectObjects = new List<GameObject>();              // skill 사용하면 해당 구역의 오브젝트들이 전체 담김
    public List<GameObject> blockObjects = new List<GameObject>();              // skill 사용하면 해당 구역의 플레이어와 겹친 오브젝트가 담김

    public List<Material> defaltMaterial = new List<Material>();

    protected PlayerManage playerManage;

    protected virtual void Awake() {

        activeTrueLayerIndex = LayerMask.NameToLayer("ActiveTrue");
        activeFalseLayerIndex = LayerMask.NameToLayer("ActiveFalse");
        groundLayerIndex = LayerMask.NameToLayer("Ground");

        playerManage = FindObjectOfType<PlayerManage>();
    }

    protected abstract void Start();


    // Tile이 들어간 tag는 자기 자신외 자식객체에서 tile이 포함되어있는지 확인하고 해당 오브젝트를 담음
    protected virtual void InitParentObjectWithTag(ConvertItem tagName) {
        parentObject = GameObject.FindGameObjectsWithTag($"{tagName}");

        foreach (GameObject each in parentObject) {
            Renderer[] allChildRenderers = each.GetComponentsInChildren<Renderer>();

            foreach (Renderer item in allChildRenderers) {
                GameObject parentObject = item.transform.parent?.gameObject; // null 조건부 연산자 사용

                if (parentObject != null ){
                    if (!parentObject.name.Contains("3D")) {
                        if (!AllObjects.Contains(parentObject)) {
                            AllObjects.Add(parentObject);
                        }
                    }
                    else {
                        if (!AllObjects.Contains(parentObject.transform.parent.gameObject)) {
                            AllObjects.Add(parentObject.transform.parent.gameObject);
                        }
                    }

                }
            }
        }
    }

    // 플레이어 스킬로 해당 영역 잘렸을 경우 selet objects에 담음
    public abstract void AddSelectObjects(GameObject selectCheck);

    // 레이어 변경해서 화면상에 비출지 결정
    public virtual void ChangeLayerActiveFalseInSelectObjects() {
        foreach (GameObject item in AllObjects) {
            if (!SelectObjects.Contains(item)) {

                item.layer = activeFalseLayerIndex;

                // 하위 객체의 레이어 변경 => Root3D가 안보여야함
                foreach (Transform child in item.transform) {
                    child.gameObject.layer = activeFalseLayerIndex;
                }
            }
        }
    }

    public abstract void ChangeLayerAllActiveTrue();

    public virtual void ChangeLayerActiveTrueWhen3DModeCancle() {
        foreach (GameObject each in AllObjects) {
            if (SelectObjects.Contains(each)) {
                each.layer = activeTrueLayerIndex;

                // 하위 객체의 레이어 변경 => Ground로 바꿔서 지나갈 수 있어야함
                foreach (Transform child in each.transform) {
                    child.gameObject.layer = groundLayerIndex;
                }
            }
            else {
                each.layer = activeFalseLayerIndex;

                // 하위 객체의 레이어 변경 => Root3D가 안보여야함
                foreach (Transform child in each.transform) {
                    child.gameObject.layer = activeFalseLayerIndex;
                }
            }
        }
    }

    // 자식을 전부 돌아서 레이어를 변경해야함
    protected virtual void ChangeLayerActiveWithAllChild(Transform parent, int layerIndex) {
        foreach (Transform child in parent) {
            child.gameObject.layer = layerIndex;

            ChangeLayerActiveWithAllChild(child, layerIndex);
        }
    }

    //  ======================================================================

    public virtual void AddBlockObject(GameObject blockCheck) {
        if (blockCheck.name.Contains("Tile")) {
            blockObjects.Add(blockCheck);

            MeshRenderer tileRenderer = blockCheck.GetComponentInChildren<MeshRenderer>();
            defaltMaterial.Add(tileRenderer.materials[0]);
        }
    }
    public virtual void ClearBlockObject() {
        blockObjects.Clear();
        defaltMaterial.Clear();
    }

    public virtual void ChangeMaterial_Origin() {
        if (defaltMaterial.Count != blockObjects.Count) {
            Debug.LogWarning("defaultMaterial and blockObjects lists must have the same number of items.");
            return;
        }

        for (int i = 0; i < defaltMaterial.Count; i++) {
            MeshRenderer tileRenderer = blockObjects[i].GetComponentInChildren<MeshRenderer>();
            Material[] newMaterials = new Material[tileRenderer.materials.Length];

            MeshRenderer defaultRenderer = blockObjects[i].GetComponentInChildren<MeshRenderer>();
            
            for (int j = 0; j < newMaterials.Length; j++) {
                newMaterials[j] = defaltMaterial[i]; // 각 블록에 대해 기본 머티리얼 설정
            }

            tileRenderer.materials = newMaterials; // 새 머티리얼 배열 할당
        }
    }

    public virtual void ChangeMaterial_Block() {
        for (int i = 0; i < defaltMaterial.Count; i++) {
            MeshRenderer tileRenderer = blockObjects[i].GetComponentInChildren<MeshRenderer>();
            Material[] newMaterials = new Material[tileRenderer.materials.Length];
            newMaterials[0] = BlockMaterial;
            tileRenderer.materials = newMaterials;
        }
    }

    public virtual void ChangeMaterial_select() {
        for (int i = 0; i < defaltMaterial.Count; i++) {
            MeshRenderer tileRenderer = blockObjects[i].GetComponentInChildren<MeshRenderer>();
            Material[] newMaterials = new Material[tileRenderer.materials.Length];
            newMaterials[0] = SelectMaterial;
            tileRenderer.materials = newMaterials;
        }
    }

    //TODO: 시간 나면 바꿀 것
    protected void AddParentOfParent(List<GameObject> listObject, Transform parent) {
        Transform targetTransform = parent.parent;
        if (targetTransform != null) {
            AddIfNotSelected(listObject, targetTransform.gameObject);
        }
    }

    // Adds the object to the selection list if it's not already in the list
    protected void AddIfNotSelected(List<GameObject> listObject, GameObject addObject) {
        if (!listObject.Contains(addObject)) {
            listObject.Add(addObject);
        }
    }

    #region 수정 전 object로 잡음
    /*
    protected void InitParentObjectWithTag(ConvertItem tagName) {
        parentObject = new GameObject[GameObject.FindGameObjectsWithTag($"{tagName}").Length];
        for (int i = 0; i < parentObject.Length; i++) {
            parentObject[i] = GameObject.FindGameObjectsWithTag($"{tagName}")[i];
        }

        foreach (GameObject parenttile in parentObject) {
            foreach (Transform child in parenttile.transform) {
                if (child.name.Contains("Root3D")) {
                    GameObject parentGameObject = child.parent.gameObject;
                    if (!AllObjects.Contains(parentGameObject)) {
                        AllObjects.Add(parentGameObject);
                    }
                }
                else if (!child.name.Contains("Root") && !child.name.Contains("3D")) {
                    if (!AllObjects.Contains(child.gameObject)) {
                        AllObjects.Add(child.gameObject);
                    }
                }
            }
        }
    }

    protected void InitParentObjectWithTag_Object2(ConvertItem tagName) {
        parentObject = new GameObject[GameObject.FindGameObjectsWithTag($"{tagName}").Length];
        for (int i = 0; i < parentObject.Length; i++) {
            parentObject[i] = GameObject.FindGameObjectsWithTag($"{tagName}")[i];
        }
        for (int i = 0; i < parentObject.Length; i++) {
            foreach (Transform child in parentObject[i].transform) {
                foreach (Transform item in child) {
                    if (!AllObjects.Contains(item.gameObject)) {
                        AllObjects.Add(item.gameObject);
                    }
                }
            }
        }

    }
    */

    #endregion

    #region 수정 전 오브젝트를 활성화 비활성화
    /*
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
            if (each.CompareTag("PushBox")) {
                foreach (Transform item in each.transform) {
                    item.gameObject.layer = activeTrueLayerIndex;
                }
            }
        }
    }

    public void ChangeLayerActiveTrueWhen3DModeCancle() {
        foreach (GameObject each in AllObjects) {
            if (SelectObjects.Contains(each)) {
                each.layer = activeTrueLayerIndex;
            }
            else {
                each.layer = activeFalseLayerIndex;
            }
        }
    }
    */

    #endregion

}
