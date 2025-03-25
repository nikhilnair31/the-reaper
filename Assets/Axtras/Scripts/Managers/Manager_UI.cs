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

    [Header("General")]
    [SerializeField] private Tool currentTool;

    [Header("Game UI")]
    [SerializeField] private GameObject diaryPanelGO;
    [SerializeField] private Image cursorImage;
    [SerializeField] private Button lanternButton;
    [SerializeField] private Button diaryButton;
    [SerializeField] private Button slicingButton;
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
    }
    
    public void UpdateCursorPosition(Vector3 position) {
        if (currentTool == Tool.Lantern) {
            cursorImage.rectTransform.position = position;
        }
    }
    
    public void ToggleLantern() {
        Debug.Log("Lantern button pressed!");

        currentTool = Tool.Lantern;
    }
    public void ToggleDiary() {
        Debug.Log("Diary button pressed!");

        currentTool = Tool.Diary;

        bool isActive = diaryPanelGO.activeSelf;
        diaryPanelGO.SetActive(!isActive);
    }
    public void ToggleSlice() {
        Debug.Log("Slicing button pressed!");

        currentTool = Tool.Slice;
    }

    public bool GetLanternStatus() {
        return currentTool == Tool.Lantern;
    }
}