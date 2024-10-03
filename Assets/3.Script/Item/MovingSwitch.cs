using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovingSwitch : MonoBehaviour {

    private bool isPushed;
    public int Password;
    private MovingObject movingObject;
    private void Awake() {
        movingObject = GetComponentInParent<MovingObject>();
    }


    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && !isPushed) {
            isPushed = true;
            movingObject.OderToMoveObjects(Password);

            string[] include = { "TileButton" };
            string key = AudioManager.instance.GetDictionaryKey<string, List<AudioClip>>(AudioManager.SFX, include);
            AudioManager.instance.SFX_Play(AudioManager.instance.InGameAudio, key);
        }
    }


}

/*
 1. 플레이어와 부딪히면 타일을 움직여야함
    - 타일은 여러개일 수 있음
 
 */