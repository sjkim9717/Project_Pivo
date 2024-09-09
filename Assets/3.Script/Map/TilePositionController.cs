using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePositionController : MonoBehaviour
{

    private PlayerManager playerManager;
    private PlayerManage playerManage;

    private Vector3 tilePosition;

    private void Awake() {
        playerManager = FindObjectOfType<PlayerManager>();
        if(playerManager == null) playerManage = FindObjectOfType<PlayerManage>();

        tilePosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
    }


    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Player")) {
            if (playerManager == null) {
                playerManage.onPlayerEnterTile.Invoke(tilePosition);
            }
            else {
                playerManager.onPlayerEnterTile.Invoke(tilePosition);
            }
        }
    }

}
