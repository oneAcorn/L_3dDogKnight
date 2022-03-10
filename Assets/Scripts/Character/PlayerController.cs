using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator anim;
    private CharacterState characterState;

    private GameObject attackTarget;
    private float lastAttackTime;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterState = GetComponent<CharacterState>();
    }

    private void Start()
    {
        MouseManager.Instance.OnMouseClick += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
    }

    private void Update()
    {
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", _agent.velocity.sqrMagnitude);
    }

    public void MoveToTarget(Vector3 target)
    {
        //打断攻击的Coroutine
        StopAllCoroutines();
        _agent.isStopped = false;
        _agent.destination = target;
    }

    private void EventAttack(GameObject target)
    {
        if (target != null)
        {
            attackTarget = target;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    private IEnumerator MoveToAttackTarget()
    {
        _agent.isStopped = false;
        transform.LookAt(attackTarget.transform);
        //玩家与敌人间距离超过攻击距离1
        //TODO: 修改攻击范围到配置
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > 1f)
        {
            _agent.destination = attackTarget.transform.position;
            yield return null;
        }

        //停止移动
        _agent.isStopped = true;

        if (lastAttackTime < 0)
        {
            anim.SetTrigger("Attack");
            //重置冷却时间
            lastAttackTime = 0.5f;
        }
    }
}