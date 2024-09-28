using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour, IBomb {
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float distance = 1.5f;
    [SerializeField] private float height = 2.5f;
    private Rigidbody playerRigid;
    private Rigidbody bombRigid;
    private Vector3 originPos;
    private Vector3 BombToMove;

    [SerializeField] private GameObject EffectPrefab;
    private GameObject effect;

    private GameObject UIBomb;

    private PlayerManage playerManage;
    private DynamicManager UI_Dynamic;

    private ConvertMode_Destroy destroyComponent;

    private void Awake() {
        bombRigid = GetComponent<Rigidbody>();

        UI_Dynamic = FindObjectOfType<DynamicManager>();
        destroyComponent = FindObjectOfType<ConvertMode_Destroy>();

        playerManage = FindObjectOfType<PlayerManage>();
        playerRigid = playerManage.PlayerRigid3D;

        effect = Instantiate(EffectPrefab, transform);
        effect.SetActive(false);
    }

    private void Start() {
        UIBomb = UI_Dynamic.BombGroup;
    }
    private void OnEnable() {
        bombRigid.constraints = RigidbodyConstraints.FreezeAll;

        BombToMove = bombRigid.position;
        Vector3 initPos = new Vector3(transform.parent.position.x + transform.parent.position.y + 2, transform.parent.position.z);
        originPos = initPos;
    }

    private void FixedUpdate() {
        // 바닥 밑으로 떨어짐
        if (bombRigid.position.y <= -20f) {
            gameObject.SetActive(false);
        }
        if (testEndFlag) return;
        // 목표 위치로 이동
        if (Vector3.Distance(bombRigid.position, BombToMove) >= 0.1f) {
            bombRigid.MovePosition(Vector3.Lerp(bombRigid.position, BombToMove, Time.fixedDeltaTime * moveSpeed));
        }
    }

    public void IBombMoveStart() {
        testEndFlag = false;
        bombRigid.isKinematic = true;
        bombRigid.useGravity = false;
        foreach (var each in bombRigid.GetComponentsInChildren<Collider>()) each.isTrigger = true;

        UIBomb.SetActive(true);
        bombRigid.constraints = RigidbodyConstraints.FreezeRotation & RigidbodyConstraints.FreezePositionX & RigidbodyConstraints.FreezePositionZ;
        // Bomb의 목표 위치를 설정 (현재 위치에서 위로 1.5 유닛 이동)
        BombToMove = new Vector3(bombRigid.position.x, playerRigid.position.y + height, bombRigid.position.z);

    }

    public void IBombMoving() {
        // Bomb의 이동을 위해 Y축 고정
        bombRigid.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

        // 플레이어의 회전 축을 기반으로 폭탄의 위치 업데이트
        // 플레이어의 로컬 좌표계에서 폭탄의 상대적인 위치 계산
        Vector3 localBombPosition = playerRigid.transform.forward;
        localBombPosition = localBombPosition + new Vector3(0, 0, distance);

        // 폭탄을 플레이어의 위치에서 로컬 위치만큼 떨어진 곳으로 이동
        Vector3 BombToCalPos = playerRigid.position + localBombPosition;
        BombToCalPos = new Vector3(Mathf.Round(BombToCalPos.x / 2) * 2, BombToCalPos.y, Mathf.Round(BombToCalPos.z / 2) * 2);

        BombToMove = BombToCalPos;
    }

    private bool testEndFlag;
    public void IBombMoveEnd() {
        bombRigid.isKinematic = false;
        bombRigid.useGravity = true;
        foreach (var each in bombRigid.GetComponentsInChildren<Collider>()) each.isTrigger = false;


        testEndFlag = true;
        bombRigid.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

        bombRigid.velocity = new Vector3(0, bombRigid.velocity.y, 0);
    }

    public void IBombExplosion() {
        UIBomb.SetActive(false);
        if (playerManage.CurrentMode == PlayerMode.Player3D) {
            Vector3 boxSize = new Vector3(2f, 2f, 2f);
            RaycastHit[] hits = Physics.BoxCastAll(transform.position, boxSize, transform.up, Quaternion.identity, 0);

            foreach (RaycastHit item in hits) {
                if (!item.collider.name.Contains("Root3D")) {
                    if (item.collider.CompareTag("Destroy")) {
                        Debug.LogWarning("IBombExplosion | " + item.collider.name);

                        item.collider.gameObject.SetActive(false);
                        destroyComponent.DeleteDestroiedObject(item.collider.gameObject);
                    }
                }

            }
        }
        else if (playerManage.CurrentMode == PlayerMode.Player2D) {
            Vector2 boxSize = new Vector3(4f, 4f, 2f);
            RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, boxSize, 0, transform.up, 0);

            Debug.Log(" 2D 모드 hits length | " + hits.Length);
            foreach (RaycastHit2D item in hits) {
                if (item.transform.parent.CompareTag("Destroy")) {
                    Debug.Log(" 2D 모드 Destroy item | " + item.transform.parent.name);
                    item.transform.parent.gameObject.SetActive(false);
                    destroyComponent.DeleteDestroiedObject(item.transform.parent.gameObject);
                }
            }
        }

        effect.transform.position = transform.position;
        effect.SetActive(true);
        StartCoroutine(EffectTimeDelay());
    }

    private IEnumerator EffectTimeDelay() {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }



    private void OnCollisionStay(Collision collision) {
        if (collision.transform.parent != null) {
            if (collision.transform.parent.parent != null) {
                Transform parent = collision.transform.parent.parent;

                if (parent.CompareTag("PushBox")) {
                    Vector3 pos = parent.GetComponent<Transform>().position;
                    bombRigid.MovePosition(new Vector3(pos.x, bombRigid.position.y, pos.z));
                }
            }
        }

    }


    private void OnDisable() {
        bombRigid.isKinematic = true;
        effect.SetActive(false);
        bombRigid.velocity = Vector3.zero;
        bombRigid.MovePosition(originPos);
        bombRigid.MoveRotation(Quaternion.identity);
    }

    private void OnDrawGizmos() {
        // Draw a Gizmo representation of the box size
        Gizmos.color = Color.grey;
        Vector3 boxSize = new Vector3(2f, 2f, 2f);

        if (playerManage != null && playerManage.CurrentMode == PlayerMode.Player3D) {
            Gizmos.DrawWireCube(transform.position, boxSize * 2);
        }
        else {
            Gizmos.DrawWireCube(transform.position, new Vector3(boxSize.x * 2, boxSize.y * 2, 0.1f));
        }
    }

}
