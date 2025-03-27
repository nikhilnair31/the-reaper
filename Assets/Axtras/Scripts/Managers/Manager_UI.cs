using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Manager_UI : MonoBehaviour
{
    #region Vars
    public static Manager_UI Instance { get; private set; }

    public enum Tool {
        None,
        Lantern,
        Diary,
        Slice
    }

    [Header("Menu UI")]
    [SerializeField] private GameObject menuPanelGO;
    [SerializeField] private Button startButton;
    [SerializeField] private Button endButton;
    
    [Header("Over UI")]
    [SerializeField] private GameObject overPanelGO;
    [SerializeField] private TMP_Text storyText;
    
    [Header("Loading UI")]
    [SerializeField] private GameObject loadingPanelGO;

    [Header("Tools Settings")]
    [SerializeField] private Tool currentTool;
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D lanternCursor;
    [SerializeField] private Texture2D diaryCursor;
    [SerializeField] private Texture2D sliceCursor;

    [Header("Game UI")]
    [SerializeField] private GameObject gamePanelGO;
    [SerializeField] private Button lanternButton;
    [SerializeField] private Button diaryButton;
    [SerializeField] private Button slicingButton;
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
        
        startButton?.onClick.AddListener(StartGame);
        endButton?.onClick.AddListener(EndGame);
        lanternButton?.onClick.AddListener(ToggleLantern);
        diaryButton?.onClick.AddListener(ToggleDiary);
        slicingButton?.onClick.AddListener(ToggleSlice);

        currentTool = Tool.None;
        
        Cursor.visible = true;
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }
    
    public void StartGame() {
        Debug.Log("Start button pressed!");
        
        startButton.gameObject.SetActive(false);

        loadingPanelGO.SetActive(false);
        overPanelGO.SetActive(false);

        Manager_Game.Instance.StartGame();
    }
    public void EndGame() {
        Debug.Log("End button pressed!");
        
        endButton.gameObject.SetActive(false);

        loadingPanelGO.SetActive(true);
        overPanelGO.SetActive(false);

        Manager_Game.Instance.EndGame();
    }
    
    public void SetStoryText(string text) {
        Debug.Log($"SetStoryText: {text}");
        
        storyText.text = text;
    }
    
    public void ToggleLantern() {
        Debug.Log("Lantern button pressed!");

        if (currentTool == Tool.Lantern) {
            currentTool = Tool.None;            
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
            SetButtonSize(lanternButton, 1f);
        }
        else {
            ResetPanels();
            ResetButtonSize();

            currentTool = Tool.Lantern;
            Cursor.SetCursor(lanternCursor, Vector2.zero, CursorMode.Auto);
            SetButtonSize(lanternButton, 1.5f);
        }

        bool isActive = torchFuelPanelGO.activeSelf;
        torchFuelPanelGO.SetActive(!isActive);
    }
    public void ToggleDiary() {
        Debug.Log("Diary button pressed!");
        
        if (currentTool == Tool.Diary) {
            currentTool = Tool.None;            
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
            SetButtonSize(diaryButton, 1f);
        }
        else {
            ResetPanels();
            ResetButtonSize();

            currentTool = Tool.Diary;
            Cursor.SetCursor(diaryCursor, Vector2.zero, CursorMode.Auto);
            SetButtonSize(diaryButton, 1.5f);
        }

        bool isActive = diaryPanelGO.activeSelf;
        diaryPanelGO.SetActive(!isActive);
    }
    public void ToggleSlice() {
        Debug.Log("Slicing button pressed!");
        
        if (currentTool == Tool.Slice) {
            currentTool = Tool.None;            
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
            SetButtonSize(slicingButton, 1f);
        }
        else {
            ResetPanels();
            ResetButtonSize();

            currentTool = Tool.Slice;
            Cursor.SetCursor(sliceCursor, Vector2.zero, CursorMode.Auto);
            SetButtonSize(slicingButton, 1.5f);
        }
    }

    private void ResetButtonSize() {
        var lanternRT = lanternButton.GetComponent<RectTransform>();
        var diaryRT = diaryButton.GetComponent<RectTransform>();
        var slicingRT = slicingButton.GetComponent<RectTransform>();

        lanternRT.localScale = Vector3.one;
        diaryRT.localScale = Vector3.one;
        slicingRT.localScale = Vector3.one;
    }
    private void ResetPanels() {
        torchFuelPanelGO.SetActive(false);
        diaryPanelGO.SetActive(false);
    }

    private void SetButtonSize(Button btn, float sizeVal) {
        var rt = btn.GetComponent<RectTransform>();
        rt.localScale = new (sizeVal, sizeVal, sizeVal);
    }

    public bool GetLanternStatus() {
        return currentTool == Tool.Lantern;
    }
    public bool GetDiaryStatus() {
        return currentTool == Tool.Diary;
    }
    public bool GetSliceStatus() {
        return currentTool == Tool.Slice;
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