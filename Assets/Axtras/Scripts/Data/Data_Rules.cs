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
    [SerializeField] public RuleData ruleDat;
    #endregion
    
    [Serializable]
    public class RuleData 
    {
        public TattooRule tattoo;
        public ScarRule scar;
        public AuraRule aura;
        public LimbRule limb;
    }
    
    [Serializable]
    public class TattooRule 
    {
        public string name;
        public int count;
        public string location;
    }
    
    [Serializable]
    public class ScarRule 
    {
        public string name;
        public int count;
        public string location;
    }
    
    [Serializable]
    public class AuraRule 
    {
        public string name;
    }
    
    [Serializable]
    public class LimbRule 
    {
        public string name;
        public int count;
    }
}