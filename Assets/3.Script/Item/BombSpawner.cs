using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpawner : MonoBehaviour {
    private Vector3 originPos;
    
    [SerializeField] private float colliderRadius = 10f;
    [SerializeField] private GameObject BombPrefab;
    
    private GameObject bomb;
    public GameObject Bomb { get { return bomb; } }

    private PlayerManage playerManage;

    private void Awake() {
        playerManage = FindObjectOfType<PlayerManage>();

        bomb = Instantiate(BombPrefab, transform);
        bomb.SetActive(false);
        bomb.name = BombPrefab.name;

        originPos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
        bomb.transform.position = originPos;
    }

    private void Update() {
        if (!bomb.activeSelf) {
            if (CheckPlayerCloseToBombSpawner()) {
                InitBomb();
            }
        }
    }

    private bool CheckPlayerCloseToBombSpawner() {
        if (playerManage.CurrentMode == PlayerMode.Player3D) {
            Collider[] collAll = Physics.OverlapSphere(transform.position, colliderRadius);

            foreach (Collider each in collAll) {
                if (each.CompareTag("Player")) return true;
            }
        }
        else {
            Collider2D[] collAll = Physics2D.OverlapCircleAll(transform.position, colliderRadius);

            foreach (Collider2D each in collAll) {
                if (each.CompareTag("Player")) return true;
            }
        }

        return false;
    }
    private void OnDisable() {
        Debug.Log($" spawner false? | {gameObject.activeSelf}");
    }
    private void InitBomb() {
        originPos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
        bomb.transform.position = originPos;
        bomb.SetActive(true);
    }

}
