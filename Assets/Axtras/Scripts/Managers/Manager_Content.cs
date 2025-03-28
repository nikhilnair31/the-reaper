using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Manager_Content : MonoBehaviour 
{
    #region Variables
    public static Manager_Content Instance { get; private set; }
    
    [Header("Story Settings")]
    [SerializeField] private List<string> storyList = new ();

    [Header("Whipsers Settings")]
    [SerializeField][TextArea(3, 10)] private string whispersStr;
    private List<string> whispersList = new ();
    
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
        whispersList = whispersStr.Split('\n').ToList();
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