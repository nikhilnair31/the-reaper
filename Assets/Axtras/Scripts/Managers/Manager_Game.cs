using UnityEngine;

public class Manager_Game : MonoBehaviour 
{
    #region Vars
    public static Manager_Game Instance { get; private set; }
    
    private GameObject playerGO;
    private GameObject mainCameraGO;

    [Header("Game Settings")]
    [SerializeField] private int run = 0;
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
    }
    private void Init() {
        if (playerGO == null)
            playerGO = GameObject.FindGameObjectWithTag("Player");
        if (mainCameraGO == null)   
            mainCameraGO = Camera.main.gameObject;
    }
    
    public void StartRun() {
        Debug.Log("Game Run Started");
        
        run++;
        score = 0;
    }
    public void EndRun() {
        Debug.Log("Game Run Ended");
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