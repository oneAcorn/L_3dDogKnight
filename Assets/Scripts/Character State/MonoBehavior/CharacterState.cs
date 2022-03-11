using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterState : MonoBehaviour
{
    public CharacterData_SO templateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    //是否暴击
    [HideInInspector] public bool isCritical;

    private void Awake()
    {
        if (templateData != null)
        {
            //生成模板数据的copy,防止多个Character共用一个模板
            characterData = Instantiate(templateData);
        }
    }

    #region Read from Data_SO

    public int MaxHealth
    {
        get => characterData != null ? characterData.maxHealth : 0;
        set => characterData.maxHealth = value;
    }

    public int CurrentHealth
    {
        get => characterData != null ? characterData.currentHealth : 0;
        set => characterData.currentHealth = value;
    }

    public int BaseDefence
    {
        get => characterData != null ? characterData.baseDefence : 0;
        set => characterData.baseDefence = value;
    }

    public int CurrentDefence
    {
        get => characterData != null ? characterData.currentDefence : 0;
        set => characterData.currentDefence = value;
    }

    #endregion

    #region Character Combat

    public void TakeDamage(CharacterState attacker, CharacterState defener)
    {
        int damage = Mathf.Max((attacker.CurrentDamage() - defener.CurrentDefence), 0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        //TODO Update UI
        //TODO Experience Update
        if (isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }
    }

    private int CurrentDamage()
    {
        float coreDamage = Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)
            coreDamage *= attackData.criticalMultiplier;
        return (int) coreDamage;
    }

    #endregion
}