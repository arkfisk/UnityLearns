using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyWall : MonoBehaviour
{
    public Sprite dmgSprite; //���� �������� �޾��� ���� ��������Ʈ
    public int hp = 4;  //������ Ÿ�� Ƚ��
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
