using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class Data_CorpseFeature 
{
    [Header("Feature Tracking")]
    public List<string> appliedTattoos = new ();
    public List<string> appliedScars = new ();
    public List<string> appliedAura = new ();
    public List<string> missingLimbs = new ();
}