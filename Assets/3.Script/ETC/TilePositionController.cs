using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePositionController : MonoBehaviour
{

    private PlayerManager playerManager;

    private void Awake() {
        playerManager = FindObjectOfType<PlayerManager>();
    }


    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Player")) {
            playerManager.onPlayerEnterTile.Invoke(transform.position);
        }
    }

}
