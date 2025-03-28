using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Manager_Content : MonoBehaviour 
{
    #region Variables
    public static Manager_Content Instance { get; private set; }
    
    [Header("Story Settings")]
    [SerializeField][TextArea(3, 10)] private string storyStr;
    [SerializeField] private List<string> storyList = new ();

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
        if (storyList.Count == 0)
            storyList = storyStr.Split('\n').ToList();
        
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
            Debug.LogWarning("No story available");
            return "...";
        }
        
        int randomIndex = Random.Range(0, storyList.Count);
        string story = storyList[randomIndex];
        Debug.Log($"Picked Story: {story}");

        return story;
    }
}