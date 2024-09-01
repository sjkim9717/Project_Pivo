using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SkillController : MonoBehaviour {

    private int skillCount = 0;
    private bool isSkillButtonPressed = false;
    public bool IsTryToUseSkill { get; private set; }  // skill 사용하려고 할 경우 섹션 표시 및 사용 가능인지 불가능인지 확인

    private Vector3 startSection = Vector3.zero;
    private Vector3 finishSection = Vector3.zero;

    private GameObject sectionLine;
    private GameObject sectionLine_First;

    private Rigidbody playerRigidbody;

    private PlayerManager playerManager;
    private Player3DController playerController;

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

    //TODO: 스킬 사용 구간과 플레이어가 겹치는지 확인해야함 
    private void Skill() {
        float skillSectionInput = Input.GetAxis("SkillSection");

        if (skillSectionInput != 0 && !isSkillButtonPressed) {                      // 스킬 버튼이 눌렸는지 감지
            isSkillButtonPressed = true;                                            // 버튼이 눌린 상태로 표시
            skillCount++;
            IsTryToUseSkill = true;
            playerManager.IsMovingStop = true;
            Debug.Log("스킬 시도 등록. 현재 스킬 횟수: " + skillCount);

        }

        if(skillCount == 1) {
            GetKeyInput();      // 화살표 섹션 이동
            CancleSkill();      // 취소키
        }


        if (skillCount >= 2) {                                                      // 스킬 사용 시도 횟수가 2회 이상인지 확인
            if (CheckSkillUsable()) {                                               //TODO: [기억] 스킬 사용해서 2D로 변경됨
                playerManager.SetPlayerMode(false);
                playerManager.SwitchMode();
                Debug.Log("2D 모드로 전환됨");
            }
            else {                                                                  // 3D 모드 유지
                playerManager.SetPlayerMode(true);
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


    // 입력을 받아서 일정 범위 결정
    private void GetKeyInput() {
        // 각 화살표 키가 눌렸는지 확인
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.UpArrow)) {
            MoveSectionLine(true);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.DownArrow)) {
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
            return;
        }
    }


    // section line 위치 만들기
    private void MoveSectionLine(bool up) {
        float direction = up ? 1f : -1f;

        EffectOn();

        if (startSection == Vector3.zero) {
            startSection = new Vector3(playerRigidbody.position.x, playerRigidbody.position.y, (int)playerRigidbody.position.z - direction * 1f);
            finishSection = new Vector3(startSection.x, startSection.y, startSection.z + direction * 2f);
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
    }

    //TODO: 플레이어가 스킬 자르면 해당하는 영역을 확인해야함
    private bool CheckSkillUsable() {

        return true;
    }



    // 같은 선상에 있으면 해당 오브젝트 담아야함
}

/*
 1. Player3DController에서 스킬 시전 할 경우를 받아서 
 2. 키보드 입력을 받고
    CLAMP?
 3. 위 아래 섹션 중가 

=> MoveSectionLine 메소드
 startSection 벡터 :현재 위치를 중심으로 z-1,x, y는 고정 
 finishSection 벡터 : z+1,x, y고정에서 출발
            z축을 +2단위로 움직임
단 clamp함수를 사용해서 to벡터의 z가 움직일 수 있는 최대 범위(from - to)는 10까지로 제한 


    1. 해당되는 오브젝트들 다 담아놓고
    2. 플레이어와 z축 비교
    3. 같으면 material 변경 빨간색 표시 
    4. 

 
 */