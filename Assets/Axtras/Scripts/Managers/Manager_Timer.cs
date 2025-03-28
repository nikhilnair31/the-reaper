using UnityEngine;
using UnityEngine.UI;

public class Manager_Timer : MonoBehaviour
{
    #region Vars
    public static Manager_Timer Instance { get; private set; }
    
    [Header("Fuel Settings")]
    [SerializeField] private Image fuelAmountImage;
    [SerializeField] private float fuelNormalDrainMul = 1f;
    [SerializeField] private float fuelBoostDrainMul = 2f;
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
    }
    private void Init() {
        fuelAmountImage.fillAmount = 1f;
    }

    private void Update() {
        HandleLantern();
    }
    private void HandleLantern() {
        if (Controller_Player.Instance.GetToolLantern()) {
            UpdateFuelAmount();
        }
    }

    public void UpdateFuelAmount() {
        var drainMul = Controller_Player.Instance.GetIsLanternBoosting() ? fuelBoostDrainMul : fuelNormalDrainMul;
        var drainAmt = drainMul * Time.deltaTime;
        var amtVal = Mathf.Max(0, fuelAmountImage.fillAmount - drainAmt);

        SetFuelFill(amtVal);
        
        if (fuelAmountImage.fillAmount <= 0) {
            FuelEmpty();
        }
    }
    private void SetFuelFill(float amount) {
        fuelAmountImage.fillAmount = Mathf.Clamp01(amount);
    }
    private void FuelEmpty() {
        Debug.Log("Fuel Empty!");

        Manager_UI.Instance.ControlOverUI(true);
    }
}