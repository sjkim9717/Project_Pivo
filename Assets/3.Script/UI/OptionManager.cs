using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager : MonoBehaviour {
    private GameObject mainGroup;

    private void Awake() {
        mainGroup = transform.parent.GetChild(0).gameObject;
    }

    //TODO: â��� ȭ�� ������ ���� �ؼ� ��ư ���� ��� ����Ǿ����
    public void ButtonOnClick_LeftArrow() {

    }
    public void ButtonOnClick_RightArrow() {

    }


    public void ButtonOnClick_Cancle() {
        gameObject.SetActive(false);
        mainGroup.SetActive(true);
    }

    public void ButtonOnClick_Apply() {
        //TODO: â���, ���� �����ϴ� �޼ҵ�
    }


}

/*
 1. UI Title - Option

 2. ����
    - â ����
    - ���� �� �̹��� ��ġ ����
    - ���� ����

    - Apply : ������� ������ Ȱ��ȭ ��������

 3. Button On Click
    - Cancle : Option â �ݰ� Main â ����
    - Apply : Setting ���� ( â ��� / ���� ) //TODO: â���, ���� ���� �ʿ���
    - Arrow : ���� ����Ǿ��� => â ��忡 ���� Arrow�� ������ �ȴ����� �����Ǿ����

4. Button On Enter //TODO: Ȱ��ȭ �ؾ���
    - Cancle 
    - Apply 
 */