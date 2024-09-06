using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_PlayerMove : MonoBehaviour
{
    [SerializeField]private float moveSpeed = 5f;
    private bool isPlayerMovable;
    public void SetPlayerMove(bool isPlayerMovable) { this.isPlayerMovable = isPlayerMovable; }


    private Animator ani2D;
    private GameObject player;
    private Transform playerTransform;
    private Rigidbody2D playerRigid;

    private void Awake() {
        player = transform.GetChild(1).gameObject;
        playerTransform = player.transform;
        ani2D = player.GetComponent<Animator>();
    }

    private void Update() {
        if (isPlayerMovable) {
            Move();
        }
    }

    private void Move() {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput != 0) {       // 오른쪽 키를 입력받아 2D에서는 앞 뒤로만 이동

            float moveDirection = horizontalInput > 0 ? 1f : -1f;

            playerTransform.localScale = new Vector3(moveDirection, 1f, 1f);
            playerTransform.position = new Vector3(
                playerTransform.position.x + moveSpeed * horizontalInput * Time.deltaTime,
                playerTransform.position.y,
                playerTransform.position.z);
        }

        // Animation
        ani2D.SetBool("IsMove", horizontalInput != 0);
    }

}
