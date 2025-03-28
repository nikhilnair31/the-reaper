using System;
using UnityEngine;

[Serializable]
public class Data_Rules 
{
    #region Variables
    [Header("Rules Settings")]
    [SerializeField] private string ruleStr = "";
    [SerializeField] private bool universal = false;
    [SerializeField] private bool unlocked = false;
    #endregion 
    
    Data_Rules(string ruleStr, bool universal, bool unlocked) {
        this.ruleStr = ruleStr;
        this.universal = universal;
        this.unlocked = unlocked;
    }
}