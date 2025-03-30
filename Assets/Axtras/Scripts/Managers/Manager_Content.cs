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

    [Header("Whipsers Settings")]
    [SerializeField][TextArea(3, 10)] private string whispersStr;
    [SerializeField]private List<string> whispersList = new ();
    
    [Header("Rules Settings")]
    [SerializeField] private List<Data_Rules> rulesList = new ();
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
    }
    private void Init() {
        if (whispersList.Count == 0)
            whispersList = whispersStr.Split('\n').ToList();
    }
    
    public void InitDiaryRules() {
        var listOfRulesData = rulesList.Where(rule => rule.unlocked).ToList();
        var listOfRulesStr = listOfRulesData.Select(rule => rule.ruleStr).ToList();
        Controller_Diary.Instance.InitDiaryPages(listOfRulesStr);
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
}