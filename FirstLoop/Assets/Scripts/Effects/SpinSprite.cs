using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinSprite : MonoBehaviour
{
    Transform tf_Target;

    bool spin = false;

    public static bool isFinished = true;


    private void Start()
    {
        tf_Target = PlayerController2.instance.transform;
    }

    private void Update()
    {
        if (tf_Target != null)
        {
            if (!spin)
            {
                Quaternion t_Rotation = Quaternion.LookRotation(tf_Target.position);
                Vector3 t_Euler = new Vector3(0, t_Rotation.eulerAngles.y, 0);
                transform.eulerAngles = t_Euler;
            }
            else
            {
                transform.Rotate(0, 99 * Time.deltaTime * 8, 0);
            }
        }
    }

    public IEnumerator SetAppearOrDiasppear(bool p_Flag)
    {
        spin = true;

        SpriteRenderer[] t_SpriteRenderer = GetComponentsInChildren<SpriteRenderer>();

        Color t_FrontColor = t_SpriteRenderer[0].color;
        Color t_RearColor = t_SpriteRenderer[1].color;

        if (p_Flag)
        {
            t_FrontColor.a = 0; t_RearColor.a = 0;
            t_SpriteRenderer[0].color = t_FrontColor; t_SpriteRenderer[1].color = t_RearColor;
        }

        float t_FadeSpeed = (p_Flag == true) ? 0.01f : -0.01f;

        yield return new WaitForSeconds(0.3f);

        while (true)
        {
            if (p_Flag && t_FrontColor.a >= 1) break;
            else if (!p_Flag && t_FrontColor.a <= 0) break;

            t_FrontColor.a += t_FadeSpeed; t_RearColor.a += t_FadeSpeed;
            t_SpriteRenderer[0].color = t_FrontColor; t_SpriteRenderer[1].color = t_RearColor;
            yield return null; 
        }
        spin = false;
        isFinished = true;
        gameObject.SetActive(p_Flag);
    } 
}
