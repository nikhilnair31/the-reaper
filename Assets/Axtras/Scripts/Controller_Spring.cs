using UnityEngine;

public class Controller_Spring : MonoBehaviour
{
    #region Vars
    [Header("General Settings")]
    [SerializeField] private Transform pointA;  // First transform
    [SerializeField] private Transform pointB;  // Second transform
    [SerializeField] private LineRenderer lineRenderer;  // Reference to the LineRenderer component
    
    [Header("Spring Settings")]
    [SerializeField] private bool createSpringJoint = true;  // Option to create spring joint
    [SerializeField] private float objectMass = 1.0f;  // Mass for the object
    [SerializeField] private float spring = 10.0f;  // Spring force
    [SerializeField] private float damper = 0.2f;  // Damping effect
    [SerializeField] private float minDistance = 0.2f;  // Minimum distance before spring takes effect
    [SerializeField] private float maxDistance = 2.0f;  // Maximum distance spring can stretch
    
    private SpringJoint springJoint;
    private Rigidbody objectRigidbody;
    #endregion 
    
    void Start()
    {
        // Check if we have all required components
        if (pointA == null || pointB == null)
        {
            Debug.LogError("Missing transform references!");
            enabled = false;
            return;
        }
        
        // Create line renderer if not assigned
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }
        
        // Configure the line renderer
        lineRenderer.positionCount = 2;
        
        // Set up physics if requested
        if (createSpringJoint)
        {
            SetupSpringPhysics();
        }
    }
    
    void Update()
    {
        // Update line renderer positions each frame
        if (lineRenderer != null && pointA != null && pointB != null)
        {
            lineRenderer.SetPosition(0, pointA.position);
            lineRenderer.SetPosition(1, pointB.position);
        }
    }
    
    void SetupSpringPhysics()
    {
        // Make sure point B has a rigidbody
        objectRigidbody = pointB.GetComponent<Rigidbody>();
        if (objectRigidbody == null)
        {
            objectRigidbody = pointB.gameObject.AddComponent<Rigidbody>();
            objectRigidbody.mass = objectMass;
            objectRigidbody.useGravity = true;
            objectRigidbody.linearDamping = 0.5f;  // Add some drag for stability
        }
        
        // Add spring joint to point B
        springJoint = pointB.gameObject.AddComponent<SpringJoint>();
        springJoint.connectedBody = pointA.GetComponent<Rigidbody>();
        
        // If point A doesn't have a rigidbody, create one and make it kinematic
        if (springJoint.connectedBody == null)
        {
            Rigidbody connectedBody = pointA.gameObject.AddComponent<Rigidbody>();
            connectedBody.isKinematic = true;  // Make it immovable
            springJoint.connectedBody = connectedBody;
        }
        
        // Configure the spring
        springJoint.spring = spring;
        springJoint.damper = damper;
        springJoint.minDistance = minDistance;
        springJoint.maxDistance = maxDistance;
        
        // Configure additional spring settings
        springJoint.autoConfigureConnectedAnchor = false;
        springJoint.anchor = Vector3.zero;
        springJoint.connectedAnchor = Vector3.zero;
        
        // Enable collision between connected objects
        springJoint.enableCollision = true;
        
        // Disable auto-configuration of spring parameters
        springJoint.autoConfigureConnectedAnchor = false;
    }
}