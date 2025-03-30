using UnityEngine;

public class Manager_Game : MonoBehaviour 
{
    #region Vars
    public static Manager_Game Instance { get; private set; }
    
    private GameObject playerGO;
    private GameObject mainCameraGO;
    private Data_General generalData = new();

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
        var loadedGeneralData = Manager_SaveLoad.Instance.LoadGeneral();
        if (loadedGeneralData == null) {
            Manager_SaveLoad.Instance.SaveGeneral();
        }
        
        // Set the run and score values
        SetRunCnt(generalData.runCnt);
        SetScore(generalData.scoreCnt);
    }

    public void StartRun() {
        Debug.Log("Game Run Started");

        // Prewarm physics steps
        for (int i = 0; i < 100; i++) {
            Physics.simulationMode = SimulationMode.Script;
            Physics.Simulate(Time.fixedDeltaTime);
        }
        Physics.simulationMode = SimulationMode.FixedUpdate;

        // Set time to normal speed
        Time.timeScale = 1f;
        
        // Add run count
        run++;
    }
    public void EndRun() {
        Debug.Log("Game Run Ended");
        
        // Pick a random story from the list
        var story = Manager_Content.Instance.PickStory();
        Manager_UI.Instance.SetStoryText(story);

        // Pick and unlock a random rule from the list
        Manager_Content.Instance.PickRule();

        // Save the current set of stories and rules
        Manager_SaveLoad.Instance.SaveStories();
        Manager_SaveLoad.Instance.SaveRules();

        // Set time to frozen
        Time.timeScale = 0f;
    }
    
    public void IncScore() {
        Debug.Log($"IncScore | Score: {score} | ScoreInc: {scoreInc}");
        score += scoreInc;
    }
    public void DecScore() {
        Debug.Log($"DecScore | Score: {score} | ScoreInc: {scoreInc}");
        score -= scoreInc;
    }

    public Data_General GetGeneral() {
        return generalData;
    }

    private void SetRunCnt(int runcnt) {
        run = runcnt;
    }
    private void SetScore(int scoreval) {
        score = scoreval;
    }
}