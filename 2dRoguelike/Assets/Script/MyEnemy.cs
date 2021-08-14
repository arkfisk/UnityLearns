using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyEnemy : MyMovingObject
{
    public int playerDamage;

    private Animator animator;
    private Transform target;
    private bool skipMove;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;


    protected override void Start()
    {
        MyGameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            yDir = target.position.y > transform.position.y ? 1 : -1;
        else
            xDir= target.position.x > transform.position.x ? 1 : -1;

        AttemptMove<MyPlayer>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        MyPlayer hitPlayer = component as MyPlayer;

        hitPlayer.LoseFood(playerDamage);
        animator.SetTrigger("enemyAttack");
        MySoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }
}
