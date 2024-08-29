using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile3DController : MonoBehaviour
{

    private PlayerManager playerManager;

    private void Awake() {
        playerManager = GetComponent<PlayerManager>();
    }


    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Player")) {
            playerManager.onPlayerEnterTile.Invoke(transform.position);
        }
    }



}
