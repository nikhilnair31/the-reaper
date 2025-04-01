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
    private Data_General general;
    
    [Header("Save Settings")]
    [SerializeField] private string generalFileName = "general";
    [SerializeField] private string whispersFileName = "whispers";
    [SerializeField] private string storyFileName = "story";
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
            Debug.Log($"Stories loaded from: {filePath}\nData:\n{JsonUtility.ToJson(stories, true)}");
            return stories;
        } 
        else {
            Debug.Log("No stories save file found.");
            return null;
        }
    }
    public List<Data_Rules> LoadRules() {
        string filePath = Application.persistentDataPath + "/" + rulesFileName + ".json";
        if (File.Exists(filePath)) {
            string jsonData = File.ReadAllText(filePath);
            Debug.Log($"jsonData: {jsonData}");
            rules = JsonHelper.FromJson<Data_Rules>(jsonData);
            Debug.Log($"rules: {rules}");
            Debug.Log($"Rules loaded from: {filePath}\nData:\n{JsonUtility.ToJson(rules, true)}");
            return rules;
        } 
        else {
            Debug.Log("No rules save file found.");
            return null;
        }
    }
    public Data_General LoadGeneral() {
        string filePath = Application.persistentDataPath + "/" + generalFileName + ".json";
        if (File.Exists(filePath)) {
            string jsonData = File.ReadAllText(filePath);
            general = JsonUtility.FromJson<Data_General>(jsonData);
            Debug.Log($"General loaded from: {filePath}\nData:\n{JsonUtility.ToJson(general, true)}");
            return general;
        } 
        else {
            Debug.Log("No general save file found.");
            return null;
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
    public void SaveGeneral() {
        var general = Manager_Game.Instance.GetGeneral();
        string jsonData = JsonUtility.ToJson(general, true);
        
        string filePath = Application.persistentDataPath + "/" + generalFileName + ".json";
        File.WriteAllText(filePath, jsonData);
        
        Debug.Log("General saved to: " + filePath);
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
    [Serializable]
    private class General {
        public Data_General general = new ();
    }
    #endregion
}