using UnityEngine;
using System;
using System.Collections.Generic;

public class Manager_SaveLoad : MonoBehaviour 
{
    #region Vars
    public static Manager_SaveLoad Instance { get; private set; }
    
    [Header("Save Settings")]
    [SerializeField] private string storyFileName = "story";
    [SerializeField] private string whispersFileName = "whispers";
    [SerializeField] private string rulesFileName = "rules";
    #endregion
    
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    #region Load
    #endregion
    
    #region Set
    #endregion
    
    #region Save
    #endregion
    
    #region Serialization and Deserialization
    #endregion
}