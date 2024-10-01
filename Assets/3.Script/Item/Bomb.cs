using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour, IBomb {
    private float t = 0f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float distance = 2f;
    [SerializeField] private float height = 2.5f;
    private int activeFalseLayerIndex;
    private int layerMask;

    private bool IsMoveEnd;
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
        activeFalseLayerIndex = LayerMask.NameToLayer("ActiveFalse");
        layerMask = 1 << LayerMask.NameToLayer("ActiveTrue");
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

    private void Update() {
        if (gameObject.layer == activeFalseLayerIndex) {
            bombRigid.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private void FixedUpdate() {
        // 바닥 밑으로 떨어짐
        if (bombRigid.position.y <= -10f) {
            gameObject.SetActive(false);
            UIBomb.SetActive(false);    
        }

        if (IsMoveEnd) return;
        
        // 목표 위치로 이동
        if (Vector3.Distance(bombRigid.position, BombToMove) >= 0.1f) {
            t += moveSpeed * Time.fixedDeltaTime;
            t = Mathf.Clamp01(t);
            bombRigid.MovePosition(Vector3.Lerp(bombRigid.position, BombToMove, t));
        }
        else {
            bombRigid.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public void IBombMoveStart() {
        IsMoveEnd = false;
        bombRigid.isKinematic = true;
        bombRigid.useGravity = false;
        foreach (var each in bombRigid.GetComponentsInChildren<Collider>()) each.isTrigger = true;

        UIBomb.SetActive(true);
        bombRigid.constraints = RigidbodyConstraints.FreezeRotation & RigidbodyConstraints.FreezePositionX & RigidbodyConstraints.FreezePositionZ;

        // Bomb의 목표 위치를 설정 (현재 위치에서 플레이어 위로 2f 이동)
        BombToMove = new Vector3(bombRigid.position.x, playerRigid.position.y + height, bombRigid.position.z);
    }

    public void IBombMoving() {
        t = 0f;

        bombRigid.isKinematic = false;
        bombRigid.useGravity = false;

        // 플레이어의 앞쪽 방향 확인 후 해당 방향에 distance 더해서 움직여야한는 위치 설정
        Vector3 direction = playerRigid.transform.forward;
        Vector3 BombToCalPos = playerRigid.position + direction * distance;

        BombToMove = SetPosition(ref BombToCalPos);
    }

    private Vector3 SetPosition(ref Vector3 BombToCalPos) {
        float xFloor = Mathf.Floor(BombToCalPos.x * 10) / 10;
        float zFloor = Mathf.Floor(BombToCalPos.z * 10) / 10;

        float yFloor = Mathf.RoundToInt(playerRigid.position.y);
         yFloor = Mathf.Floor(yFloor + height);

        Vector3 _calPos = new Vector3(Mathf.RoundToInt(xFloor * 0.5f) * 2, yFloor, Mathf.RoundToInt(zFloor * 0.5f) * 2);
        BombToCalPos = _calPos;
        return BombToCalPos;
    }
    public void IBombMoveEnd() {
        bombRigid.isKinematic = false;
        bombRigid.useGravity = true;
        foreach (var each in bombRigid.GetComponentsInChildren<Collider>()) each.isTrigger = false;

        IsMoveEnd = true;
        bombRigid.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        bombRigid.velocity = new Vector3(0, bombRigid.velocity.y, 0);
    }

    public void IBombExplosion() {
        UIBomb.SetActive(false);
        if (playerManage.CurrentMode == PlayerMode.Player3D) {
            Vector3 boxSize = new Vector3(2f, 2f, 2f);
            RaycastHit[] hits = Physics.BoxCastAll(transform.position, boxSize, transform.up, Quaternion.identity, 0, layerMask);

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
            RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, boxSize, 0, transform.up, 0, layerMask);

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
