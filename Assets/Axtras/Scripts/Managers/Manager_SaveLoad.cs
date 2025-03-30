using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class Manager_SaveLoad : MonoBehaviour 
{
    #region Vars
    public static Manager_SaveLoad Instance { get; private set; }

    private List<Data_Stories> stories = new ();
    private List<Data_Rules> rules = new ();
    
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
    public List<Data_Stories> LoadStories() {
        string filePath = Application.persistentDataPath + "/" + storyFileName + ".json";
        if (File.Exists(filePath)) {
            string jsonData = File.ReadAllText(filePath);
            stories = JsonUtility.FromJson<StoriesList>(jsonData).stories;
            Debug.Log($"Stories loaded from: {filePath}");
            return stories;
        } 
        else {
            Debug.Log("No stories save file found.");
            return new();
        }
    }
    public List<Data_Rules> LoadRules() {
        string filePath = Application.persistentDataPath + "/" + rulesFileName + ".json";
        if (File.Exists(filePath)) {
            string jsonData = File.ReadAllText(filePath);
            rules = JsonUtility.FromJson<RulesList>(jsonData).rules;
            Debug.Log($"Rules loaded from: {filePath}");
            return rules;
        } 
        else {
            Debug.Log("No rules save file found.");
            return new();
        }
    }
    #endregion
    
    #region Save
    public void SaveStories() {
        var storiesList = new StoriesList() {
            stories = Manager_Content.Instance.GetStories()
        };
        string jsonData = JsonUtility.ToJson(storiesList, true);
        
        string filePath = Application.persistentDataPath + "/" + storyFileName + ".json";
        File.WriteAllText(filePath, jsonData);
        
        Debug.Log("Stories saved to: " + filePath);
    }
    public void SaveRules() {
        var rulesList = new RulesList() {
            rules = Manager_Content.Instance.GetRules()
        };
        string jsonData = JsonUtility.ToJson(rulesList, true);
        
        string filePath = Application.persistentDataPath + "/" + rulesFileName + ".json";
        File.WriteAllText(filePath, jsonData);
        
        Debug.Log("Rules saved to: " + filePath);
    }
    #endregion
    
    #region Serialization and Deserialization
    [Serializable]
    private class StoriesList {
        public List<Data_Stories> stories = new ();
    }
    [Serializable]
    private class RulesList {
        public List<Data_Rules> rules = new ();
    }
    #endregion
}