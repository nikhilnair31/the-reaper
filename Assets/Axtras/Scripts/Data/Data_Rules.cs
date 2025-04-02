using System;
using UnityEngine;

[Serializable]
public class Data_Rules 
{
    #region Variables
    [Header("Rules Settings")]
    [SerializeField] public string rule_content = "";
    [SerializeField] public bool is_universal = false;
    [SerializeField] public bool is_unlocked = false;
    [SerializeField] public RuleData rule_data;
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
        public bool enabled;
        public string type;
        public bool exception;
        public bool reference;
    }
    
    [Serializable]
    public class ScarRule 
    {
        public bool enabled;
        public string type;
        public bool exception;
    }
    
    [Serializable]
    public class AuraRule 
    {
        public bool enabled;
        public string color;
        public bool exception;
        public bool reference;
    }
    
    [Serializable]
    public class LimbRule 
    {
        public bool enabled;
        public string missing;
        public bool exception;
    }
}