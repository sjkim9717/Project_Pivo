using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState3D_Skill : PlayerState3D {
    [SerializeField] private float skillDistance = 14f;
    private int skillCount = 0;
    private int blockZposCheck = 0;

    private bool isSkillButtonPressed = false;

    private Vector3 startSection = Vector3.zero;
    private Vector3 finishSection = Vector3.zero;

    private GameObject sectionLine;
    private GameObject sectionLine_First;

    private List<GameObject> blockObjects = new List<GameObject>();              // skill 사용하면 해당 구역의 플레이어와 겹친 오브젝트가 담김

    private ConvertMode[] convertMode;
    protected override void OnEnable() {
        base.OnEnable();
        startSection = Vector3.zero;
        finishSection = Vector3.zero;
        isSkillButtonPressed = false;
        blockZposCheck = 0;
        skillCount = 0;
    }

    private void Start() {

        sectionLine = transform.parent.GetChild(4).gameObject;
        sectionLine_First = Instantiate(sectionLine, sectionLine.transform.parent);
        sectionLine_First.name = "SectionLine_First";


        convertMode = new ConvertMode[FindObjectsOfType<ConvertMode>().Length];

        convertMode[0] = FindObjectOfType<ConvertMode_Tile>();
        convertMode[1] = FindObjectOfType<ConvertMode_Item>();
        convertMode[2] = FindObjectOfType<ConvertMode_Destroy>();

    }

    public override void EnterState() {
        Control3D.Ani3D.SetBool("IsTryUseSkill", true);
    }
    private void Update() {
        skillSectionInput = Input.GetAxis("SkillSection");
        interactionInput = Input.GetAxisRaw("Interaction");

        ChangeState();
    }


    private void ChangeState() {
        if (PlayerManage.instance.CurrentState == PlayerState.Dead) {
            return;
        }
        else if (PlayerManage.instance.CurrentState == PlayerState.Disable) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Control3D.ChangeState(PlayerState.Idle);
            }
            else {
                return;
            }
        }


        if (skillSectionInput != 0 && !isSkillButtonPressed) { // 스킬 버튼이 눌렸는지 감지
            isSkillButtonPressed = true;                                            // 버튼이 눌린 상태로 표시
            skillCount++;
            Debug.Log("스킬 시도 등록. 현재 스킬 횟수: " + skillCount);
        }


        if (skillCount == 1) {
            CheckBlockObjectZPosition();                                            // 막힌 오브젝트의 위치를 비교함

            // 각 화살표 키가 눌렸는지 확인
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow)) {
                if (blockZposCheck != 1)
                    MoveSectionLine(true);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.DownArrow)) {
                if (blockZposCheck != -1)
                    MoveSectionLine(false);
            }
        }


        if (skillCount >= 2) {                                                      // 스킬 사용 시도 횟수가 2회 이상인지 확인
            if (CheckSkillUsable()) {                                               //TODO: [기억] 스킬 사용해서 2D로 변경됨

                for (int i = 0; i < convertMode.Length; i++) {
                    convertMode[i].ChangeLayerActiveFalse(convertMode[i].SelectObjects);
                }

                PlayerManage.instance.CurrentMode = PlayerMode.Player2D;
                PlayerManage.instance.SwitchMode();
                Debug.Log("2D 모드로 전환됨");
            }
            else {
                finishSection = startSection;
                sectionLine.transform.position = finishSection;
                sectionLine_First.transform.position = startSection;

                ResetSelectObject();                                               // 3D 모드 유지
                skillCount = 1;
                return;
            }
            skillCount = 0;                                                         // 스킬 시도 후 시도 횟수 초기화

            sectionLine.SetActive(false);
            sectionLine_First.SetActive(false);
        }


        if (skillSectionInput == 0 && isSkillButtonPressed) {                           // 스킬 버튼이 해제되었는지 감지
            isSkillButtonPressed = false;                                               // 버튼 눌림 상태를 초기화
        }



        if (interactionInput != 0) {       // cancle
            skillCount = 0;
            ResetSelectObject();
            sectionLine.SetActive(false);
            sectionLine_First.SetActive(false);

            PlayerManage.instance.IsChangingModeTo3D = false;
            Control3D.ChangeState(PlayerState.Idle);
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) {

            Control3D.ChangeState(PlayerState.Disable);
        }

    }

    private void CheckBlockObjectZPosition() {
        if (blockObjects.Count >= 1) {
            if (blockObjects[0].transform.position.z >= Control3D.PlayerRigid.position.z) blockZposCheck = 1;
            else blockZposCheck = -1;
        }
        else {
            blockZposCheck = 0;
        }
    }

    private void MoveSectionLine(bool up) {
        float direction = up ? 1f : -1f;

        sectionLine.SetActive(true);
        sectionLine_First.SetActive(true);

        if (startSection == Vector3.zero) {
            startSection = new Vector3(Control3D.PlayerRigid.position.x, Control3D.PlayerRigid.position.y, (int)Control3D.PlayerRigid.position.z - 1f);
            finishSection = new Vector3(startSection.x, startSection.y, startSection.z - direction * 2f);
        }
        else {
            float movingAmount = direction * 2f;

            // finishSection의 z 값 이동 및 클램프 적용
            finishSection.z += movingAmount;
            finishSection.z = Mathf.Clamp(finishSection.z, startSection.z - skillDistance, startSection.z + skillDistance);

            if (finishSection == startSection) {
                startSection = new Vector3(Control3D.PlayerRigid.position.x, Control3D.PlayerRigid.position.y, (int)Control3D.PlayerRigid.position.z - direction * 1f);
                finishSection = new Vector3(startSection.x, startSection.y, startSection.z + direction * 2f);
            }
        }


        sectionLine.transform.position = finishSection;
        sectionLine_First.transform.position = startSection;


        ResetSelectObject();                                                    // raycast가 움직이기전 초기화
        ChangeSelectObjectLayer(startSection, finishSection);                   // Raycast를 넣어서  
        ChangeBlockObjectMaterial();
    }

    // convertMode가 여러개 있을 경우 전체 담아서 초기화
    public void ResetSelectObject() {
        if (convertMode[0].SelectObjects == null) return;

        foreach (var item in convertMode[0].SelectObjects) {
            if (item.TryGetComponent(out TileController tile)) {
                tile.InitMaterial();
            }
        }

        blockObjects.Clear();

        for (int i = 0; i < convertMode.Length; i++) {
            convertMode[i].SelectObjects.Clear();
        }

        foreach (ConvertMode item in convertMode) {
            item.ChangeLayerAllActiveTrue();
        }
    }

    // 플레이어가 스킬 자르면 해당하는 영역을 확인해야함
    private void ChangeSelectObjectLayer(Vector3 start, Vector3 finish) {

        float centerpos = (finish.z + start.z) * 0.5f;
        Vector3 center = new Vector3((startSection.x + finishSection.x) * 0.5f, (startSection.y + finishSection.y) * 0.5f, centerpos);

        float zSize = Mathf.Abs(finish.z - start.z) / 2 - 0.5f;
        Vector3 halfExtents = new Vector3(30, 20, zSize); // 직사각형의 절반 크기

        Vector3 direction = (finish - start).normalized;

        RaycastHit[] hits = Physics.BoxCastAll(center, halfExtents, direction, Quaternion.identity, 0);

        foreach (RaycastHit hit in hits) {
            Transform currentTransform = hit.transform;

            // 최상위 부모까지 탐색
            while (currentTransform.parent != null) {
                currentTransform = currentTransform.parent;
            }
            // 최상위 부모의 태그 확인
            string tagName = currentTransform.tag;

            if (Enum.TryParse(tagName, out ConvertItem tagEnum)) {

                Transform parent = hit.transform.parent;

                // 부모의 두 번째 레벨까지 확인
                Transform targetTransform = parent?.parent != null && parent.parent.CompareTag("PushBox") ? parent.parent : parent;

                AddSelectObjectsWithTag(tagEnum, targetTransform.gameObject);
            }
        }
    }

    private void AddSelectObjectsWithTag(ConvertItem tagName, GameObject parent) {

        switch (tagName) {
            case ConvertItem.ParentTile:
                if (!convertMode[0].SelectObjects.Contains(parent)) {
                    //Debug.Log("ParentTile Hit: " + parent.name);
                    convertMode[0].SelectObjects.Add(parent);
                }
                break;
            case ConvertItem.Objects:
            case ConvertItem.Objects_2:
                if (!convertMode[1].SelectObjects.Contains(parent)) {
                    //Debug.Log("selectObjects Hit: " + parent.name);
                    convertMode[1].SelectObjects.Add(parent);
                }
                break;
            case ConvertItem.Destroy:
                if (!convertMode[2].SelectObjects.Contains(parent)) {
                    //Debug.Log("selectObjects Hit: " + parent.name);
                    convertMode[2].SelectObjects.Add(parent);
                }
                break;
            default:
                break;
        }

    }

    // 플레이어가 스킬로 섹션을 자르면 해당하는 역역 중 같은 선상에 있는 물체 확인
    private bool CheckSkillUsable() {
        if (startSection == finishSection) return false;
        return blockObjects.Count >= 1 ? false : true;
    }

    private void ChangeBlockObjectMaterial() {
        if (convertMode[0].SelectObjects == null) return;
        blockObjects.Clear();

        foreach (GameObject each in convertMode[0].SelectObjects) {
            Vector2 playerXYpos = new Vector2(Control3D.PlayerRigid.position.x, Control3D.PlayerRigid.position.y);
            Vector2 eachXYpos = new Vector2(each.transform.position.x, each.transform.position.y);

            if (playerXYpos.x < (eachXYpos.x + 1) && playerXYpos.x > (eachXYpos.x - 1)) {    // 만약 플레이어와 오브젝트가 같은 x좌표에 있다면 플레이어의 y좌표를 기준으로  +4 이하에 있으면 같은 선상임
                if (eachXYpos.y >= playerXYpos.y - 0.5) {
                    if (eachXYpos.y <= playerXYpos.y + 4) {
                        Debug.Log(" 같은 선상의 오브젝트 이름 | " + each.name);
                        blockObjects.Add(each);
                        each.GetComponentInChildren<TileController>().ChangeMaterial();
                    }
                }
            }
        }

    }



    public override void ExitState() {
        Control3D.Ani3D.SetBool("IsTryUseSkill", false);
    }
}
