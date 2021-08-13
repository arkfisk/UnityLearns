using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyWall : MonoBehaviour
{
    public Sprite dmgSprite; //���� �������� �޾��� ���� ��������Ʈ
    public int hp = 4;  //������ Ÿ�� Ƚ��

    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DamageWall(int loss)
    {
        spriteRenderer.sprite = dmgSprite;
        hp -= loss;
        if (hp <= 0)
            gameObject.SetActive(false);
    }
}
