using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
   [Header("Skill")] public float kickForce = 20;

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
}
