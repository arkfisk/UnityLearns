using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    [SerializeField] float fadeSpeed;


    bool CheckSameSprite(SpriteRenderer p_Spriterenderer, Sprite p_Sprite)
    {
        if (p_Spriterenderer.sprite == p_Sprite)
            return true;
        else
            return false;
    }

    public IEnumerator SpriteChangeCoroutine(Transform p_Target, string p_SpriteName)
    {
        SpriteRenderer[] t_SpriteRenderer = p_Target.GetComponentsInChildren<SpriteRenderer>();
        Sprite t_Sprite = Resources.Load("Characters/"+p_SpriteName, typeof(Sprite)) as Sprite; //리소스 폴더의 캐릭터 폴더에 있는 이미지 파일을 스프라이트 형식으로 변환

        if (!CheckSameSprite(t_SpriteRenderer[0], t_Sprite))
        {
            Color t_color = t_SpriteRenderer[0].color;
            Color t_ShadowColor = t_SpriteRenderer[1].color;
            t_color.a = 0;
            t_ShadowColor.a = 0;
            t_SpriteRenderer[0].color = t_color;
            t_SpriteRenderer[1].color = t_ShadowColor;

            t_SpriteRenderer[0].sprite = t_Sprite;
            t_SpriteRenderer[1].sprite = t_Sprite;

            while (t_color.a < 1)
            {
                t_color.a += fadeSpeed;
                t_ShadowColor.a += fadeSpeed;
                t_SpriteRenderer[0].color = t_color;
                t_SpriteRenderer[1].color = t_ShadowColor;

                yield return null;
            }
        }
    }
}
