using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkillController : MonoBehaviour {

    private int skillCount = 0;
    private int blockZposCheck = 0;

    private bool isSkillButtonPressed = false;

    public bool IsTryToUseSkill { get; private set; }  // skill 사용하려고 할 경우 섹션 표시 및 사용 가능인지 불가능인지 확인

    private Vector3 startSection = Vector3.zero;
    private Vector3 finishSection = Vector3.zero;

    private GameObject sectionLine;
    private GameObject sectionLine_First;
    [SerializeField]
    private List<GameObject>[] selectObjects;              // skill 사용하면 해당 구역의 오브젝트들이 전체 담김
    private List<GameObject> blockObjects = new List<GameObject>();              // skill 사용하면 해당 구역의 플레이어와 겹친 오브젝트가 담김

    private Rigidbody playerRigidbody;

    private PlayerManager playerManager;
    private Player3DController playerController;
    private ConvertMode[] convertMode;

    private Animator ani3D;

    private void Awake() {
        playerRigidbody = GetComponent<Rigidbody>();

        playerManager = GetComponentInParent<PlayerManager>();
        playerController = GetComponent<Player3DController>();

        ani3D = GetComponentInChildren<Animator>();

        sectionLine = transform.parent.GetChild(4).gameObject;
        sectionLine_First = Instantiate(sectionLine, sectionLine.transform.parent);
        sectionLine_First.name = "SectionLine_First";
    }

    private void Start() {
        convertMode = new ConvertMode[FindObjectsOfType<ConvertMode>().Length];

        convertMode[0] = FindObjectOfType<ConvertMode_Tile>();
        convertMode[1] = FindObjectOfType<ConvertMode_Item>();

        selectObjects = new List<GameObject>[convertMode.Length];
        for (int i = 0; i < selectObjects.Length; i++) {
            selectObjects[i] = new List<GameObject>();
        }
    }

    private void Update() {

        if (!playerController.IsMove && !playerController.IsClimb) {
            Skill();
        }


        if (!IsTryToUseSkill) {
            startSection = Vector3.zero;
            finishSection = Vector3.zero;
            sectionLine.transform.position = finishSection;
            sectionLine_First.transform.position = startSection;
        }
    }


    private void Skill() {
        float skillSectionInput = Input.GetAxis("SkillSection");

        if (skillSectionInput != 0 && !isSkillButtonPressed) {                      // 스킬 버튼이 눌렸는지 감지
            isSkillButtonPressed = true;                                            // 버튼이 눌린 상태로 표시
            skillCount++;
            IsTryToUseSkill = true;
            playerManager.IsMovingStop = true;
            Debug.Log("스킬 시도 등록. 현재 스킬 횟수: " + skillCount);

        }

        if (skillCount == 1) {
            CheckBlockObjectZPosition();                                            // 막힌 오브젝트의 위치를 비교함

            GetKeyInput();      // 화살표 섹션 이동
            CancleSkill();      // 취소키
        }


        if (skillCount >= 2) {                                                      // 스킬 사용 시도 횟수가 2회 이상인지 확인
            if (CheckSkillUsable()) {                                               //TODO: [기억] 스킬 사용해서 2D로 변경됨

                for (int i = 0; i < selectObjects.Length; i++) {
                    convertMode[i].ChangeLayerActiveFalse(selectObjects[i]);
                }

                playerManager.SetPlayerMode(false);
                playerManager.SwitchMode();
                Debug.Log("2D 모드로 전환됨");
            }
            else {
                ResetSelectObject();                                               // 3D 모드 유지
                skillCount = 1;
                return;
            }
            skillCount = 0;                                                         // 스킬 시도 후 시도 횟수 초기화
            IsTryToUseSkill = false;
            playerManager.IsMovingStop = false;
            EffectOff();
        }

        ani3D.SetBool("IsTryUseSkill", IsTryToUseSkill);                            // 애니메이션 상태 처리

        if (skillSectionInput == 0 && isSkillButtonPressed) {                           // 스킬 버튼이 해제되었는지 감지
            isSkillButtonPressed = false;                                               // 버튼 눌림 상태를 초기화
        }
    }

    private void EffectOn() {
        sectionLine.SetActive(true);
        sectionLine_First.SetActive(true);
    }

    private void EffectOff() {
        sectionLine.SetActive(false);
        sectionLine_First.SetActive(false);
    }

    // convertMode가 여러개 있을 경우 전체 담아서 초기화
    public void ResetSelectObject() {
        if (selectObjects[0] == null) return;


        foreach (var item in selectObjects[0]) {
            item.GetComponentInChildren<TileController>().InitMaterial();
        }

        blockObjects.Clear();

        for (int i = 0; i < convertMode.Length; i++) {
            selectObjects[i].Clear();
        }

        foreach (ConvertMode item in convertMode) {
            item.ChangeLayerAllActiveTrue();
        }
    }

    // 입력을 받아서 일정 범위 결정
    private void GetKeyInput() {
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

    private void CancleSkill() {
        float CancleInput = Input.GetAxis("Climb");

        if (CancleInput != 0) {       // x 키 취소
            skillCount = 0;
            IsTryToUseSkill = false;
            playerManager.SetPlayerMode(true);
            playerManager.IsMovingStop = false;
            playerManager.isChangingModeTo3D = false;
            EffectOff();
            ResetSelectObject();
            return;
        }
    }


    // section line 위치 만들기
    private void MoveSectionLine(bool up) {
        float direction = up ? 1f : -1f;

        EffectOn();

        if (startSection == Vector3.zero) {
            startSection = new Vector3(playerRigidbody.position.x, playerRigidbody.position.y, (int)playerRigidbody.position.z - 1f);
            finishSection = new Vector3(startSection.x, startSection.y, startSection.z - direction * 2f);
        }
        else {
            float movingAmount = direction * 2f;

            // finishSection의 z 값 이동 및 클램프 적용
            finishSection.z += movingAmount;
            finishSection.z = Mathf.Clamp(finishSection.z, startSection.z - 10f, startSection.z + 10f);

            if (finishSection == startSection) {
                startSection = new Vector3(playerRigidbody.position.x, playerRigidbody.position.y, (int)playerRigidbody.position.z - direction * 1f);
                finishSection = new Vector3(startSection.x, startSection.y, startSection.z + direction * 2f);
            }
        }


        sectionLine.transform.position = finishSection;
        sectionLine_First.transform.position = startSection;


        ResetSelectObject();                                                    // raycast가 움직이기전 초기화
        ChangeSelectObjectLayer(startSection, finishSection);                   // Raycast를 넣어서  
        ChangeBlockObjectMaterial();
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
            if (hit.transform.parent != null) {
                if (hit.transform.parent.parent != null) {
                    string tagName = hit.transform.parent.parent.tag;

                    if(Enum.TryParse(tagName, out Tag tagEnum)) {
                        
                        GameObject parent = hit.transform.parent.gameObject;
                        AddSelectObjectsWithTag(tagEnum, parent);
                    }
                    
                }
            }
        }
    }

    private void AddSelectObjectsWithTag(Tag tagName, GameObject parent) {
        switch (tagName) {
            case Tag.ParentTile:
                if (!selectObjects[0].Contains(parent)) {
                    //Debug.Log("ParentTile Hit: " + parent.name);
                    selectObjects[0].Add(parent);
                }
                break;
            case Tag.Triggers:
            case Tag.Biscuit:
            case Tag.Objects:
            case Tag.Puzzle:
            case Tag.Tree:
                if (!selectObjects[1].Contains(parent)) {
                    //Debug.Log("selectObjects Hit: " + parent.name);
                    selectObjects[1].Add(parent);
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
        if (selectObjects == null) return;
        blockObjects.Clear();

        foreach (GameObject each in selectObjects[0]) {
            Vector2 playerXYpos = new Vector2(playerRigidbody.position.x, playerRigidbody.position.y);
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

    // blockObjects갯수가 0이상일경우에 오브젝트의 z위치가 플레이어의 위치보다 위에 있는지 아래에 있는지 판별
    private void CheckBlockObjectZPosition() {
        if (blockObjects.Count >= 1) {
            if (blockObjects[0].transform.position.z >= playerRigidbody.position.z) blockZposCheck = 1;
            else blockZposCheck = -1;
        }
        else {
            blockZposCheck = 0;
        }
    }

}

/*
 1. Player3DController에서 스킬 시전 할 경우를 받아서 
 2. 키보드 입력을 받고
    CLAMP?
 3. 위 아래 섹션 중가 

=> MoveSectionLine 메소드
 startSection 벡터 :현재 위치를 중심으로 z-1,x, y는 고정 
 finishSection 벡터 : z+1,x, y고정에서 출발 
단 clamp함수를 사용해서 to벡터의 z가 움직일 수 있는 최대 범위(from - to)는 10까지로 제한 


    1. 해당되는 오브젝트들 다 담아놓고
    2. 플레이어와 z축 비교
    3. 같으면 material 변경 빨간색 표시 
    4. 

 
 */