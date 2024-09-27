using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpawner : MonoBehaviour {
    [SerializeField] private float colliderRadius = 10f;
    [SerializeField] private GameObject BombPrefab;
    public GameObject Bomb { get { return bomb; } }
    private GameObject bomb;
    private Vector3 originPos;

    private void Awake() {
        bomb = Instantiate(BombPrefab, transform);
        bomb.SetActive(false);
        bomb.name = BombPrefab.name;
        //TODO: bomb 생성 위치 변경할 것
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
        if (PlayerManage.instance.CurrentMode == PlayerMode.Player3D) {
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

    private void InitBomb() {
        originPos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
        bomb.transform.position = originPos;
        bomb.SetActive(true);
    }

}
