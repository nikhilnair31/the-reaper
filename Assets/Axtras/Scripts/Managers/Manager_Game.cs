using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager_Game : MonoBehaviour
{
    #region Vars
    [Header("Game Settings")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private string gameOverSceneName = "";
    
    private bool isGameOver = false;
    
    public static Manager_Game Instance { get; private set; }
    
    public bool IsGameOver => isGameOver;
    #endregion
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
    
    public void TriggerGameOver()
    {
        if (isGameOver)
            return;
            
        isGameOver = true;
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
    
    void LoadGameOverScene()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }
    
    public void RestartGame()
    {
        isGameOver = false;
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        if (Manager_Timer.Instance != null)
        {
            Manager_Timer.Instance.SetTimerFill(1.0f);
            Manager_Timer.Instance.SetTimerActive(true);
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}