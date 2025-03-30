using UnityEngine;

public class Manager_Game : MonoBehaviour 
{
    #region Vars
    public static Manager_Game Instance { get; private set; }
    
    private GameObject playerGO;
    private GameObject mainCameraGO;

    [Header("Run Settings")]
    [SerializeField] private int run = 0;
    
    [Header("Score Settings")]
    [SerializeField] private int score = 0;
    [SerializeField] private int scoreInc = 10;
    [SerializeField] private int scoreDec = 15;
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
        if (playerGO == null)
            playerGO = GameObject.FindGameObjectWithTag("Player");
        if (mainCameraGO == null)   
            mainCameraGO = Camera.main.gameObject;
    }
    private void DataLoad() {
        // Load general data from save file
        var general = Manager_SaveLoad.Instance.LoadGeneral();
        
        // Set the run and score values
        SetRunCnt(general.runCnt);
        SetScore(general.scoreCnt);
    }

    public void StartRun() {
        Debug.Log("Game Run Started");
        
        // Add run count
        run++;
    }
    public void EndRun() {
        Debug.Log("Game Run Ended");

        Manager_SaveLoad.Instance.SaveStories();
        Manager_SaveLoad.Instance.SaveRules();
    }
    
    public void IncScore() {
        Debug.Log($"IncScore | Score: {score} | ScoreInc: {scoreInc}");
        score += scoreInc;
    }
    public void DecScore() {
        Debug.Log($"DecScore | Score: {score} | ScoreInc: {scoreInc}");
        score -= scoreInc;
    }

    private void SetRunCnt(int runcnt) {
        run = runcnt;
    }
    private void SetScore(int scoreval) {
        score = scoreval;
    }
}