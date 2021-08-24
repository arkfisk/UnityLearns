using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{
    [SerializeField] Camera cam;

    RaycastHit hitInfo; //레이저를 맞은 사물의 정보를 담음

    [SerializeField] GameObject go_NormalCrosshair;
    [SerializeField] GameObject go_InteractiveCrosshair;
    [SerializeField] GameObject go_Crosshair;
    [SerializeField] GameObject go_Cursor;
    [SerializeField] GameObject go_FieldCursor;
    [SerializeField] GameObject go_TargetNameBar;
    [SerializeField] Text txt_TargetName;

    bool isContact = false;
    public static bool isInteract = false;

    [SerializeField] ParticleSystem ps_QuestionEffect;

    [SerializeField] Image img_Interaction;
    [SerializeField] Image img_InteractionEffect;

    DialogueManager theDM;


    public void SettingUI(bool p_flag)
    {
        go_Crosshair.SetActive(p_flag);
        if (!p_flag)
        {
            StopCoroutine("Interaction");
            Color color = img_Interaction.color;
            color.a = 0;
            img_Interaction.color = color;
            go_TargetNameBar.SetActive(false);
            go_Cursor.SetActive(false);
            go_FieldCursor.SetActive(false);
        }
        else
        {
            if (CameraController.onlyView)
                go_Cursor.SetActive(true);
            else
                go_FieldCursor.SetActive(true);

            go_NormalCrosshair.SetActive(true);
            go_InteractiveCrosshair.SetActive(false);
        }

        isInteract = !p_flag;
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
        if (CameraController.onlyView)
        {
            Vector3 t_MousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);

            //화면의 2D좌표를 실제 마우스 좌표로 환산
            if (Physics.Raycast(cam.ScreenPointToRay(t_MousePos), out hitInfo, 100))
            {
                Contact();
            }
            else
            {
                NotContact();
            }
        }
        else
        {
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, 2)) //마우스와 크로스헤어 시점을 일치
            {
                Contact();
            }
            else
            {
                NotContact();
            }
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

                if (!CameraController.onlyView)
                {
                    StopCoroutine("InteractionEffect");
                    StopCoroutine("Interaction");
                    StartCoroutine("Interaction", true);
                    StartCoroutine("InteractionEffect");
                }
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

            if (!CameraController.onlyView)
            {
                StopCoroutine("Interaction");
                StartCoroutine("Interaction", false);
            }
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

    IEnumerator InteractionEffect()  //한 프레임씩 대기하면서 계속 투명해지고 크기가 이미지의 크기가 커짐
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

        yield return new WaitForSeconds(0.5f);

        InteractionEvent t_Event = hitInfo.transform.GetComponent<InteractionEvent>();

        if (hitInfo.transform.GetComponent<InteractionType>().isObject)
        {
            DialogueCall(t_Event);
        }
        else
        {
            TransferCall();
        }
    }

    void TransferCall()
    {
        string t_SceneName = hitInfo.transform.GetComponent<InteractionDoor>().GetSceneName();
        string t_LocationName = hitInfo.transform.GetComponent<InteractionDoor>().GetLocationName();
        StartCoroutine(FindObjectOfType<TransferManager>().Transfer(t_SceneName, t_LocationName));
    }

    void DialogueCall(InteractionEvent p_event)
    {
        theDM.SetNextEvent(p_event.GetNextEvent());
        if (p_event.GetAppearType() == AppearType.Appear) theDM.SetAppearObjects(p_event.GetTargets());
        else if (p_event.GetAppearType() == AppearType.Disappear) theDM.SetDisappearObjects(p_event.GetTargets());
        theDM.ShowDialogue(p_event.GetDialogue());
    }
}
