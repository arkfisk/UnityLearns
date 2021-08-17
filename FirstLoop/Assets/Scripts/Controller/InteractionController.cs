using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{
    [SerializeField] Camera cam;

    RaycastHit hitInfo; //�������� ���� �繰�� ������ ����

    [SerializeField] GameObject go_NormalCrosshair;
    [SerializeField] GameObject go_InteractiveCrosshair;
    [SerializeField] GameObject go_Crosshair;
    [SerializeField] GameObject go_Cursor;
    [SerializeField] GameObject go_TargetNameBar;
    [SerializeField] Text txt_TargetName;

    bool isContact = false;
    public static bool isInteract = false;

    [SerializeField] ParticleSystem ps_QuestionEffect;

    [SerializeField] Image img_Interaction;
    [SerializeField] Image img_InteractionEffect;

    DialogueManager theDM;

    public void HideUI()
    {
        go_Crosshair.SetActive(false);
        go_Cursor.SetActive(false);
        go_TargetNameBar.SetActive(false);
    }


    private void Start()
    {
        theDM = FindObjectOfType<DialogueManager>();
    }


    void Update()
    {
        if (!isInteract)
        {
            CheckObject();
            ClickLeftButton();
        }
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
            go_TargetNameBar.SetActive(true);
            txt_TargetName.text = hitInfo.transform.GetComponent<InteractionType>().GetName();

            if (!isContact)
            {
                isContact = true;

                go_InteractiveCrosshair.SetActive(true);
                go_NormalCrosshair.SetActive(false);
                StopCoroutine("InteractionEffect");
                StopCoroutine("Interaction");
                StartCoroutine("Interaction", true);
                StartCoroutine("InteractionEffect");
                //Debug.Log("���ͷ��� �ڷ�ƾ ����-���̵���");
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
            go_TargetNameBar.SetActive(false);
            isContact = false;

            go_InteractiveCrosshair.SetActive(false);
            go_NormalCrosshair.SetActive(true);

            StopCoroutine("Interaction");
            StartCoroutine("Interaction", false);
            //Debug.Log("���ͷ��� �ڷ�ƾ ����-���̵�ƿ�");
        }
    }

    IEnumerator Interaction(bool p_Appear)
    {
        Color color = img_Interaction.color;

        if (p_Appear)
        {
            color.a = 0;
            while (color.a < 1)
            {
                color.a += 0.1f;
                img_Interaction.color = color;
                yield return null;
            }
        }
        else
        {
            while (color.a > 0)
            {
                color.a -= 0.1f;
                img_Interaction.color = color;
                yield return null;
            }
        }

    }

    IEnumerator InteractionEffect()  //�� �����Ӿ� ����ϸ鼭 ��� ���������� ũ�Ⱑ �̹����� ũ�Ⱑ Ŀ��
    {
        while(isContact && !isInteract)
        {
            Color color = img_InteractionEffect.color;
            color.a = 0.5f;

            img_InteractionEffect.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            Vector3 t_scale = img_InteractionEffect.transform.localScale;

            while (color.a > 0) 
            {
                color.a -= 0.01f;
                img_InteractionEffect.color = color;
                t_scale.Set(t_scale.x + Time.deltaTime, t_scale.y + Time.deltaTime, t_scale.z + Time.deltaTime);
                img_InteractionEffect.transform.localScale = t_scale;

                yield return null;
            }

            yield return null;
        }
    }

    void ClickLeftButton()
    {      
            if (!isInteract)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (isContact)
                    {
                        Interact();
                    }
                }
            }
    }

    void Interact()
    {
        isInteract = true;

        StopCoroutine("Interaction");
        Color color = img_Interaction.color;
        color.a = 0;
        img_Interaction.color = color;

        ps_QuestionEffect.gameObject.SetActive(true);
        Vector3 t_targetPos = hitInfo.transform.position;
        ps_QuestionEffect.GetComponent<QuestionEffect>().SetTarget(t_targetPos);
        ps_QuestionEffect.transform.position = cam.transform.position;

        StartCoroutine(WaitCollision());
    }

    IEnumerator WaitCollision()
    {
        yield return new WaitUntil(()=>QuestionEffect.isCollider);
        QuestionEffect.isCollider = false;

        theDM.ShowDialogue();
    }
}