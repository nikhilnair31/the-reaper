using UnityEngine;
using System.Collections.Generic;

public class Controller_Corpse : MonoBehaviour 
{
    #region Variables
    private SphereCollider sphereCollider;
    private GameObject corpseGO;

    [Header("General Settings")]
    [SerializeField] private MeshRenderer corpseMeshRend;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private bool visualizeRays = true;
    [SerializeField] private LayerMask raycastLayerMask;
    
    [Header("Tattoo Settings")]
    [SerializeField] private GameObject[] tattooPrefabs;
    [SerializeField] private float tattooSize = 0.1f;
    [SerializeField] private int maxTattoos = 10;
    [SerializeField] private int tattooRayCount = 100;
    [SerializeField] private float tattooRayHitChance = 0.05f;
    
    [Header("Scar Settings")]
    [SerializeField] private GameObject[] scarPrefabs;
    [SerializeField] private float scarSize = 0.15f;
    [SerializeField] private int maxScars = 5;
    [SerializeField] private int scarRayCount = 100;
    [SerializeField] private float scarRayHitChance = 0.05f;

    [Header("Aura Settings")]
    [SerializeField] private GameObject[] auraPrefabs;
    [SerializeField] private float auraSpawnChance = 0.5f;
    #endregion
    
    private void Start() {
        if (corpseGO == null)
            corpseGO = transform.Find("Corpse").gameObject;
        if (sphereCollider == null)
            sphereCollider = GetComponent<SphereCollider>();

        RandomizeBaseTexture();
        RandomizeTattoos();
        RandomizeScars();
        RandomizeAuras();
    }
    private void RandomizeBaseTexture() {
        if (corpseMeshRend) {
            Material[] mats = corpseMeshRend.materials;
            var mat = mats[0];
            mat.mainTexture = Resources.Load<Texture2D>("Textures/Corpse/Corpse_" + Random.Range(1, 4));
            corpseMeshRend.materials = mats;
        }
    }
    private void RandomizeTattoos() {
        if (tattooPrefabs == null || tattooPrefabs.Length == 0)
            return;
            
        var hits = CastRaysInward();
        int tattooCount = 0;
        
        foreach (var hit in hits) {
            if (tattooCount >= maxTattoos)
                break;
                
            GameObject prefab = tattooPrefabs[Random.Range(0, tattooPrefabs.Length)];
            SpawnDecal(hit, prefab, tattooSize);
            tattooCount++;
        }
    }
    private void RandomizeScars() {
        if (scarPrefabs == null || scarPrefabs.Length == 0)
            return;
            
        var hits = CastRaysInward(tattooRayCount, tattooRayHitChance);
        int scarCount = 0;
        
        foreach (var hit in hits) {
            if (scarCount >= maxScars)
                break;
                
            GameObject prefab = scarPrefabs[Random.Range(0, scarPrefabs.Length)];
            SpawnDecal(hit, prefab, scarSize);
            scarCount++;
        }
    }
    private void RandomizeAuras() {
        if (auraPrefabs == null || auraPrefabs.Length == 0 || Random.value > auraSpawnChance)
            return;
            
        GameObject auraPrefab = auraPrefabs[Random.Range(0, auraPrefabs.Length)];
        
        GameObject aura = Instantiate(auraPrefab, corpseGO.transform);
        aura.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    private List<RaycastHit> CastRaysInward(int rayCount = 100, float rayHitChance = 0.05f) {
        List<RaycastHit> allHits = new();
        
        Vector3 center = transform.position;
        float radius = 1.0f;
        
        if (sphereCollider != null) {
            center = transform.TransformPoint(sphereCollider.center);
            radius = sphereCollider.radius * Mathf.Max(
                transform.localScale.x,
                transform.localScale.y,
                transform.localScale.z
            );
        }
        else if (meshFilter != null) {
            Bounds bounds = meshFilter.sharedMesh.bounds;
            radius = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
            center = transform.TransformPoint(bounds.center);
        }
        
        Vector3[] points = GeneratePointsOnSphere(rayCount, radius, center);
        
        foreach (Vector3 point in points) {
            if (Random.value > rayHitChance)
                continue;
                
            Vector3 direction = (center - point).normalized;
            float distance = Vector3.Distance(point, center);
            
            Ray ray = new(point, direction);

            bool didHit = Physics.Raycast(ray, out RaycastHit hit, distance, raycastLayerMask);
            
            if (!didHit && meshFilter != null){
                didHit = RaycastAgainstMesh(ray, out hit, distance);
            }
            
            if (didHit && (hit.collider == null || hit.collider != sphereCollider)) {
                allHits.Add(hit);
                
                if (visualizeRays)
                    Debug.DrawLine(point, hit.point, Color.green, 0.1f);
            }
            else if (visualizeRays)
                Debug.DrawLine(point, center, Color.red, 0.1f);
        }

        return allHits;
    }
    private bool RaycastAgainstMesh(Ray ray, out RaycastHit hit, float maxDistance) {
        hit = new RaycastHit();
        
        if (meshFilter == null || meshFilter.sharedMesh == null)
            return false;
            
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        
        Matrix4x4 worldToLocal = transform.worldToLocalMatrix;
        Ray localRay = new (
            worldToLocal.MultiplyPoint3x4(ray.origin),
            worldToLocal.MultiplyVector(ray.direction)
        );
        
        float nearestHit = maxDistance;
        bool didHit = false;
        
        for (int i = 0; i < triangles.Length; i += 3) {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];
            
            if (RayTriangleIntersection(localRay, v0, v1, v2, out float distance)) {
                if (distance < nearestHit) {
                    nearestHit = distance;
                    
                    Vector3 localHitPoint = localRay.origin + localRay.direction * distance;
                    hit.point = transform.TransformPoint(localHitPoint);
                    
                    Vector3 localNormal = Vector3.Cross(v1 - v0, v2 - v0).normalized;
                    hit.normal = transform.TransformDirection(localNormal);
                    
                    hit.distance = Vector3.Distance(ray.origin, hit.point);
                    didHit = true;
                }
            }
        }
        
        return didHit;
    }
    private bool RayTriangleIntersection(Ray ray, Vector3 v0, Vector3 v1, Vector3 v2, out float distance){
        Vector3 e1 = v1 - v0;
        Vector3 e2 = v2 - v0;
        Vector3 h = Vector3.Cross(ray.direction, e2);
        float a = Vector3.Dot(e1, h);
        
        distance = 0;
        
        if (a > -Mathf.Epsilon && a < Mathf.Epsilon)
            return false;
            
        float f = 1.0f / a;
        Vector3 s = ray.origin - v0;
        float u = f * Vector3.Dot(s, h);
        
        if (u < 0.0f || u > 1.0f)
            return false;
            
        Vector3 q = Vector3.Cross(s, e1);
        float v = f * Vector3.Dot(ray.direction, q);
        
        if (v < 0.0f || u + v > 1.0f)
            return false;
            
        distance = f * Vector3.Dot(e2, q);
        
        return distance > Mathf.Epsilon;
    }
    private Vector3[] GeneratePointsOnSphere(int count, float radius, Vector3 center) {
        Vector3[] points = new Vector3[count];
        
        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;
        
        for (int i = 0; i < count; i++) {
            float t = (float)i / count;
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = angleIncrement * i;
            
            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = Mathf.Cos(inclination);
            
            points[i] = new Vector3(x, y, z) * radius + center;
        }
        
        return points;
    }
    
    private void SpawnDecal(RaycastHit hit, GameObject prefab, float size) {
        if (prefab == null)
            return; 
                   
        GameObject decal = Instantiate(prefab, hit.point, Quaternion.identity);        
        decal.transform.forward = hit.normal;        
        decal.transform.position += hit.normal * 0.01f;        
        decal.transform.Rotate(hit.normal, Random.Range(0, 360), Space.World);        
        
        float scale = Random.Range(size * 0.7f, size * 1.3f);
        decal.transform.localScale = new Vector3(scale, scale, scale);    

        if (hit.collider != null)
            decal.transform.SetParent(hit.collider.transform);
    }
}