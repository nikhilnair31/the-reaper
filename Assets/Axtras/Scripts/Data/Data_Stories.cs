using System;
using UnityEngine;

[Serializable]
public class Data_Stories 
{
    #region Variables
    [Header("Stories Settings")]
    [SerializeField] public string storyStr = "";
    [SerializeField] public bool shown = false;
    #endregion
}