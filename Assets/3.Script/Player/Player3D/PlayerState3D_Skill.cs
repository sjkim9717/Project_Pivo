using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerState3D_Skill : PlayerState3D {
    [SerializeField] private float skillDistance = 14f;
    private int skillCount = 0;

    private bool isSkillButtonPressed = false;

    private Vector3 startSection = Vector3.zero;
    private Vector3 finishSection = Vector3.zero;

    private GameObject sectionLine;
    private GameObject sectionLine_First;

    private ConvertMode[] convertMode;
    protected override void OnEnable() {
        base.OnEnable();
        startSection = Vector3.zero;
        finishSection = Vector3.zero;
        isSkillButtonPressed = false;
        skillCount = 0;

        playerManage.StartSection = Vector3.zero;
        playerManage.FinishSection = Vector3.zero;
    }

    private void Start() {

        sectionLine = transform.parent.GetChild(4).gameObject;
        sectionLine_First = Instantiate(sectionLine, sectionLine.transform.parent);
        sectionLine_First.name = "SectionLine_First";

        // 색상 변경
        var renderer = sectionLine_First.GetComponent<Renderer>();
        if (renderer != null) {
            renderer.material.color = Color.red; // 빨간색으로 설정
        }

        convertMode = new ConvertMode[FindObjectsOfType<ConvertMode>().Length];

        convertMode[0] = FindObjectOfType<ConvertMode_Tile>();
        convertMode[1] = FindObjectOfType<ConvertMode_Object>();
        convertMode[2] = FindObjectOfType<ConvertMode_Destroy>();
        convertMode[3] = FindObjectOfType<ConvertMode_Item>();
        convertMode[4] = FindObjectOfType<ConvertMode_MoveTile>();

    }

    public override void EnterState() {
        Control3D.Ani3D.SetBool("IsTryUseSkill", true);
        AudioManager.instance.SFX_Play(playerManage.PlayerAudio, "ViewChange_ChangStart");
    }
    private void Update() {
        skillSectionInput = Input.GetAxis("SkillSection");
        interactionInput = Input.GetAxisRaw("Interaction");

        ChangeState();
    }

    private void ChangeState() {
        if (playerManage.CurrentState == PlayerState.Dead) {
            return;
        }

        Control3D.Move(0, 0);

        if (skillSectionInput != 0 && !isSkillButtonPressed) { // 스킬 버튼이 눌렸는지 감지
            isSkillButtonPressed = true;                                            // 버튼이 눌린 상태로 표시
            skillCount++;
            Debug.Log("스킬 시도 등록. 현재 스킬 횟수: " + skillCount);
        }


        if (skillCount == 1) {

            // 각 화살표 키가 눌렸는지 확인
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow)) {  // 막힌 오브젝트의 위치를 비교함
                if (!IsMoveNotAllowed()) {
                    AudioManager.instance.SFX_Play(playerManage.PlayerAudio, "ViewChange_Cast");
                    MoveSectionLine(true);
                }
                else {
                    string[] include = { "ViewChange", "Block" };
                    string key = AudioManager.instance.GetDictionaryKey<string, List<AudioClip>>(AudioManager.SFX, include);
                    AudioManager.instance.SFX_Play(playerManage.PlayerAudio, key);
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.DownArrow)) {
                if (!IsMoveNotAllowed()) {
                    AudioManager.instance.SFX_Play(playerManage.PlayerAudio, "ViewChange_Cast");
                    MoveSectionLine(false);
                }
                else {
                    string[] include = { "ViewChange", "Block" };
                    string key = AudioManager.instance.GetDictionaryKey<string, List<AudioClip>>(AudioManager.SFX, include);
                    AudioManager.instance.SFX_Play(playerManage.PlayerAudio, key);
                }
            }
        }


        if (skillCount >= 2) {                                                      // 스킬 사용 시도 횟수가 2회 이상인지 확인

            if (CheckSkillUsable()) {                                               //TODO: [기억] 스킬 사용해서 2D로 변경됨

                playerManage.StartSection = startSection;                  //TODO: [기억] 몬스터 mode switch시에 사용 
                playerManage.FinishSection = finishSection;

                playerManage.CurrentMode = PlayerMode.Player2D;
                playerManage.SwitchMode();
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

            AudioManager.instance.SFX_Play(playerManage.PlayerAudio, "ViewChange_ChangeEnd");
        }


        if (skillSectionInput == 0 && isSkillButtonPressed) {                           // 스킬 버튼이 해제되었는지 감지
            isSkillButtonPressed = false;                                               // 버튼 눌림 상태를 초기화
        }


        if (interactionInput != 0) {       // cancle
            skillCount = 0;
            ResetSelectObject();
            sectionLine.SetActive(false);
            sectionLine_First.SetActive(false);

            playerManage.IsChangingModeTo3D = false;
            Control3D.ChangeState(PlayerState.Idle);
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) {

            Control3D.ChangeState(PlayerState.Disable);
        }

    }
    private bool IsMoveNotAllowed() {
        return convertMode.Any(item => item.blockObjects.Count > 0);
    }



    private void MoveSectionLine(bool up) {
        float direction = up ? 1f : -1f;

        sectionLine.SetActive(true);
        sectionLine_First.SetActive(true);

        if (startSection == Vector3.zero) {
            // 홀수 단위로 z좌표를 설정해야함            
            float zPosition = SetZposition(ref direction);
            startSection = new Vector3(Control3D.PlayerRigid.position.x, Control3D.PlayerRigid.position.y, zPosition);
            finishSection = new Vector3(startSection.x, startSection.y, zPosition + direction * 2f);
        }
        else {
            float movingAmount = direction * 2f;

            // finishSection의 z 값 이동 및 클램프 적용
            finishSection.z += movingAmount;
            finishSection.z = Mathf.Clamp(finishSection.z, startSection.z - skillDistance, startSection.z + skillDistance);

            if (finishSection == startSection) { // 홀수 단위로 z좌표를 설정해야함
                float zPosition = Mathf.FloorToInt(finishSection.z - direction * 2);
                startSection = new Vector3(Control3D.PlayerRigid.position.x, Control3D.PlayerRigid.position.y, zPosition);
            }
        }


        sectionLine.transform.position = finishSection;
        sectionLine_First.transform.position = startSection;

        ResetSelectObject();                                                    // raycast가 움직이기전 초기화

        playerManage.StartSection = startSection;                  //TODO: [기억] 몬스터 mode switch시에 사용 
        playerManage.FinishSection = finishSection;

        ChangeSelectObjectLayer(startSection, finishSection);                   // Raycast를 넣어서  

        ChangeBlockObjectMaterial(convertMode[0]);
        ChangeBlockObjectMaterial(convertMode[2]);
        ChangeBlockObjectMaterial(convertMode[3]);
    }

   
    private float SetZposition(ref float direction) {
        float zFloor = Mathf.Floor(Control3D.PlayerRigid.position.z * 10) / 10;      // 소숫점 2자리 이하 버림
        float zPosition = Mathf.RoundToInt(zFloor);
        if (zPosition % 2 == 0) {// 짝수        // 가장 가까운 홀수 두 개와 거리비교
            float lowerOdd = zPosition - 1; // 아래 홀수
            float upperOdd = zPosition + 1; // 위 홀수

            // 두 홀수와의 거리 비교
            if (Mathf.Abs(lowerOdd - zFloor) < Mathf.Abs(upperOdd - zFloor)) {
                zPosition = lowerOdd; // 아래 홀수가 더 가까움
                direction = 1f;
            }
            else {
                zPosition = upperOdd; // 위 홀수가 더 가까움
                direction = -1f;
            }
        }
        else {
            direction = 1f;
        }
        return zPosition;
    }



    // convertMode가 여러개 있을 경우 전체 담아서 초기화
    public void ResetSelectObject() {
        if (convertMode[0].SelectObjects == null) return;
        if (convertMode[2].SelectObjects == null) return;
        if (convertMode[3].SelectObjects == null) return;

        ResetBlock(convertMode[0]);
        ResetBlock(convertMode[2]);
        ResetBlock(convertMode[3]);

        foreach (ConvertMode item in convertMode) {
            item.ChangeLayerAllActiveTrue();
            item.SelectObjects.Clear();
        }

        playerManage.StartSection = Vector3.zero;
        playerManage.FinishSection = Vector3.zero;
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
                AddSelectObjectsWithTag(tagEnum, hit.collider.gameObject);
            }
        }
    }

    private void AddSelectObjectsWithTag(ConvertItem tagName, GameObject parent) {

        switch (tagName) {
            case ConvertItem.ParentTile:
                convertMode[0].AddSelectObjects(parent);
                break;
            case ConvertItem.Objects:
                convertMode[1].AddSelectObjects(parent);
                break;
            case ConvertItem.Destroy:
                convertMode[2].AddSelectObjects(parent);
                break;
            case ConvertItem.Objects_2:
                convertMode[3].AddSelectObjects(parent);
                break;
            case ConvertItem.MoveTile:
                convertMode[4].AddSelectObjects(parent);
                break;
            default:
                break;
        }

    }

    // 플레이어가 스킬로 섹션을 자르면 해당하는 역역 중 같은 선상에 있는 물체 확인
    private bool CheckSkillUsable() {
        if (startSection == finishSection) return false;
        foreach (ConvertMode each in convertMode) {
            if (each.blockObjects.Count >= 1) return false;
        }
        return true;
    }


    private void ChangeBlockObjectMaterial(ConvertMode convertMode) {
        if (convertMode.SelectObjects == null) return;

        ResetBlock(convertMode);

        foreach (GameObject each in convertMode.SelectObjects) {
            Vector2 playerXYpos = new Vector2(Control3D.PlayerRigid.position.x, Control3D.PlayerRigid.position.y);
            Vector2 eachXYpos = new Vector2(each.transform.position.x, each.transform.position.y);

            if (playerXYpos.x < (eachXYpos.x + 1) && playerXYpos.x > (eachXYpos.x - 1)) {    // 만약 플레이어와 오브젝트가 같은 x좌표에 있다면 플레이어의 y좌표를 기준으로  +4 이하에 있으면 같은 선상임
                if (eachXYpos.y >= playerXYpos.y - 0.5) {
                    if (eachXYpos.y <= playerXYpos.y + 4) {
                        Debug.Log(" 같은 선상의 오브젝트 이름 | " + each.name);
                        convertMode.AddBlockObject(each);
                    }
                }
            }
        }

        convertMode.ChangeMaterial_Block();
    }

    private void ResetBlock(ConvertMode convertMode) {
        convertMode.ChangeMaterial_Origin();
        convertMode.ClearBlockObject();
    }

    public override void ExitState() {

        sectionLine.SetActive(false);
        sectionLine_First.SetActive(false);

        Control3D.Ani3D.SetBool("IsTryUseSkill", false);
    }
}
