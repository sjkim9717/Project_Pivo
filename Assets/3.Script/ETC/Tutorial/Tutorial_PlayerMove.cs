using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_PlayerMove : MonoBehaviour
{
    private float moveSpeed = 7f;
    private bool isPlayerMovable;
    public void SetPlayerMove() { isPlayerMovable = true; }

    private Animator ani2D;
    private GameObject player;
    private Transform playerTransform;

    private void Awake() {
        player = transform.GetChild(1).gameObject;
        playerTransform = player.GetComponent<Transform>();
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
            transform.localScale = new Vector3(moveDirection, 1f, 1f);
            playerTransform.position = Vector3.right * moveSpeed * horizontalInput * Time.deltaTime;

        }

        // Animation
        ani2D.SetBool("IsMove", horizontalInput != 0);
    }

}
