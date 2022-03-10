using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates
{
    GUARD,
    PATROL,
    CHASE,
    DEAD
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    private EnemyStates enemyStates;
    private NavMeshAgent _agent;
    private Animator _anim;

    [Header("Basic Settings")] public float sightRadius;

    public bool isGuard;

    private float speed;
    private GameObject attackTarget;

    private bool isWalk;
    private bool isChase;
    private bool isFollow;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();
        speed = _agent.speed;
    }

    private void Update()
    {
        SwitchStates();
        SwitchAnimation();
    }

    private void SwitchAnimation()
    {
        _anim.SetBool("Walk", isWalk);
        _anim.SetBool("Chase", isChase);
        _anim.SetBool("Follow", isFollow);
    }

    private void SwitchStates()
    {
        if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }

        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                break;
            case EnemyStates.PATROL:
                break;
            case EnemyStates.CHASE:
                ChaseTarget();
                break;
            case EnemyStates.DEAD:
                break;
        }
    }

    private void ChaseTarget()
    {
        //TODO: 追Player

        //TODO:在攻击范围内攻击
        //TODO:配合动画
        isWalk = false;
        isChase = true;
        _agent.speed = speed;
        if (!FoundPlayer())
        {
            //TODO:拉脱回到上一个状态
            isFollow = false;
            //停在当前位置
            _agent.destination = transform.position;
        }
        else
        {
            isFollow = true;
            _agent.destination = attackTarget.transform.position;
        }
    }

    private bool FoundPlayer()
    {
        //以自己为中心点,查找球形范围内所有碰撞体
        var mColliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in mColliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }

        attackTarget = null;
        return false;
    }
}