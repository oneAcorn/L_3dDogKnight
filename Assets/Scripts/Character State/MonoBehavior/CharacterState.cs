using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState : MonoBehaviour
{
   public CharacterData_SO characterData;

   #region Read from Data_SO

   public int MaxHealth {
      get => characterData != null ? characterData.maxHealth : 0;
      set => characterData.maxHealth = value;
   }

   public int CurrentHealth {
      get => characterData != null ? characterData.currentHealth : 0;
      set => characterData.currentHealth = value;
   }

   public int BaseDefence {
      get => characterData != null ? characterData.baseDefence : 0;
      set => characterData.baseDefence = value;
   }

   public int CurrentDefence {
      get => characterData != null ? characterData.currentDefence : 0;
      set => characterData.currentDefence = value;
   }

   #endregion

}
