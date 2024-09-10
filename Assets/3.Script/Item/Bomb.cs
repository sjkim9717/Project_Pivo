using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour, IBomb {
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float distance = 1.5f;
    private Rigidbody playerRigid;
    private Rigidbody bombRigid;
    private Vector3 originPos;
    private Vector3 BombToMove;

    private void Awake() {
        bombRigid = GetComponent<Rigidbody>();
        playerRigid = FindObjectOfType<PlayerManage>().PlayerRigid3D;
    }
    private void OnEnable() {
        bombRigid.isKinematic = false;
        bombRigid.constraints = RigidbodyConstraints.FreezeAll;

        BombToMove = bombRigid.position;
        originPos = bombRigid.position;
    }
    private void Update() {

        // 목표 위치로 이동
        if (Vector3.Distance(bombRigid.position, BombToMove) >= 0.1f) {
            bombRigid.MovePosition(Vector3.Lerp(bombRigid.position, BombToMove, Time.deltaTime * moveSpeed));
        }
        // 바닥 밑으로 떨어짐
        if (bombRigid.position.y <= -20f) {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) {
            foreach (Transform item in transform) {
                if (item.TryGetComponent(out Collider collider)) {
                    collider.isTrigger = false;
                }
                else if (item.TryGetComponent(out Collider2D collider2D)) {
                    collider2D.isTrigger = false;
                }
            }

        }
    }

    public void IBombMoveStart() {
        bombRigid.constraints = RigidbodyConstraints.FreezeRotation & RigidbodyConstraints.FreezePositionX & RigidbodyConstraints.FreezePositionZ;
        // Bomb의 목표 위치를 설정 (현재 위치에서 위로 1.5 유닛 이동)
        BombToMove = new Vector3(bombRigid.position.x, bombRigid.position.y + 1.5f, bombRigid.position.z);
        
    }

    public void IBombMoving() {
        // Bomb의 이동을 위해 Y축 고정
        bombRigid.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

        // 플레이어의 회전 축을 기반으로 폭탄의 위치 업데이트
        // 플레이어의 로컬 좌표계에서 폭탄의 상대적인 위치 계산
        Vector3 localBombPosition = playerRigid.rotation * new Vector3(0, 0, distance);

        // 폭탄을 플레이어의 위치에서 로컬 위치만큼 떨어진 곳으로 이동
        Vector3 BombToCalPos = playerRigid.position + localBombPosition;
        BombToCalPos = new Vector3(Mathf.Round(BombToCalPos.x / 2) * 2, BombToCalPos.y, Mathf.Round(BombToCalPos.z / 2) * 2);

        BombToMove = BombToCalPos;
    }


    public void IBombMoveEnd() {
        bombRigid.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        foreach (Transform item in transform) {
            if (item.TryGetComponent(out Collider collider)) {
                collider.isTrigger = true;
            }
            else if(item.TryGetComponent(out Collider2D collider2D)) {
                collider2D.isTrigger = true;
            }
        }

        bombRigid.velocity = new Vector3(0, bombRigid.velocity.y, 0);
    }

    public void IBombExplosion() {
        gameObject.SetActive(false);
    }
    private void OnDisable() {
        bombRigid.velocity = Vector3.zero;
        bombRigid.MovePosition(originPos);
        bombRigid.MoveRotation(Quaternion.identity);
    }
}
