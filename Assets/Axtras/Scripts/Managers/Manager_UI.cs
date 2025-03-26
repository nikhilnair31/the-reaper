using UnityEngine;
using UnityEngine.UI;

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

    [Header("Tools Settings")]
    [SerializeField] private Tool currentTool;
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Sprite lanternCursor;
    [SerializeField] private Sprite diaryCursor;
    [SerializeField] private Sprite sliceCursor;

    [Header("Game UI")]
    [SerializeField] private RectTransform cursorRT;
    [SerializeField] private Image cursorImage;
    [SerializeField] private Button lanternButton;
    [SerializeField] private Button diaryButton;
    [SerializeField] private Button slicingButton;
    [SerializeField] private GameObject torchFuelPanelGO;
    [SerializeField] private GameObject diaryPanelGO;
    
    [Header("Over UI")]
    [SerializeField] private GameObject overPanelGO;
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
        if (cursorRT != null)
            cursorRT.gameObject.SetActive(false);
        if (diaryPanelGO != null)
            diaryPanelGO.SetActive(false);
        if (torchFuelPanelGO != null)
            torchFuelPanelGO.SetActive(false);
        
        lanternButton?.onClick.AddListener(ToggleLantern);
        diaryButton?.onClick.AddListener(ToggleDiary);
        slicingButton?.onClick.AddListener(ToggleSlice);

        currentTool = Tool.None;
        
        Cursor.visible = true;
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }
    
    public void ToggleLantern() {
        Debug.Log("Lantern button pressed!");

        if (currentTool == Tool.Lantern) {
            currentTool = Tool.None;
            // cursorRT.gameObject.SetActive(false);
            SetButtonSize(lanternButton, 1f);
        }
        else {
            currentTool = Tool.Lantern;
            cursorImage.sprite = lanternCursor;
            // cursorRT.gameObject.SetActive(true);
            SetButtonSize(lanternButton, 1.5f);
        }

        bool isActive = torchFuelPanelGO.activeSelf;
        torchFuelPanelGO.SetActive(!isActive);
    }
    public void ToggleDiary() {
        Debug.Log("Diary button pressed!");

        if (currentTool == Tool.Diary) {
            currentTool = Tool.None;
            // cursorRT.gameObject.SetActive(false);
            SetButtonSize(diaryButton, 1f);
        }
        else {
            currentTool = Tool.Diary;
            cursorImage.sprite = diaryCursor;
            // cursorRT.gameObject.SetActive(true);
            SetButtonSize(diaryButton, 1.5f);
        }

        bool isActive = diaryPanelGO.activeSelf;
        diaryPanelGO.SetActive(!isActive);
    }
    public void ToggleSlice() {
        Debug.Log("Slicing button pressed!");

        if (currentTool == Tool.Slice) {
            currentTool = Tool.None;
            // cursorRT.gameObject.SetActive(false);
            SetButtonSize(slicingButton, 1f);
        }
        else {
            currentTool = Tool.Slice;
            cursorImage.sprite = sliceCursor;
            // cursorRT.gameObject.SetActive(true);
            SetButtonSize(slicingButton, 1.5f);
        }
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

    public void ControlOverUI(bool active) {
        Debug.Log($"ControlOverUI:L {active}");

        overPanelGO.SetActive(active);
    }
}