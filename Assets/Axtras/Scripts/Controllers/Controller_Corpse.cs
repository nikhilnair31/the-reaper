using UnityEngine;

public class Controller_Corpse : MonoBehaviour 
{
    #region Vars
    [Header("Rope Settings")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private LineRenderer lineRenderer;
    #endregion 
    
    private void Start() {
    }
    
    private void Update() {
        lineRenderer.SetPosition(0, pointA.position);
        lineRenderer.SetPosition(1, pointB.position);
    }      
}