// Manager_UI.cs
using UnityEngine;
using UnityEngine.UI;

public class Manager_UI : MonoBehaviour
{
    #region Vars
    [Header("UI Elements")]
    [SerializeField] private Button lanternButton;
    [SerializeField] private Button canvasButton;
    [SerializeField] private Button logButton;
    
    [Header("References")]
    [SerializeField] private GameObject lanternObject;
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private Image cursorImage; // Reference to cursor image
    
    private bool isLanternActive = false;
    
    public static Manager_UI Instance { get; private set; }
    #endregion
    
    void Awake()
    {
        // Implement singleton pattern
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
        // Initialize default states
        if (lanternObject != null)
        {
            lanternObject.SetActive(false);
            isLanternActive = false;
        }
        
        if (uiCanvas != null)
        {
            uiCanvas.SetActive(false);
        }
        
        // Turn off cursor image by default
        if (cursorImage != null)
        {
            cursorImage.gameObject.SetActive(false);
        }
        
        // Set up button listeners
        if (lanternButton != null)
        {
            lanternButton.onClick.AddListener(ToggleLantern);
        }
        
        if (canvasButton != null)
        {
            canvasButton.onClick.AddListener(ToggleUICanvas);
        }
        
        if (logButton != null)
        {
            logButton.onClick.AddListener(LogButtonPressed);
        }
    }
    
    public void UpdateCursorPosition(Vector3 position)
    {
        if (cursorImage != null && isLanternActive)
        {
            cursorImage.rectTransform.position = position;
        }
    }
    
    public void ToggleLantern()
    {
        isLanternActive = !isLanternActive;
        
        if (lanternObject != null)
        {
            lanternObject.SetActive(isLanternActive);
        }
        
        if (cursorImage != null)
        {
            cursorImage.gameObject.SetActive(isLanternActive);
        }
        
        // Toggle system cursor visibility (opposite of lantern)
        Cursor.visible = !isLanternActive;
        
        Debug.Log("Lantern toggled: " + (isLanternActive ? "ON" : "OFF"));
    }
    
    public void ToggleUICanvas()
    {
        if (uiCanvas != null)
        {
            bool isActive = uiCanvas.activeSelf;
            uiCanvas.SetActive(!isActive);
            Debug.Log("UI Canvas toggled: " + (!isActive ? "ON" : "OFF"));
        }
    }
    
    public void LogButtonPressed()
    {
        Debug.Log("Log button pressed!");
    }
    
    // Additional public methods for other scripts to access UI functionality
    public bool IsLanternActive()
    {
        return isLanternActive;
    }
    
    public void SetLanternActive(bool active)
    {
        if (isLanternActive != active)
        {
            isLanternActive = active;
            
            if (lanternObject != null)
            {
                lanternObject.SetActive(active);
            }
            
            if (cursorImage != null)
            {
                cursorImage.gameObject.SetActive(active);
            }
            
            // Toggle system cursor visibility (opposite of lantern)
            Cursor.visible = !active;
        }
    }
    
    public void ShowUICanvas(bool show)
    {
        if (uiCanvas != null)
        {
            uiCanvas.SetActive(show);
        }
    }
    
    void OnEnable()
    {
        // Ensure correct cursor state when script is enabled
        if (cursorImage != null)
        {
            cursorImage.gameObject.SetActive(isLanternActive);
        }
        Cursor.visible = !isLanternActive;
    }
    
    void OnDisable()
    {
        // Restore system cursor when script is disabled
        Cursor.visible = true;
    }
}