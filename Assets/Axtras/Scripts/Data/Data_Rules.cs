using System;
using UnityEngine;

[Serializable]
public class Data_Rules 
{
    #region Variables
    [Header("Rules Settings")]
    [SerializeField] public string ruleStr = "";
    [SerializeField] public bool universal = false;
    [SerializeField] public bool unlocked = false;
    #endregion
}