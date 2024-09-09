using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePositionController : MonoBehaviour
{

    private PlayerManager playerManager;
    private PlayerManage playerManage;

    private void Awake() {
        playerManager = FindObjectOfType<PlayerManager>();
        if(playerManager == null) playerManage = FindObjectOfType<PlayerManage>();
    }


    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Player")) {
            if (playerManager == null) {
                playerManage.onPlayerEnterTile.Invoke(transform.position);
            }
            else {
                playerManager.onPlayerEnterTile.Invoke(transform.position);
            }
        }
    }

}
