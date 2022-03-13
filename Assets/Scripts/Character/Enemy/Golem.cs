using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
   [Header("Skill")] public float kickForce = 20;
   public GameObject rockPrefab;
   public Transform handPos;

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
   public void ThrowRock()
   {
      if(attackTarget==null)
         return;
      //第三个参数传Quaternion.identity意味着保持prefab自身设置的旋转
      var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
      rock.GetComponent<Rock>().target = attackTarget;
   }
}
