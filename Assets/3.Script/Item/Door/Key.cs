using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private int password;
    private bool isTouched;
    private bool isBlueColor;
    public void SetPassword(int _password) { password = _password; }
    public void SetColor(bool _isBlueColor) { isBlueColor = _isBlueColor; }

    private GateManage gateManage;
    private ConvertMode_Object convertMode_Item;
    private void Awake() {
        gateManage = FindObjectOfType<GateManage>();
        convertMode_Item = FindObjectOfType<ConvertMode_Object>();
        Debug.Log(" color check | key | " + isBlueColor);
    }
    private void Start() {
        SettingColor(isBlueColor);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (!isTouched) {
                isTouched = true;  
                gateManage.FindDoor(password);
                StartCoroutine(DeleteTimeDelay());
                convertMode_Item.DeleteDestroiedObject(transform.parent.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            if (!isTouched) {
                isTouched = true;
                gateManage.FindDoor(password);
                StartCoroutine(DeleteTimeDelay());
                convertMode_Item.DeleteDestroiedObject(transform.parent.gameObject);
            }
        }
    }

    private IEnumerator DeleteTimeDelay() {
        yield return new WaitForSeconds(.5f);
        transform.parent.gameObject.SetActive(false);
    }


    public void SettingColor(bool isblueColor) {

        Color targetColor = isblueColor ? Color.cyan : Color.red;

        if(transform.TryGetComponent(out Renderer renderer)) {
            renderer.material.color = targetColor;
        }

        if (transform.parent.TryGetComponent(out Renderer parentrenderer)) {
            parentrenderer.material.color = targetColor;
        }
    }

}

/*
 1. 키가 플레이어와 닿았다면
 2. 매니저에 신호를 주기
 
 */