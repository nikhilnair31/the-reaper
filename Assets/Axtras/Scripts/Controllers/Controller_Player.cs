using UnityEngine;

public class Controller_Player : MonoBehaviour
{
    #region Vars
    private Vector3 velocity = Vector3.zero;
    private Camera mainCamera;
    private RaycastHit hit;
    private Vector3 lastPosition;
    
    [Header("Movement Settings")]
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private float maxDistance = 10f;
    
    [Header("Lantern Settings")]
    [SerializeField] private Transform lightTransform;
    [SerializeField] private GameObject lightObject;
    [SerializeField] private float boostMul = 2f;
    private bool isBoosting = false;
    private bool isLanternActive = false;
    
    [Header("Slice Settings")]
    [SerializeField] private float sliceDistance = 10f;
    #endregion 
    
    private void Start() {
        mainCamera = Camera.main;
        lastPosition = (lightTransform != null) ? lightTransform.position : Vector3.zero;
        
        lightObject?.SetActive(false);
    }
    
    private void Update() {
        HandleLantern();
        HandleDiary();
        HandleSlice();
    }
    private void HandleLantern() {
        if (Manager_UI.Instance.GetLanternStatus()) {
            if (Input.GetKeyDown(KeyCode.F)) {
                isBoosting = !isBoosting;
                var light = lightObject.GetComponent<Light>();
                light.intensity = isBoosting ? light.intensity * boostMul : light.intensity / boostMul;
            }

            if (Input.GetMouseButtonDown(0)) {
                isLanternActive = !isLanternActive;
                lightObject?.SetActive(isLanternActive);
            }
            
            if (isLanternActive) {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                Vector3 targetPosition = ray.origin + ray.direction * maxDistance;
                if (Physics.Raycast(ray, out hit, maxDistance)) {
                    targetPosition = hit.point;
                }
                
                lightTransform.position = Vector3.SmoothDamp(
                    lightTransform.position, 
                    targetPosition, 
                    ref velocity, 
                    smoothTime
                );
                
                float movement = Vector3.Distance(lightTransform.position, lastPosition);
                lastPosition = lightTransform.position;
                Manager_Timer.Instance.UpdateTimer(movement);
            }
        }
    }
    private void HandleDiary() {
        if (Manager_UI.Instance.GetDiaryStatus()) {
        }
    }
    private void HandleSlice() {
        if (Manager_UI.Instance.GetSliceStatus()) {
            if (Input.GetMouseButton(0)) {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                Debug.DrawRay(ray.origin, ray.direction * sliceDistance, Color.red, 0.1f);
                
                if (Physics.Raycast(ray, out hit, sliceDistance)) {
                    Debug.Log($"Slicing object: {hit.collider.name}");

                    if (hit.transform.CompareTag("Rope")) {
                        Debug.Log($"Slicing rope!");

                        // Disable rope
                        hit.transform.gameObject.SetActive(false);
                        // Get corpse and disable spring
                        var corpse = hit.transform.parent.Find("Person");
                        var spring = corpse.GetComponent<SpringJoint>();
                        Destroy(spring);
                    }
                    else if (hit.transform.CompareTag("Corpse")) {
                        Debug.Log($"Slicing corpse!");
                    }
                }
            }
        }
    }
}