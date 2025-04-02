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
        Debug.Log($"LoadStories");
        
        string filePath = Application.persistentDataPath + "/" + storyFileName + ".json";
        Debug.Log($"Stories loading from: {filePath}");

        if (File.Exists(filePath)) {
            var jsonData = File.ReadAllText(filePath);
            Debug.Log($"jsonData: {jsonData}");

            var storiesWrapper = JsonUtility.FromJson<StoriesWrapper>(jsonData);
            var storiesList = storiesWrapper.stories;
            Debug.Log($"stories count: {storiesList.Count}");

            return storiesList;
        } 
        else {
            Debug.Log("No stories save file found.");
            return null;
        }
    }
    public List<Data_Rules> LoadRules() {
        Debug.Log($"LoadRules");
        
        string filePath = Application.persistentDataPath + "/" + rulesFileName + ".json";
        Debug.Log($"Rules loading from: {filePath}");

        if (File.Exists(filePath)) {
            var jsonData = File.ReadAllText(filePath);
            Debug.Log($"jsonData: {jsonData}");

            var rulesWrapper = JsonUtility.FromJson<RulesWrapper>(jsonData);
            var rulesList = rulesWrapper.rules;
            Debug.Log($"rules count: {rulesList.Count}");
            
            return rulesList;
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
        var storiesList = new StoriesWrapper() {
            stories = Manager_Content.Instance.GetStories()
        };
        string jsonData = JsonUtility.ToJson(storiesList, true);
        
        string filePath = Application.persistentDataPath + "/" + storyFileName + ".json";
        File.WriteAllText(filePath, jsonData);
        
        Debug.Log("Stories saved to: " + filePath);
    }
    public void SaveRules() {
        var rulesList = new RulesWrapper() {
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
    private class StoriesWrapper {
        public List<Data_Stories> stories = new ();
    }
    [Serializable]
    private class RulesWrapper {
        public List<Data_Rules> rules = new ();
    }
    #endregion
}