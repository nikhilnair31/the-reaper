using UnityEngine;

public class Controller_Corpse : MonoBehaviour 
{
    #region Vars
    private SpringJoint springJoint;
    private Rigidbody objectRigidbody;

    [Header("Rope Settings")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private bool createSpringJoint = true;
    [SerializeField] private float objectMass = 1.0f;
    [SerializeField] private float spring = 10.0f;
    [SerializeField] private float damper = 0.2f;
    [SerializeField] private float minDistance = 0.2f;
    [SerializeField] private float maxDistance = 2.0f;
    #endregion 
    
    private void Start() {
        if (pointA == null || pointB == null) {
            Debug.LogError("Missing transform references!");
            enabled = false;
            return;
        }
        if (lineRenderer == null) {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }
        
        lineRenderer.positionCount = 2;
        
        if (createSpringJoint) {
            SetupSpringPhysics();
        }
    }
    
    private void Update() {
        if (lineRenderer != null && pointA != null && pointB != null) {
            lineRenderer.SetPosition(0, pointA.position);
            lineRenderer.SetPosition(1, pointB.position);
        }
    }
    
    private void SetupSpringPhysics() {
        objectRigidbody = pointB.GetComponent<Rigidbody>();
        if (objectRigidbody == null) {
            objectRigidbody = pointB.gameObject.AddComponent<Rigidbody>();
            objectRigidbody.mass = objectMass;
            objectRigidbody.useGravity = true;
            objectRigidbody.linearDamping = 0.5f;
        }
        
        springJoint = pointB.gameObject.AddComponent<SpringJoint>();
        springJoint.connectedBody = pointA.GetComponent<Rigidbody>();
        
        if (springJoint.connectedBody == null) {
            Rigidbody connectedBody = pointA.gameObject.AddComponent<Rigidbody>();
            connectedBody.isKinematic = true;
            springJoint.connectedBody = connectedBody;
        }
        
        springJoint.spring = spring;
        springJoint.damper = damper;
        springJoint.minDistance = minDistance;
        springJoint.maxDistance = maxDistance;
        
        springJoint.autoConfigureConnectedAnchor = false;
        springJoint.anchor = Vector3.zero;
        springJoint.connectedAnchor = Vector3.zero;
        
        springJoint.enableCollision = true;
        
        springJoint.autoConfigureConnectedAnchor = false;
    }       
}