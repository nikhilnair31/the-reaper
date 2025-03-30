using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Manager_Content : MonoBehaviour 
{
    #region Variables
    public static Manager_Content Instance { get; private set; }
    
    [Header("Story Settings")]
    [SerializeField] private List<Data_Stories> storyList = new ();
    private int storyIndex = 0;
    
    [Header("Rules Settings")]
    [SerializeField] private List<Data_Rules> rulesList = new ();

    [Header("Whipsers Settings")]
    [SerializeField][TextArea(3, 10)] private string whispersStr;
    [SerializeField]private List<string> whispersList = new ();
    #endregion 

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start() {
        Init();
        DataLoad();
    }
    private void Init() {
        if (whispersList.Count == 0)
            whispersList = whispersStr.Split('\n').ToList();
    }
    private void DataLoad() {
        // Load stories from save files
        var stories = Manager_SaveLoad.Instance.LoadStories();
        if (stories == null) {
            Manager_SaveLoad.Instance.SaveStories();
            stories = storyList;
        };
        // Set stories in the content manager
        SetStories(stories);

        // Load rules from save files
        var rules = Manager_SaveLoad.Instance.LoadRules();
        if (rules == null) {
            Manager_SaveLoad.Instance.SaveRules();
            rules = rulesList;
        };
        // Set rules in the content manager
        SetRules(rules);
        
        // Initialize the diary rules
        InitDiaryRules();
    }
    
    public void InitDiaryRules() {
        var listOfRulesData = rulesList.Where(rule => rule.unlocked).ToList();
        var listOfRulesStr = listOfRulesData.Select(rule => rule.ruleStr).ToList();
        Controller_Diary.Instance.InitDiaryPages(listOfRulesStr);
    }

    public List<Data_Stories> GetStories() {
        return storyList;
    }
    public List<Data_Rules> GetRules() {
        return rulesList;
    }

    private void SetStories(List<Data_Stories> stories) {
        storyList = stories;
    }
    private void SetRules(List<Data_Rules> rules) {
        rulesList = rules;
    }

    public string PickWhisper() {
        if (whispersList.Count == 0) {
            Debug.LogWarning("No whisper available");
            return "...";
        }
        
        int randomIndex = Random.Range(0, whispersList.Count);
        string whisper = whispersList[randomIndex];
        Debug.Log($"Picked Whisper: {whisper}");

        return whisper;
    }
    public string PickStory() {
        if (storyList.Count == 0) {
            Debug.LogWarning("No story objects available");
            return "...";
        }

        if (storyIndex >= storyList.Count) {
            Debug.Log("All stories have been shown. Resetting story index.");
            return "...";
        }
        
        var pickedStoryObj = storyList[storyIndex];
        if (pickedStoryObj == null) {
            Debug.LogError($"Story object at index {storyIndex} is null.");
            return "...";
        }
        Debug.Log($"Picked Story Obj: {pickedStoryObj}");
        
        var story = pickedStoryObj.storyStr;
        pickedStoryObj.shown = true;
        storyIndex++;

        return story;
    }
    public void PickRule() {
        if (rulesList.Count == 0) {
            Debug.LogWarning("No rule objects available");
            return;
        }

        var lockedRulesList = rulesList.Where(rule => !rule.unlocked).ToList();
        if (lockedRulesList.Count == 0) {
            Debug.Log("All rules have been unlocked.");
            return;
        }

        var unlockRuleObj = rulesList[Random.Range(0, lockedRulesList.Count)];
        Debug.Log($"Picked to unlock rule obj: {unlockRuleObj}");
        
        unlockRuleObj.unlocked = true;
    }
}