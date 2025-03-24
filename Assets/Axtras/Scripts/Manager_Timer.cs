using UnityEngine;
using UnityEngine.UI;

public class Manager_Timer : MonoBehaviour
{
    #region Vars
    [Header("Timer Settings")]
    [SerializeField] private Image timerImage;
    [SerializeField] private float fillDrainRate = 0.05f;
    [SerializeField] private float movementDrainMultiplier = 1.5f;
    
    private bool isActive = true;
    
    public static Manager_Timer Instance { get; private set; }
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
        if (timerImage != null)
        {
            timerImage.type = Image.Type.Filled;
            timerImage.fillMethod = Image.FillMethod.Radial360;
            timerImage.fillAmount = 1.0f;
        }
    }
    
    public float UpdateTimer(float movementAmount)
    {
        if (!isActive || timerImage == null)
            return 0;
        
        float drain = fillDrainRate * (1.0f + movementAmount * movementDrainMultiplier) * Time.deltaTime;
        timerImage.fillAmount = Mathf.Max(0, timerImage.fillAmount - drain);
        
        if (timerImage.fillAmount <= 0 && isActive)
        {
            TimerEmpty();
        }
        
        return timerImage.fillAmount;
    }
    
    public void SetTimerFill(float amount)
    {
        if (timerImage != null)
        {
            timerImage.fillAmount = Mathf.Clamp01(amount);
        }
    }
    
    public void AddTimerFill(float amount)
    {
        if (timerImage != null)
        {
            timerImage.fillAmount = Mathf.Clamp01(timerImage.fillAmount + amount);
            
            if (timerImage.fillAmount > 0 && !isActive)
            {
                isActive = true;
            }
        }
    }
    
    private void TimerEmpty()
    {
        isActive = false;
        
        if (Manager_Game.Instance != null)
        {
            Manager_Game.Instance.TriggerGameOver();
        }
    }
    
    public void SetTimerActive(bool active)
    {
        isActive = active;
    }
}