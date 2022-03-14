using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator anim;
    private CharacterState characterState;

    private GameObject attackTarget;
    private float lastAttackTime;
    private bool isDead;

    private float stopDistance;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterState = GetComponent<CharacterState>();
        stopDistance = _agent.stoppingDistance;
    }

    private void Start()
    {
        SaveManager.Instance.LoadPlayerData();
    }

    private void OnEnable()
    {
        MouseManager.Instance.OnMouseClick += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
        GameManager.Instance.RegisterPlayer(characterState);
    }

    private void OnDisable()
    {
        MouseManager.Instance.OnMouseClick -= MoveToTarget;
        MouseManager.Instance.OnEnemyClicked -= EventAttack;
    }

    private void Update()
    {
        isDead = characterState.CurrentHealth == 0;
        if (isDead)
        {
            GameManager.Instance.NotifyObservers();
        }

        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", _agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }

    public void MoveToTarget(Vector3 target)
    {
        //打断攻击的Coroutine
        StopAllCoroutines();
        if (isDead)
            return;
        //复原设置的停止距离
        _agent.stoppingDistance = stopDistance;
        _agent.isStopped = false;
        _agent.destination = target;
    }

    private void EventAttack(GameObject target)
    {
        if (isDead)
            return;
        if (target != null)
        {
            attackTarget = target;
            characterState.isCritical = UnityEngine.Random.value < characterState.attackData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    private IEnumerator MoveToAttackTarget()
    {
        _agent.isStopped = false;
        //在攻击范围内停止移动
        _agent.stoppingDistance = characterState.attackData.attackRange;
        transform.LookAt(attackTarget.transform);
        //玩家与敌人间距离超过攻击距离1
        while (Vector3.Distance(attackTarget.transform.position, transform.position) >
               characterState.attackData.attackRange)
        {
            _agent.destination = attackTarget.transform.position;
            yield return null;
        }

        //停止移动
        _agent.isStopped = true;

        if (lastAttackTime < 0)
        {
            anim.SetBool("Critical", characterState.isCritical);
            anim.SetTrigger("Attack");
            //重置冷却时间
            lastAttackTime = characterState.attackData.coolDown;
        }
    }

    //Animation Event
    private void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<Rock>())
            {
                attackTarget.GetComponent<Rock>().rockState = Rock.RockState.HitEnemy;
                //初始速度本身是0,但是这样会导致状态切换为HitNothing,所以刚开始给他一个1的速度(飞行时是600多)
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 22, ForceMode.Impulse);
            }
        }
        else
        {
            var targetState = attackTarget.GetComponent<CharacterState>();
            targetState.TakeDamage(characterState, targetState);
        }
    }
}