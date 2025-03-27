using UnityEngine;
using System.Collections.Generic;

public class Manager_Game : MonoBehaviour 
{
    #region Vars
    public static Manager_Game Instance { get; private set; }
    
    private GameObject playerGO;
    private GameObject mainCameraGO;

    [Header("Game Settings")]
    [SerializeField] private int score = 0;
    [SerializeField] private int scoreInc = 10;
    [SerializeField] private int scoreDec = 15;

    [Header("Story Settings")]
    [SerializeField] private List<string> storyList = new ();
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
        if (playerGO == null)
            playerGO = GameObject.FindGameObjectWithTag("Player");
        if (mainCameraGO == null)   
            mainCameraGO = Camera.main.gameObject;
    }
    
    public void StartGame() {
        Debug.Log("Game Started");
    }
    public void EndGame() {
        Debug.Log("Game Ended");

        Manager_UI.Instance.ControlLoadingUI(false);
        Manager_UI.Instance.ControlGameUI(false);
        Manager_UI.Instance.ControlMenuUI(false);
        Manager_UI.Instance.ControlOverUI(true);

        Manager_UI.Instance.SetStoryText(PickStory());  
    }

    private string PickStory() {
        if (storyList.Count == 0) {
            Debug.LogWarning("No story available");
            return "...";
        }
        
        int randomIndex = Random.Range(0, storyList.Count);
        string story = storyList[randomIndex];
        Debug.Log($"Picked Story: {story}");

        return story;
    }
    
    public void IncScore() {
        Debug.Log($"IncScore | Score: {score} | ScoreInc: {scoreInc}");
        score += scoreInc;
    }
    public void DecScore() {
        Debug.Log($"DecScore | Score: {score} | ScoreInc: {scoreInc}");
        score -= scoreInc;
    }
}