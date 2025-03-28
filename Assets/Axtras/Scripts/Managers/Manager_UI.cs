using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Manager_UI : MonoBehaviour
{
    #region Vars
    public static Manager_UI Instance { get; private set; }

    [Header("Menu UI")]
    [SerializeField] private GameObject menuPanelGO;
    [SerializeField] private Button startRunButton;
    [SerializeField] private Button exitButton;
    
    [Header("Over UI")]
    [SerializeField] private GameObject overPanelGO;
    [SerializeField] private TMP_Text storyText;
    
    [Header("Loading UI")]
    [SerializeField] private GameObject loadingPanelGO;

    [Header("Tools Settings")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D lanternCursor;
    [SerializeField] private Texture2D diaryCursor;
    [SerializeField] private Texture2D sliceCursor;
    [SerializeField] private Texture2D grabCursor;

    [Header("Game UI")]
    [SerializeField] private GameObject gamePanelGO;
    [SerializeField] private Button lanternButton;
    [SerializeField] private Button diaryButton;
    [SerializeField] private Button slicingButton;
    [SerializeField] private Button grabButton;
    [SerializeField] private Button endRunButton;
    [SerializeField] private GameObject torchFuelPanelGO;
    [SerializeField] private GameObject diaryPanelGO;
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
        if (diaryPanelGO != null)
            diaryPanelGO.SetActive(false);
        if (torchFuelPanelGO != null)
            torchFuelPanelGO.SetActive(false);
        
        // Menu buttons
        startRunButton?.onClick.AddListener(StartRun);
        exitButton?.onClick.AddListener(ExitGame);
        // Game buttons
        endRunButton?.onClick.AddListener(EndRun);
        // Tool buttons
        lanternButton?.onClick.AddListener(ToggleLantern);
        diaryButton?.onClick.AddListener(ToggleDiary);
        slicingButton?.onClick.AddListener(ToggleSlice);
        grabButton?.onClick.AddListener(ToggleGrab);
        
        Cursor.visible = true;
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }
    
    public void StartRun() {
        Debug.Log("Start run button pressed!");
        
        startRunButton.gameObject.SetActive(false);

        loadingPanelGO.SetActive(false);
        overPanelGO.SetActive(false);

        Manager_Game.Instance.StartRun();
    }
    public void EndRun() {
        Debug.Log("End run button pressed!");

        ControlLoadingUI(false);
        ControlGameUI(false);
        ControlMenuUI(false);
        ControlOverUI(true);
        
        var story = Manager_Content.Instance.PickStory();
        SetStoryText(story);  

        Manager_Game.Instance.EndRun();
    }
    public void ExitGame() {
        Debug.Log("Exit button pressed!");

        Application.Quit();
    }
    
    public void SetStoryText(string text) {
        Debug.Log($"SetStoryText: {text}");
        
        storyText.text = text;
    }
    
    public void ToggleLantern() {
        Debug.Log("Lantern button pressed!");

        if (Controller_Player.Instance.GetToolLantern()) {
            Controller_Player.Instance.SetTool(Controller_Player.Tool.None);         
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
            SetButtonSize(lanternButton, 1f);
        }
        else {
            ResetPanels();
            ResetButtonSize();

            Controller_Player.Instance.SetTool(Controller_Player.Tool.Lantern);  
            Cursor.SetCursor(lanternCursor, Vector2.zero, CursorMode.Auto);
            SetButtonSize(lanternButton, 1.5f);
        }

        bool isActive = torchFuelPanelGO.activeSelf;
        torchFuelPanelGO.SetActive(!isActive);
    }
    public void ToggleDiary() {
        Debug.Log("Diary button pressed!");
        
        if (Controller_Player.Instance.GetToolDiary()) {
            Controller_Player.Instance.SetTool(Controller_Player.Tool.None);          
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
            SetButtonSize(diaryButton, 1f);
        }
        else {
            ResetPanels();
            ResetButtonSize();

            Controller_Player.Instance.SetTool(Controller_Player.Tool.Diary); 
            Cursor.SetCursor(diaryCursor, Vector2.zero, CursorMode.Auto);
            SetButtonSize(diaryButton, 1.5f);
        }

        bool isActive = diaryPanelGO.activeSelf;
        diaryPanelGO.SetActive(!isActive);
    }
    public void ToggleSlice() {
        Debug.Log("Slicing button pressed!");
        
        if (Controller_Player.Instance.GetToolSlice()) {
            Controller_Player.Instance.SetTool(Controller_Player.Tool.None);            
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
            SetButtonSize(slicingButton, 1f);
        }
        else {
            ResetPanels();
            ResetButtonSize();

            Controller_Player.Instance.SetTool(Controller_Player.Tool.Slice); 
            Cursor.SetCursor(sliceCursor, Vector2.zero, CursorMode.Auto);
            SetButtonSize(slicingButton, 1.5f);
        }
    }
    public void ToggleGrab() {
        Debug.Log("Grabing button pressed!");
        
        if (Controller_Player.Instance.GetToolGrab()) {
            Controller_Player.Instance.SetTool(Controller_Player.Tool.None);            
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
            SetButtonSize(grabButton, 1f);
        }
        else {
            ResetPanels();
            ResetButtonSize();

            Controller_Player.Instance.SetTool(Controller_Player.Tool.Grab); 
            Cursor.SetCursor(grabCursor, Vector2.zero, CursorMode.Auto);
            SetButtonSize(grabButton, 1.5f);
        }
    }

    private void ResetButtonSize() {
        var lanternRT = lanternButton.GetComponent<RectTransform>();
        var diaryRT = diaryButton.GetComponent<RectTransform>();
        var slicingRT = slicingButton.GetComponent<RectTransform>();
        var grabingRT = grabButton.GetComponent<RectTransform>();

        lanternRT.localScale = Vector3.one;
        diaryRT.localScale = Vector3.one;
        slicingRT.localScale = Vector3.one;
        grabingRT.localScale = Vector3.one;
    }
    private void ResetPanels() {
        torchFuelPanelGO.SetActive(false);
        diaryPanelGO.SetActive(false);
    }

    private void SetButtonSize(Button btn, float sizeVal) {
        var rt = btn.GetComponent<RectTransform>();
        rt.localScale = new (sizeVal, sizeVal, sizeVal);
    }

    public void ControlLoadingUI(bool active) {
        Debug.Log($"ControlLoadingUI {active}");

        loadingPanelGO.SetActive(active);
    }
    public void ControlGameUI(bool active) {
        Debug.Log($"ControlGameUI {active}");

        loadingPanelGO.SetActive(active);
    }
    public void ControlMenuUI(bool active) {
        Debug.Log($"ControlMenuUI {active}");

        loadingPanelGO.SetActive(active);
    }
    public void ControlOverUI(bool active) {
        Debug.Log($"ControlOverUI {active}");

        overPanelGO.SetActive(active);
    }
}