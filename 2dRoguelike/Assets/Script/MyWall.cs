using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyWall : MonoBehaviour
{
    public Sprite dmgSprite; //벽이 데미지를 받았을 때의 스프라이트
    public int hp = 4;  //가능한 타격 횟수
    public AudioClip chopSound1;
    public AudioClip chopSound2;

    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DamageWall(int loss)
    {
        MySoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
        spriteRenderer.sprite = dmgSprite;
        hp -= loss;
        if (hp <= 0)
            gameObject.SetActive(false);
    }
}
