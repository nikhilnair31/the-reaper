using UnityEngine;

public class Controller_Player : MonoBehaviour
{
    #region Vars
    [Header("General Settings")]
    [SerializeField] private Transform lightTransform;
    [SerializeField] private GameObject lightObject;
    
    [Header("Movement Settings")]
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private float maxDistance = 10f;
    
    private Vector3 velocity = Vector3.zero;
    private Camera mainCamera;
    private Vector3 lastPosition;
    private bool isLightActive = false;
    #endregion 
    
    void Start()
    {
        mainCamera = Camera.main;
        lastPosition = lightTransform != null ? lightTransform.position : Vector3.zero;
        
        if (lightObject != null)
        {
            lightObject.SetActive(false);
        }
        
        // Use the UI Manager to ensure cursor state is correct
        Cursor.visible = Manager_UI.Instance == null || !Manager_UI.Instance.IsLanternActive();
    }
    
    void Update()
    {
        if (Manager_Game.Instance != null && Manager_Game.Instance.IsGameOver)
            return;
            
        Vector3 mousePos = Input.mousePosition;
        
        // Update cursor position through the UI Manager
        if (Manager_UI.Instance != null)
        {
            Manager_UI.Instance.UpdateCursorPosition(mousePos);
        }
        
        bool mousePressed = Input.GetMouseButton(0);
        
        if (lightObject != null)
        {
            lightObject.SetActive(mousePressed);
        }
        
        isLightActive = mousePressed;
        
        if (mousePressed)
        {
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            Vector3 targetPosition = ray.origin + ray.direction * maxDistance;
            
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                targetPosition = hit.point;
            }
            
            if (lightTransform != null)
            {
                lightTransform.position = Vector3.SmoothDamp(
                    lightTransform.position, 
                    targetPosition, 
                    ref velocity, 
                    smoothTime
                );
                
                float movement = Vector3.Distance(lightTransform.position, lastPosition);
                lastPosition = lightTransform.position;
                
                if (Manager_Timer.Instance != null)
                {
                    Manager_Timer.Instance.UpdateTimer(movement);
                }
            }
        }
    }
    
    void OnEnable()
    {
        // Let the UI Manager handle cursor visibility
        if (Manager_UI.Instance != null)
        {
            Cursor.visible = !Manager_UI.Instance.IsLanternActive();
        }
    }
    
    void OnDisable()
    {
        // Restore system cursor when script is disabled
        Cursor.visible = true;
    }
}