using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] Camera cam;

    RaycastHit hitInfo; //�������� ���� �繰�� ������ ����

    [SerializeField] GameObject go_NormalCrosshair;
    [SerializeField] GameObject go_InteractiveCrosshair;

    bool isContact = false;


    void Update()
    {
        CheckObject();
    }

    void CheckObject()
    {
        Vector3 t_MousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);

        //ȭ���� 2D��ǥ�� ���� ���콺 ��ǥ�� ȯ��
        if(Physics.Raycast(cam.ScreenPointToRay(t_MousePos), out hitInfo, 100))
        {
            //Debug.Log(hitInfo.transform.name);

            Contact();
        }
        else
        {
            NotContact();
        }
    }

    void Contact()
    {
        if (hitInfo.transform.CompareTag("Interaction"))
        {
            if (!isContact)
            {
                isContact = true;

                go_InteractiveCrosshair.SetActive(true);
                go_NormalCrosshair.SetActive(false);
            }
        }
        else
        {
            NotContact();
        }
        
    }

    void NotContact()
    {
        if (isContact)
        {
            isContact = false;

            go_InteractiveCrosshair.SetActive(false);
            go_NormalCrosshair.SetActive(true);
        }
    }
}
