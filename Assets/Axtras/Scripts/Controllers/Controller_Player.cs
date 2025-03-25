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
    private bool isLanternActive = false;
    #endregion 
    
    private void Start() {
        mainCamera = Camera.main;
        lastPosition = (lightTransform != null) ? lightTransform.position : Vector3.zero;
        
        lightObject?.SetActive(false);
    }
    
    private void Update() {
        if (Manager_UI.Instance.GetLanternStatus()) {
            Vector3 mousePos = Input.mousePosition;
            Manager_UI.Instance.UpdateCursorPosition(mousePos);

            var clicked = Input.GetMouseButtonDown(0);
            lightObject?.SetActive(clicked);
            
            if (clicked) {
                Ray ray = mainCamera.ScreenPointToRay(mousePos);
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
}