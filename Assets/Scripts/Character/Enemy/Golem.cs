using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

public class Golem : EnemyController
{
    [Header("Skill")] public float kickForce = 20;
    public GameObject rockPrefab;
    public Transform grabRockPos;
    private GameObject rock;
    private bool isHoldingRock;

    protected override void Update()
    {
        base.Update();
        if (isHoldingRock && rock != null)
        {
            rock.transform.position = grabRockPos.position;
            rock.transform.rotation = grabRockPos.rotation;
        }
    }

    //Animation Event
    public void KickOff()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetState = attackTarget.GetComponent<CharacterState>();
            Vector3 flyDir = (attackTarget.transform.position - transform.position).normalized;

            targetState.GetComponent<NavMeshAgent>().isStopped = true;
            targetState.GetComponent<NavMeshAgent>().velocity = flyDir * kickForce;
            targetState.GetComponent<Animator>().SetTrigger("Dizzy");
            targetState.TakeDamage(characterState, targetState);
        }
    }

    //Animation Event
    public void GrabRock()
    {
        //第三个参数传Quaternion.identity意味着保持prefab自身设置的旋转
        rock = Instantiate(rockPrefab, grabRockPos.position, grabRockPos.rotation);
        isHoldingRock = true;
    }

    //Animation Event
    public void ThrowRock()
    {
        //第三个参数传Quaternion.identity意味着保持prefab自身设置的旋转
        // var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
        isHoldingRock = false;
        rock.GetComponent<Rock>().target = attackTarget;
        rock.GetComponent<Rock>().Shoot();
        rock = null;
    }
}