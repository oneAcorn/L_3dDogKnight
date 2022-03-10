using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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
    private NavMeshAgent agent;
    private Animator anim;

    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;

    private float speed;
    private GameObject attackTarget;
    public float lookAtTime;
    private float remainLookAtTime;
    [Header("Patrol State")] public float patrolRange;
    private Vector3 wayPoint;
    private Vector3 guardPos;

    private bool isWalk;
    private bool isChase;
    private bool isFollow;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        speed = agent.speed;
        guardPos = transform.position;
        remainLookAtTime = lookAtTime;
    }

    private void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
    }

    private void Update()
    {
        SwitchStates();
        SwitchAnimation();
    }

    private void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
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
                Patrol();
                break;
            case EnemyStates.CHASE:
                ChaseTarget();
                break;
            case EnemyStates.DEAD:
                break;
        }
    }

    private void Patrol()
    {
        isChase = false;
        agent.speed = speed * 0.5f;

        if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
        {
            //走到了巡逻点
            isWalk = false;
            if (remainLookAtTime > 0)
            {
                remainLookAtTime -= Time.deltaTime;
            }
            else
            {
                GetNewWayPoint();
            }
        }
        else
        {
            isWalk = true;
            agent.destination = wayPoint;
        }
    }

    private void ChaseTarget()
    {
        isWalk = false;
        isChase = true;
        agent.speed = speed;
        if (!FoundPlayer())
        {
            isFollow = false;
            if (remainLookAtTime > 0)
            {
                //停在当前位置
                agent.destination = transform.position;
                remainLookAtTime -= Time.deltaTime;
            }
            else if (isGuard)
            {
                enemyStates = EnemyStates.PATROL;
            }
            else
            {
                enemyStates = EnemyStates.GUARD;
            }
        }
        else
        {
            isFollow = true;
            agent.destination = attackTarget.transform.position;
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

    private void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;

        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        var position = transform.position;
        Vector3 randomPoint = new Vector3(guardPos.x + randomX, position.y,
            guardPos.z + randomZ);
        //找到随机点附近可以移动到(walkable)的那个点
        //areaMask对应的是在Navigation中Walkable Layer的值
        wayPoint = NavMesh.SamplePosition(randomPoint, out var hit, patrolRange, 1) ? hit.position : position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
}