using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using TMPro;

public class Manager_UI : MonoBehaviour
{
    #region Vars
    public static Manager_UI Instance { get; private set; }

    [Header("Loading UI")]
    [SerializeField] private GameObject loadingPanelGO;

    [Header("Menu UI")]
    [SerializeField] private GameObject menuPanelGO;
    [SerializeField] private Button startRunButton;
    [SerializeField] private Button exitButton;
    
    [Header("Over UI")]
    [SerializeField] private GameObject overPanelGO;
    [SerializeField] private Button newRunButton;
    [SerializeField] private Button menuOverButton;
    
    [Header("End Run UI")]
    [SerializeField] private GameObject endRunPanelGO;
    [SerializeField] private TMP_Text storyText;
    [SerializeField] private Button nextRunButton;

    [Header("Pause UI")]
    [SerializeField] private GameObject pausePanelGO;
    [SerializeField] private Button unpauseButton;
    [SerializeField] private Button menuPauseButton;

    [Header("Game Run UI")]
    [SerializeField] private GameObject gamePanelGO;
    [SerializeField] private Button lanternButton;
    [SerializeField] private Button diaryButton;
    [SerializeField] private Button slicingButton;
    [SerializeField] private Button grabButton;
    [SerializeField] private Button endRunButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private GameObject torchFuelPanelGO;
    [SerializeField] private GameObject diaryPanelGO;

    [Header("Tween Settings")]
    [SerializeField] private float fadeTime = 1f;

    [Header("Tools Settings")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D lanternCursor;
    [SerializeField] private Texture2D diaryCursor;
    [SerializeField] private Texture2D sliceCursor;
    [SerializeField] private Texture2D grabCursor;
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
        ButtonSetup();
        CanvasSetup();
    }
    private void Init() {
        ControlAllToolPanels(false);
        ControlAllButtonsSize(1f);
        
        Cursor.visible = true;
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);

        Time.timeScale = 1f;
    }
    private void ButtonSetup() {
        // Menu buttons
        startRunButton?.onClick.AddListener(StartRun);
        exitButton?.onClick.AddListener(ExitGame);

        // Over buttons
        newRunButton?.onClick.AddListener(StartRun);
        menuOverButton?.onClick.AddListener(ToMenu);

        // End Run buttons
        nextRunButton?.onClick.AddListener(StartRun);
        
        // Pause buttons
        unpauseButton?.onClick.AddListener(UnpauseGame);
        menuPauseButton?.onClick.AddListener(ToMenu);
        
        // Game buttons
        endRunButton?.onClick.AddListener(EndRun);
        pauseButton?.onClick.AddListener(PauseGame);
        
        // Tool buttons
        lanternButton?.onClick.AddListener(ToggleLantern);
        diaryButton?.onClick.AddListener(ToggleDiary);
        slicingButton?.onClick.AddListener(ToggleSlice);
        grabButton?.onClick.AddListener(ToggleGrab);
    }
    private void CanvasSetup() {
        ControlAllPanels(false);
        ControlMenuUI(true);
    }
    
    #region General
    public void StartRun() {
        StartCoroutine(StartRunCoor());
    }
    public IEnumerator StartRunCoor() {
        Debug.Log("Start run button pressed!");
        
        // Enable loading ui 
        ControlAllPanels(false);
        ControlLoadingUI(true);

        // Generate a run
        yield return new WaitForSecondsRealtime(fadeTime);
        Manager_Game.Instance.StartRun();
        yield return new WaitForSecondsRealtime(1f);

        // Enable game ui 
        ControlAllPanels(false);
        ControlGameUI(true);
    }
    
    public void EndRun() {
        StartCoroutine(EndRunCoor());
    }
    public IEnumerator EndRunCoor() {
        Debug.Log("End run button pressed!");
        
        // Enable loading ui 
        ControlAllPanels(false);
        ControlLoadingUI(true);

        // End the run
        yield return new WaitForSecondsRealtime(fadeTime);
        Manager_Game.Instance.EndRun();
        yield return new WaitForSecondsRealtime(1f);
        
        // Enable game ui 
        ControlAllPanels(false);
        ControlEndRunUI(true);
    }
    
    public void ToMenu() {
        StartCoroutine(ToMenuCoor());
    }
    public IEnumerator ToMenuCoor() {
        Debug.Log("ToMenu button pressed!");

        // Enable pause ui 
        ControlAllPanels(false);
        ControlMenuUI(true);

        yield return null;
    }
    
    public void UnpauseGame() {
        StartCoroutine(UnpauseGameCoor());
    }
    public IEnumerator UnpauseGameCoor() {
        Debug.Log("UnpauseGame button pressed!");

        // Enable game ui 
        ControlAllPanels(false);
        ControlGameUI(true);

        yield return null;
    }
    
    public void PauseGame() {
        StartCoroutine(PauseGameCoor());
    }
    public IEnumerator PauseGameCoor() {
        Debug.Log("PauseGame button pressed!");

        // Enable pause ui 
        ControlAllPanels(false);
        ControlPauseUI(true);

        yield return null;
    }
    
    public void ExitGame() {
        StartCoroutine(ExitGameCoor());
    }
    public IEnumerator ExitGameCoor() {
        Debug.Log("Exit button pressed!");

        Application.Quit();

        yield return null;
    }
    #endregion

    #region UI Control
    public void ControlAllPanels(bool active) {
        loadingPanelGO.SetActive(active);
        menuPanelGO.SetActive(active);
        overPanelGO.SetActive(active);
        endRunPanelGO.SetActive(active);
        pausePanelGO.SetActive(active);
        gamePanelGO.SetActive(active);
    }
    
    public void ControlLoadingUI(bool active) {
        Debug.Log($"ControlLoadingUI {active}");

        if (loadingPanelGO.TryGetComponent<CanvasGroup>(out var cg)) {
            cg.DOFade(1f, fadeTime)
                .SetUpdate(true)
                .OnStart(() => {
                    cg.alpha = 0f;
                    loadingPanelGO.SetActive(active);
                });
        }
    }
    public void ControlMenuUI(bool active) {
        Debug.Log($"ControlMenuUI {active}");

        if (menuPanelGO.TryGetComponent<CanvasGroup>(out var cg)) {
            cg.DOFade(1f, fadeTime)
                .SetUpdate(true)
                .OnStart(() => {
                    cg.alpha = 0f;
                    menuPanelGO.SetActive(active);
                });
        }
    }
    public void ControlEndRunUI(bool active) {
        Debug.Log($"ControlOverUI {active}");

        if (overPanelGO.TryGetComponent<CanvasGroup>(out var cg)) {
            cg.DOFade(1f, fadeTime)
                .SetUpdate(true)
                .OnStart(() => {
                    cg.alpha = 0f;
                    overPanelGO.SetActive(active);
                });
        }
    }
    public void ControlOverUI(bool active) {
        Debug.Log($"ControlOverUI {active}");

        if (endRunPanelGO.TryGetComponent<CanvasGroup>(out var cg)) {
            cg.DOFade(1f, fadeTime)
                .SetUpdate(true)
                .OnStart(() => {
                    cg.alpha = 0f;
                    endRunPanelGO.SetActive(active);
                });
        }
    }
    public void ControlPauseUI(bool active) {
        Debug.Log($"ControlPauseUI {active}");

        if (pausePanelGO.TryGetComponent(out CanvasGroup cg)) {
            cg.DOFade(1f, fadeTime)
                .SetUpdate(true)
                .OnStart(() => {
                    cg.alpha = 0f;
                    pausePanelGO.SetActive(active);
                });
        }
    }
    public void ControlGameUI(bool active) {
        Debug.Log($"ControlGameUI {active}");

        if (gamePanelGO.TryGetComponent<CanvasGroup>(out var cg)) {
            cg.DOFade(1f, fadeTime)
                .SetUpdate(true)
                .OnStart(() => {
                    cg.alpha = 0f;
                    gamePanelGO.SetActive(active);
                });
        }
    }
    
    private void ControlAllToolPanels(bool active = false) {
        torchFuelPanelGO?.SetActive(active);
        diaryPanelGO?.SetActive(active);
    }
    
    private void ControlAllButtonsSize(float sizeVal = 1f) {
        var lanternRT = lanternButton.GetComponent<RectTransform>();
        var diaryRT = diaryButton.GetComponent<RectTransform>();
        var slicingRT = slicingButton.GetComponent<RectTransform>();
        var grabingRT = grabButton.GetComponent<RectTransform>();

        lanternRT.localScale = Vector3.one * sizeVal;
        diaryRT.localScale = Vector3.one * sizeVal;
        slicingRT.localScale = Vector3.one * sizeVal;
        grabingRT.localScale = Vector3.one * sizeVal;
    }
    private void SetButtonSize(Button btn, float sizeVal) {
        var rt = btn.GetComponent<RectTransform>();
        rt.localScale = Vector3.one * sizeVal;
    }
    
    public void SetStoryText(string text) {
        storyText.text = text;
    }
    #endregion
    
    #region Tools
    public void ToggleLantern() {
        Debug.Log("Lantern button pressed!");

        if (Controller_Player.Instance.GetToolLantern()) {
            Controller_Player.Instance.SetTool(Controller_Player.Tool.None);         
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
            SetButtonSize(lanternButton, 1f);
        }
        else {
            ControlAllToolPanels(false);
            ControlAllButtonsSize(1f);

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
            ControlAllToolPanels(false);
            ControlAllButtonsSize(1f);

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
            ControlAllToolPanels(false);
            ControlAllButtonsSize(1f);

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
            ControlAllToolPanels(false);
            ControlAllButtonsSize(1f);

            Controller_Player.Instance.ResetTool();
            Controller_Player.Instance.SetTool(Controller_Player.Tool.Grab); 
            Cursor.SetCursor(grabCursor, Vector2.zero, CursorMode.Auto);
            SetButtonSize(grabButton, 1.5f);
        }
    }
    #endregion
}