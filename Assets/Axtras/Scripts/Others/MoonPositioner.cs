using UnityEngine;

[ExecuteInEditMode]
public class MoonPositioner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Material skyboxMaterial;
    [SerializeField] private Transform sunTransform; // Optional: to calculate moon phase

    [Header("Moon Settings")]
    [SerializeField] private float moonElevation = 45f; // Angle above horizon in degrees
    [SerializeField] private float moonAzimuth = 0f;    // Direction angle in degrees (0 = North, 90 = East)
    [SerializeField] [Range(0f, 1f)] private float moonRadius = 0.05f;
    [SerializeField] [Range(-16f, 16f)] private float moonExposure = 0f;
    
    [Header("Animation")]
    [SerializeField] private bool animateMoon = false;
    [SerializeField] private float cycleTimeInMinutes = 24f;
    [SerializeField] private float timeOfDay = 0f; // 0-24 hour format
    
    private Vector3 _moonDirection;
    private Matrix4x4 _moonSpaceMatrix;
    
    private void Start()
    {
        if (skyboxMaterial == null)
        {
            // Try to get the current skybox material
            skyboxMaterial = RenderSettings.skybox;
        }
    }
    
    private void Update()
    {
        if (skyboxMaterial == null) return;
        
        if (animateMoon)
        {
            // Use game time
            float animationTime = Application.isPlaying ? 
                Time.time / (cycleTimeInMinutes * 60f) % 1 : 
                timeOfDay / 24f % 1;
            
            // Simple circular motion
            // moonAzimuth = animationTime * 360f;
            // moonElevation = Mathf.Sin(animationTime * 2 * Mathf.PI) * 90f;
        }
        
        // Calculate moon direction from elevation and azimuth
        _moonDirection = GetDirectionFromAngles(moonElevation, moonAzimuth);
        
        // Update shader properties
        skyboxMaterial.SetVector("_MoonDir", _moonDirection);
        skyboxMaterial.SetFloat("_MoonRadius", moonRadius);
        skyboxMaterial.SetFloat("_MoonExposure", moonExposure);
        
        // Calculate and set moon space matrix for proper moon texture orientation
        UpdateMoonSpaceMatrix();
    }
    
    private Vector3 GetDirectionFromAngles(float elevation, float azimuth)
    {
        float elevRad = elevation * Mathf.Deg2Rad;
        float azimuthRad = azimuth * Mathf.Deg2Rad;
        
        // Convert spherical to Cartesian coordinates
        float y = Mathf.Sin(elevRad);
        float horizontalRadius = Mathf.Cos(elevRad);
        float x = horizontalRadius * Mathf.Sin(azimuthRad);
        float z = horizontalRadius * Mathf.Cos(azimuthRad);
        
        return new Vector3(x, y, z).normalized;
    }
    
    private void UpdateMoonSpaceMatrix()
    {
        // Create a rotation matrix that orients the moon texture correctly
        // First, calculate the up vector (perpendicular to moon direction)
        Vector3 upVector = Vector3.up;
        if (Mathf.Abs(_moonDirection.y) > 0.99f)
            upVector = Vector3.forward; // Use different up vector when moon is directly overhead
            
        Vector3 rightVector = Vector3.Cross(upVector, _moonDirection).normalized;
        upVector = Vector3.Cross(_moonDirection, rightVector).normalized;
        
        // Create the moon space matrix
        _moonSpaceMatrix = Matrix4x4.identity;
        _moonSpaceMatrix.SetColumn(0, rightVector);
        _moonSpaceMatrix.SetColumn(1, upVector);
        _moonSpaceMatrix.SetColumn(2, _moonDirection);
        
        skyboxMaterial.SetMatrix("_MoonSpaceMatrix", _moonSpaceMatrix);
    }
    
    // Helper method to set the moon's position directly from an angle
    public void SetMoonPosition(float elevationAngle, float azimuthAngle)
    {
        moonElevation = elevationAngle;
        moonAzimuth = azimuthAngle;
        animateMoon = false;
        
        // Force update
        Update();
    }
    
    // Optional: Calculate moon phase based on sun and moon positions
    public float CalculateMoonPhase()
    {
        if (sunTransform == null) return 0.5f;
        
        Vector3 sunDirection = sunTransform.forward;
        float moonSunDot = Vector3.Dot(_moonDirection, sunDirection);
        
        // Convert dot product to 0-1 range where 0 is new moon and 1 is full moon
        return (1 - moonSunDot) * 0.5f;
    }
    
    // Draw a gizmo in the editor to show where the moon is
    private void OnDrawGizmosSelected()
    {
        if (!enabled) return;
        
        Gizmos.color = Color.yellow;
        Vector3 moonPos = _moonDirection * 10f; // Draw 10 units away
        Gizmos.DrawWireSphere(moonPos, moonRadius * 2);
        Gizmos.DrawLine(Vector3.zero, moonPos);
    }
}