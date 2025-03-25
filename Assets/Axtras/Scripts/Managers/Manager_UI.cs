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
    [SerializeField] private Sprite lanternCursor;
    [SerializeField] private Sprite diaryCursor;
    [SerializeField] private Sprite sliceCursor;

    [Header("Game UI")]
    [SerializeField] private GameObject diaryPanelGO;
    [SerializeField] private Image cursorImage;
    [SerializeField] private Button lanternButton;
    [SerializeField] private Button diaryButton;
    [SerializeField] private Button slicingButton;
    
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
        if (diaryPanelGO != null)
            diaryPanelGO.SetActive(false);
        if (cursorImage != null)
            cursorImage.gameObject.SetActive(false);
        
        lanternButton?.onClick.AddListener(ToggleLantern);
        diaryButton?.onClick.AddListener(ToggleDiary);
        slicingButton?.onClick.AddListener(ToggleSlice);

        currentTool = Tool.None;
        
        Cursor.visible = true;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    
    public void UpdateCursorPosition(Vector3 position) {
        if (currentTool == Tool.Lantern) {
            cursorImage.rectTransform.position = position;
        }
    }
    
    public void ToggleLantern() {
        Debug.Log("Lantern button pressed!");

        if (currentTool == Tool.Lantern) {
            currentTool = Tool.None;
            cursorImage.gameObject.SetActive(false);
            SetButtonSize(lanternButton, 1f);
        }
        else {
            currentTool = Tool.Lantern;
            cursorImage.sprite = lanternCursor;
            cursorImage.gameObject.SetActive(true);
            SetButtonSize(lanternButton, 1.2f);
        }
    }
    public void ToggleDiary() {
        Debug.Log("Diary button pressed!");

        if (currentTool == Tool.Diary) {
            currentTool = Tool.None;
            cursorImage.gameObject.SetActive(false);
            SetButtonSize(diaryButton, 1f);
        }
        else {
            currentTool = Tool.Diary;
            cursorImage.sprite = diaryCursor;
            cursorImage.gameObject.SetActive(true);
            SetButtonSize(diaryButton, 1.2f);
        }

        bool isActive = diaryPanelGO.activeSelf;
        diaryPanelGO.SetActive(!isActive);
    }
    public void ToggleSlice() {
        Debug.Log("Slicing button pressed!");

        if (currentTool == Tool.Slice) {
            currentTool = Tool.None;
            cursorImage.gameObject.SetActive(false);
            SetButtonSize(slicingButton, 1f);
        }
        else {
            currentTool = Tool.Slice;
            cursorImage.sprite = sliceCursor;
            cursorImage.gameObject.SetActive(true);
            SetButtonSize(slicingButton, 1.2f);
        }
    }

    private void SetButtonSize(Button btn, float sizeVal) {
        var rt = btn.GetComponent<RectTransform>();
        rt.localScale = new (sizeVal, sizeVal, sizeVal);
    }

    public bool GetLanternStatus() {
        return currentTool == Tool.Lantern;
    }

    public void ControlOverUI(bool active) {
        Debug.Log($"ControlOverUI:L {active}");

        overPanelGO.SetActive(active);
    }
}