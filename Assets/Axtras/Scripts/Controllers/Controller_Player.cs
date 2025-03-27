using UnityEngine;
using System.Collections.Generic;

public class Controller_Player : MonoBehaviour
{
    #region Vars
    public static Controller_Player Instance { get; private set; }
    
    private Vector3 velocity = Vector3.zero;
    private Camera mainCamera;
    private RaycastHit hit;
    
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
    [SerializeField] private float sliceMinDistance = 5f;
    [SerializeField] private int sliceInterpolatedSteps = 15;
    [SerializeField] private LayerMask sliceLayerMask;
    private HashSet<int> processedObjects = new ();
    private Vector3 lastMousePosition;
    private bool sliceActive = false;
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
        mainCamera = Camera.main;
        
        lightObject?.SetActive(false);
    }
    
    private void Update() {
        HandleLantern();
        HandleDiary();
        HandleSlice();
    }
    private void HandleLantern() {
        if (Manager_UI.Instance.GetLanternStatus()) {
            if (Input.GetMouseButtonDown(1)) {
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
            }
        }
    }
    private void HandleDiary() {
        if (Manager_UI.Instance.GetDiaryStatus()) {
        }
    }
    private void HandleSlice() {
        if (Manager_UI.Instance.GetSliceStatus()) {
            if (Input.GetMouseButtonDown(0)) {
                sliceActive = true;
                lastMousePosition = Input.mousePosition;
            }
            
            if (Input.GetMouseButtonUp(0)) {
                sliceActive = false;
            }
            
            if (sliceActive) {
                Vector3 currentMousePosition = Input.mousePosition;
                Ray currentRay = mainCamera.ScreenPointToRay(currentMousePosition);
                Debug.DrawRay(currentRay.origin, currentRay.direction * sliceDistance, Color.red, 0.1f);
                
                if (Physics.Raycast(currentRay, out hit, sliceDistance, sliceLayerMask)) {
                    ProcessHit(hit);
                }
                
                if (lastMousePosition != Vector3.zero && Vector2.Distance(lastMousePosition, currentMousePosition) > sliceMinDistance) {
                    int steps = Mathf.CeilToInt(Vector2.Distance(lastMousePosition, currentMousePosition) / sliceMinDistance);
                    steps = Mathf.Min(steps, sliceInterpolatedSteps); // Cap maximum steps to avoid performance issues
                    
                    for (int i = 1; i < steps; i++) {
                        Vector3 lerpPosition = Vector3.Lerp(lastMousePosition, currentMousePosition, (float)i / steps);
                        Ray lerpRay = mainCamera.ScreenPointToRay(lerpPosition);
                        
                        Debug.DrawRay(lerpRay.origin, lerpRay.direction * sliceDistance, Color.yellow, 0.1f);
                        
                        if (Physics.Raycast(lerpRay, out hit, sliceDistance, sliceLayerMask)) {
                            ProcessHit(hit);
                        }
                    }
                }
                
                lastMousePosition = currentMousePosition;
            }
        }
    }

    private void ProcessHit(RaycastHit hit) {        
        // Use instance ID to prevent processing the same object multiple times per frame
        int instanceID = hit.transform.GetInstanceID();
        if (processedObjects.Contains(instanceID)) {
            return;
        }
    
        processedObjects.Add(instanceID);
        Debug.Log($"Slicing object: {hit.collider.name}");
        
        if (hit.transform.CompareTag("Rope")) {
            Debug.Log($"Slicing rope!");
            
            if (hit.transform.gameObject.activeSelf) {
                hit.transform.gameObject.SetActive(false);
                var corpse = hit.transform.parent.Find("Corpse");
                if (corpse != null) {
                    if (corpse.TryGetComponent<SpringJoint>(out var spring)) {
                        Destroy(spring);
                    }
                }
            }
        }
        else if (hit.transform.CompareTag("Corpse")) {
            Debug.Log($"Slicing corpse!");
            // Add desired functionality here
        }
    }

    public bool GetIsLanternBoosting() {
        return isBoosting;
    }
}