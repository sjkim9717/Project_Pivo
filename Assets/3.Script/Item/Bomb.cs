using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour, IBomb {
    [SerializeField] private float moveSpeed = 10f;
    private Rigidbody playerRigid;
    private Rigidbody bombRigid;
    private Vector3 originPos;
    private Vector3 BombToMove;

    private void Awake() {
        bombRigid = GetComponent<Rigidbody>();
        playerRigid = FindObjectOfType<PlayerManage>().PlayerRigid3D;
    }
    private void OnEnable() {
        BombToMove = bombRigid.position;
        originPos = bombRigid.position;
    }
    private void Update() {
        if (Vector3.Distance(bombRigid.position, BombToMove) >= 0.1f) {
            bombRigid.MovePosition(Vector3.Lerp(bombRigid.position, BombToMove, Time.deltaTime * moveSpeed));
            Debug.Log("Update | BombToMove | " + BombToMove);
            Debug.Log("Update | bombRigid | " + bombRigid.position);
        }

    }

    public void IBombMoveStart() {
        bombRigid.constraints = RigidbodyConstraints.FreezeRotation & RigidbodyConstraints.FreezePositionX & RigidbodyConstraints.FreezePositionZ;
        Debug.Log("IBombMoveStart");
        // Bomb의 목표 위치를 설정 (현재 위치에서 위로 1.5 유닛 이동)
        BombToMove = new Vector3(bombRigid.position.x, bombRigid.position.y + 1.5f, bombRigid.position.z);
        Debug.Log("IBombMoveStart | BombToMove | " + BombToMove);
        Debug.Log("IBombMoveStart | bombRigid | " + bombRigid.position);
    }

    public void IBombMoving() {
        // Bomb의 이동을 위해 Y축 고정
        bombRigid.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

        Vector3 direction = bombRigid.position - playerRigid.position;
        float angle = Vector3.Angle(direction, playerRigid.gameObject.transform.forward);
        Vector3 BombToCalPos = Vector3.zero;

        // bomb의 위치가 플레이어의 앞방향이 아닐경우 움직여야함
        if (angle >30 || angle <-30) {

            // BOMB의 position의 x와 z 좌표가 플레이어와 1이상 차이 날경우만 int값 2로 옮겨야함
            float xDistance = bombRigid.position.x - playerRigid.position.x;
            float zDistance = bombRigid.position.z - playerRigid.position.z;

            if (Mathf.Abs(xDistance) > 1f || Mathf.Abs(zDistance) > 1f) {

                // 플레이어의 이동 방향을 기반으로 Bomb의 목표 위치를 계산
                Vector3 playerDirection = new Vector3(xDistance, 0, zDistance).normalized;
                BombToCalPos = bombRigid.position + playerDirection * 2;

                if (Vector3.Distance(bombRigid.position, BombToMove) <= 0.1f) {
                    BombToMove = BombToCalPos;
                }
            }
        }

    }


    public void IBombMoveEnd() {
        bombRigid.constraints = RigidbodyConstraints.FreezeRotation & RigidbodyConstraints.FreezePositionX & RigidbodyConstraints.FreezePositionZ;
    }

    public void IBombExplosion() {
        gameObject.SetActive(false);
    }
    private void OnDisable() {
        bombRigid.position = originPos;
    }
}
