using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static bool onlyView = true;

    Vector3 originPos;
    Quaternion originRot;
    InteractionController theIC;
    PlayerController2 thePlayer;

    Coroutine coroutine;


    private void Start()
    {
        theIC = FindObjectOfType<InteractionController>();
        thePlayer = FindObjectOfType<PlayerController2>();
    }

    public void CamOrignSetting()
    {
        originPos = transform.position;
        if (onlyView)
            originRot = Quaternion.Euler(0, 0, 0);
        else
            originRot = transform.rotation;
    }

    public void CameraTargetting(Transform p_Target, float p_camSpeed = 0.1f, bool p_isReset = false, bool p_isFinish = false)
    {
        if (!p_isReset)
        {
            if (p_Target != null)
            {
                StopAllCoroutines();
                coroutine = StartCoroutine(CameraTargettingCoroutine(p_Target, p_camSpeed));
            }
        }
        else
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            StartCoroutine(CameraResetCoroutine(p_camSpeed, p_isFinish));
        }

    }

    IEnumerator CameraTargettingCoroutine(Transform p_Target, float p_camSpeed = 0.1f) 
    {
        Vector3 t_TargetPos = p_Target.position; 
        Vector3 t_TargetFrontPos = t_TargetPos + (p_Target.forward*1.3f);
        Vector3 t_Drection = (t_TargetPos - t_TargetFrontPos).normalized;         

        while (transform.position != t_TargetFrontPos || Quaternion.Angle(transform.rotation,Quaternion.LookRotation(t_Drection))>=0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, t_TargetFrontPos, p_camSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(t_Drection), p_camSpeed);

            yield return null;
        }
    }

    IEnumerator CameraResetCoroutine(float p_camSpeed=0.1f, bool p_isFinish = false)
    {
        yield return new WaitForSeconds(0.5f);

        while (transform.position != originPos || Quaternion.Angle(transform.rotation, originRot) >= 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, originPos, p_camSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, originRot, p_camSpeed);

            yield return null;
        }
        transform.position = originPos;

        if (p_isFinish)
        {
            //???? ?????? ???????? ????
            thePlayer.Reset();
            InteractionController.isInteract = false;
        }
    }
}
