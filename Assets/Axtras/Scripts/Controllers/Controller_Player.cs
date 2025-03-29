using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Controller_Player : MonoBehaviour
{
    #region Vars
    public static Controller_Player Instance { get; private set; }

    public enum Tool {
        None,
        Lantern,
        Diary,
        Slice,
        Grab
    }
    
    private Vector3 velocity = Vector3.zero;
    private Camera mainCamera;
    private RaycastHit hit;

    [Header("Tool Settings")]
    [SerializeField] private Tool currentTool;
    
    [Header("Movement Settings")]
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private float maxDistance = 10f;
    
    [Header("Lantern Settings")]
    [SerializeField] private Transform lightTransform;
    [SerializeField] private GameObject lightObject;
    [SerializeField] private float boostMul = 2f;
    private bool isBoosting = false;
    private bool isLanternActive = false;

    [Header("Diary Settings")]
    [SerializeField] private Transform diaryTransform;
    private List<string> diaryRulesList;
    
    [Header("Slice Settings")]
    [SerializeField] private float sliceDistance = 10f;
    [SerializeField] private float sliceMinDistance = 5f;
    [SerializeField] private int sliceInterpolatedSteps = 15;
    [SerializeField] private LayerMask sliceLayerMask;
    private HashSet<int> processedObjects = new ();
    private Vector3 lastMousePosition;
    private bool sliceActive = false;
    
    [Header("Grab Settings")]
    [SerializeField] private float grabDistance = 10f;
    [SerializeField] private float grabForce = 1f;
    [SerializeField] private LayerMask grabLayerMask;
    private Transform grabbedObject = null;
    private Vector3 grabOffset = Vector3.zero;
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
        InitLantern();
        InitDiary();
    }
    private void Init() {
        mainCamera = Camera.main;

        currentTool = Tool.None;
    }
    private void InitLantern() {
        lightObject?.SetActive(false);
    }
    private void InitDiary() {
        Manager_Content.Instance.InitDiaryRules();
    }
    private void InitSlice() {
    }
    private void InitGrab() {
    }
    
    private void Update() {
        HandleLantern();
        HandleDiary();
        HandleSlice();
        HandleGrab();
    }
    private void HandleLantern() {
        if (GetToolLantern()) {
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
        if (GetToolDiary()) {
        }
    }
    private void HandleSlice() {
        if (GetToolSlice()) {
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
    private void HandleGrab() {
        if (GetToolGrab()) {
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                Debug.DrawRay(ray.origin, ray.direction * grabDistance, Color.blue, 0.1f);
                if (Physics.Raycast(ray, out hit, grabDistance, grabLayerMask)) {
                    Debug.Log($"Hit object: {hit.collider.name}");
                    if (hit.transform.GetComponent<Rigidbody>() != null) {
                        grabbedObject = hit.transform;
                        grabOffset = grabbedObject.position - hit.point;
                    }
                    else {
                        Debug.Log($"No Rigidbody found on the hit object: {hit.collider.name}");
                    }
                }
            }
            
            if (Input.GetMouseButtonUp(0)) {
                grabbedObject = null;
            }
            
            if (grabbedObject != null) {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                Vector3 targetPosition = ray.origin + ray.direction * grabDistance;
                
                if (Physics.Raycast(ray, out hit, grabDistance, grabLayerMask)) {
                    targetPosition = hit.point + grabOffset;
                }
                
                if (grabbedObject.TryGetComponent(out Rigidbody rb)) {
                    Vector3 direction = targetPosition - grabbedObject.position;
                    rb.linearVelocity = direction * grabForce;
                }
            }
        }
    }

    private void ProcessHit(RaycastHit hit) {   
        Debug.Log($"Slicing object: {hit.collider.name}");

        // Use instance ID to prevent processing the same object multiple times per frame
        int instanceID = hit.transform.GetInstanceID();
        if (processedObjects.Contains(instanceID)) {
            return;
        }
    
        processedObjects.Add(instanceID);
        
        if (hit.transform.CompareTag("Rope")) {
            Debug.Log($"Slicing rope!");
            
            if (hit.transform.gameObject.activeSelf) {
                // Disable the rope object
                hit.transform.gameObject.SetActive(false);

                // Destroy the spring joint component
                var corpse = hit.transform.parent.Find("Corpse");
                if (corpse != null) {
                    if (corpse.TryGetComponent<SpringJoint>(out var spring)) {
                        Destroy(spring);
                    }
                }

                // Add score on rope slice
                Manager_Game.Instance.IncScore();

                // Spawn whispers
                var corpseController = corpse.transform.GetComponentInChildren<Controller_Corpse>();
                corpseController?.SpawnWhisper();
            }
        }
        else if (hit.transform.CompareTag("Corpse")) {
            Debug.Log($"Slicing corpse!");

            // Decrease score on corpse slice
            Manager_Game.Instance.DecScore();

            // Spawn blot on a diary rule
            Controller_Diary.Instance.SpawnBlot();

            // Spawn whispers
            var corpseController = hit.transform.parent.GetComponent<Controller_Corpse>();
            corpseController?.SpawnWhisper();
        }

        // Clear processed objects after processing
        processedObjects.Clear();
    }

    public void SetTool(Tool tool) {
        currentTool = tool;
    }
    
    public Tool GetTool() {
        return currentTool;
    }
    public bool GetToolLantern() {
        return currentTool == Tool.Lantern;
    }
    public bool GetToolDiary() {
        return currentTool == Tool.Diary;
    }
    public bool GetToolSlice() {
        return currentTool == Tool.Slice;
    }
    public bool GetToolGrab() {
        return currentTool == Tool.Grab;
    }
    public bool GetIsLanternOn() {
        return isLanternActive;
    }
    public bool GetIsLanternBoosting() {
        return isBoosting;
    }
}