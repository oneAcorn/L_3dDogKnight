using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockState
    {
        HitPlayer,
        HitEnemy,
        HitNothing
    }

    private Rigidbody rb;
    public RockState rockState;
    [Header("Basic Settings")] public float force;
    public GameObject target;
    public int damage;
    private Vector3 dir;
    public GameObject breakEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //初始速度本身是0,但是这样会导致状态切换为HitNothing,所以刚开始给他一个1的速度(飞行时是600多)
        rb.velocity = Vector3.one;
        rockState = RockState.HitPlayer;
        FlyToTarget();
    }

    private void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude < 1f)
        {
            //速度过小,没伤害了
            rockState = RockState.HitNothing;
        }
    }

    public void FlyToTarget()
    {
        if (target == null)
        {
            //防止玩家跑掉导致Target为空,然后石头飞不过去的问题
            target = FindObjectOfType<PlayerController>().gameObject;
        }

        //+ Vector3.up用来在抛射向目标前往上提一米
        dir = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(dir * force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (rockState)
        {
            case RockState.HitPlayer:
                HitPlayer(other);
                break;
            case RockState.HitEnemy:
                HitEnemy(other);
                break;
        }
    }

    private void HitPlayer(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var navAgent = other.gameObject.GetComponent<NavMeshAgent>();
            navAgent.isStopped = true;
            navAgent.velocity = dir * force;
            other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
            var playerState = other.gameObject.GetComponent<CharacterState>();
            playerState.TakeDamage(damage, playerState);
            rockState = RockState.HitNothing;
        }
    }

    private void HitEnemy(Collision other)
    {
        if (other.gameObject.GetComponent<Golem>())
        {
            //只反击石头人
            var otherState = other.gameObject.GetComponent<CharacterState>();
            otherState.TakeDamage(damage, otherState);
            Instantiate(breakEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}