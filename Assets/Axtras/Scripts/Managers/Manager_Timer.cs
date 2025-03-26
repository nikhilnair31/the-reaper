using UnityEngine;
using UnityEngine.UI;

public class Manager_Timer : MonoBehaviour
{
    #region Vars
    public static Manager_Timer Instance { get; private set; }
    
    [Header("Timer Settings")]
    [SerializeField] private Image fuelAmountImage;
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
        fuelAmountImage.fillAmount = 1f;
    }
    
    public void UpdateTimer(float movementAmount) {
        float drain = fillDrainRate * (1.0f + movementAmount * movementDrainMultiplier) * Time.deltaTime;
        var amtVal = Mathf.Max(0, fuelAmountImage.fillAmount - drain);
        SetTimerFill(amtVal);
        
        if (fuelAmountImage.fillAmount <= 0) {
            TimerEmpty();
        }
    }
    
    public void SetTimerFill(float amount) {
        fuelAmountImage.fillAmount = Mathf.Clamp01(amount);
    }
    
    private void TimerEmpty() {
        Debug.Log("Timer Empty!");

        Manager_UI.Instance.ControlOverUI(true);
    }
}