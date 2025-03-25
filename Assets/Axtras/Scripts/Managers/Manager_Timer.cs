using UnityEngine;
using UnityEngine.UI;

public class Manager_Timer : MonoBehaviour
{
    #region Vars
    public static Manager_Timer Instance { get; private set; }
    
    [Header("Timer Settings")]
    [SerializeField] private Image timerImage;
    [SerializeField] private float fillDrainRate = 0.05f;
    [SerializeField] private float movementDrainMultiplier = 1.5f;
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
        if (timerImage != null) {
            timerImage.type = Image.Type.Filled;
            timerImage.fillMethod = Image.FillMethod.Radial360;
            timerImage.fillAmount = 1.0f;
        }
    }
    
    public void UpdateTimer(float movementAmount) {
        float drain = fillDrainRate * (1.0f + movementAmount * movementDrainMultiplier) * Time.deltaTime;
        var amtVal = Mathf.Max(0, timerImage.fillAmount - drain);
        SetTimerFill(amtVal);
        
        if (timerImage.fillAmount <= 0) {
            TimerEmpty();
        }
    }
    
    public void SetTimerFill(float amount) {
        timerImage.fillAmount = Mathf.Clamp01(amount);
    }
    
    private void TimerEmpty() {
        Debug.Log("Timer Empty!");

        Manager_UI.Instance.ControlOverUI(true);
    }
}