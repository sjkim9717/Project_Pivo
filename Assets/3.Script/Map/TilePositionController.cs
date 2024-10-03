using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePositionController : MonoBehaviour {

    private PlayerManage playerManager;

    private Vector3 tilePosition;

    private void Awake() {
       playerManager = FindObjectOfType<PlayerManage>();

        tilePosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
    }


    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Player")) {
            playerManager.onPlayerEnterTile.Invoke(tilePosition);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag("Player")) {
            playerManager.onPlayerEnterTile.Invoke(tilePosition);
        }
    }

}
