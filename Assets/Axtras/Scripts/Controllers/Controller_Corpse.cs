using UnityEngine;
using System.Collections.Generic;
using GogoGaga.OptimizedRopesAndCables;
using DG.Tweening;
using TMPro;
using System.Linq;

public class Controller_Corpse : MonoBehaviour 
{
    #region Variables
    private SphereCollider sphereCollider;
    private GameObject corpseGO;
    private Rigidbody corpseRb;
    private Rope rope;

    [Header("General Settings")]
    [SerializeField] private SkinnedMeshRenderer corpseMeshRend;
    [SerializeField] private bool visualizeRays = true;
    [SerializeField] private LayerMask raycastLayerMask;
    [SerializeField] private Data_CorpseFeature features = new();
    
    [Header("Physics Settings")]
    [SerializeField] private float corpseMass = 70f;
    [SerializeField] private float ropeSpringStrength = 700f;
    [SerializeField] private float randSpringStrenthPerc = 0.1f;
    
    [Header("Tattoo Settings")]
    [SerializeField] private SO_Tattoo[] tattooObjects;
    [SerializeField] private float tattooSize = 0.1f;
    [SerializeField] private int maxTattoos = 10;
    [SerializeField] private int tattooRayCount = 100;
    [SerializeField] private float tattooRayHitChance = 0.05f;
    
    [Header("Scar Settings")]
    [SerializeField] private SO_Scar[] scarObjects;
    [SerializeField] private float scarSize = 0.15f;
    [SerializeField] private int maxScars = 5;
    [SerializeField] private int scarRayCount = 100;
    [SerializeField] private float scarRayHitChance = 0.05f;

    [Header("Aura Settings")]
    [SerializeField] private SO_Aura[] auraObjects;
    [SerializeField] private int maxAuras = 3;
    [SerializeField] private float auraSpawnChance = 0.5f;
    
    [Header("Limb Settings")]
    [SerializeField] private SO_Limb[] limbsObjects;
    [SerializeField] private Transform[] limbsMissingArray;
    [SerializeField] private float limbMissingChance = 0.05f;

    [Header("Whispers Settings")]
    [SerializeField] private GameObject whisperPrefab;
    [SerializeField] private float whipserMoveUpAmount = 3f;
    [SerializeField] private float whipserTweenTime = 3f;
    private bool spawnedWhisper = false;
    #endregion
    
    private void Start() {
        if (corpseGO == null)
            corpseGO = transform.Find("Corpse").gameObject;
        if (sphereCollider == null)
            sphereCollider = GetComponentInChildren<SphereCollider>();
        if (rope == null)
            rope = transform.GetComponentInChildren<Rope>();

        RandomizeBaseTexture();
        RandomizeTattoos();
        RandomizeScars();
        RandomizeAuras();
        RandomizeLimbsMissing();
        RandomizePhysics();
    }
    private void RandomizeBaseTexture() {
        if (corpseMeshRend) {
            Material[] mats = corpseMeshRend.materials;
            var mat = mats[0];
            var textures = Resources.LoadAll<Texture2D>("Textures/Corpse")
                .Select(t => t.name)
                .ToArray();
            var randomTextureName = textures[Random.Range(0, textures.Length)];
            mat.mainTexture = Resources.Load<Texture2D>("Textures/Corpse/" + randomTextureName);
            corpseMeshRend.materials = mats;
        }
    }
    private void RandomizeTattoos() {
        if (tattooObjects == null || tattooObjects.Length == 0) return;
            
        var hits = CastRaysInward();
        int tattooCount = 0;
        
        foreach (var hit in hits) {
            if (tattooCount >= maxTattoos) break;
                
            var obj = tattooObjects[Random.Range(0, tattooObjects.Length)];
            var prefab = obj.tattoo_decal;
            SpawnDecal(hit, prefab, tattooSize);
            features.appliedTattoos.Add(obj.tattoo_name);
            
            tattooCount++;
        }
    }
    private void RandomizeScars() {
        if (scarObjects == null || scarObjects.Length == 0) return;
            
        var hits = CastRaysInward(tattooRayCount, tattooRayHitChance);
        var scarCount = 0;
        
        foreach (var hit in hits) {
            if (scarCount >= maxScars) break;
                
            var obj = scarObjects[Random.Range(0, scarObjects.Length)];
            var prefab = obj.scar_decal;
            SpawnDecal(hit, prefab, scarSize);
            features.appliedScars.Add(obj.scar_name);
            
            scarCount++;
        }
    }
    private void RandomizeAuras() {
        if (auraObjects == null || auraObjects.Length == 0 || Random.value > auraSpawnChance) return;
            
        for (int i = 0; i < maxAuras; i++) {
            var obj = auraObjects[Random.Range(0, auraObjects.Length)];
            var prefab = obj.aura_particles;
            var aura = Instantiate(prefab, corpseGO.transform);
            aura.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            features.appliedAura.Add(obj.aura_name);
        }
    }
    private void RandomizeLimbsMissing() {
        if (limbsMissingArray == null || limbsMissingArray.Length == 0) return;
            
        foreach (var limb in limbsMissingArray) {
            if (Random.value < limbMissingChance) {
                var limb_name = limb.name;
                var limb_name_wo_side = limb.name.Substring(0, limb_name.Length -2);
                features.missingLimbs.Add(limb_name_wo_side);
                limb.localScale = Vector3.zero;
            }
        }
    }
    private void RandomizePhysics() {
        if (corpseGO.TryGetComponent(out corpseRb)) {
            corpseRb.isKinematic = false;
            corpseRb.useGravity = true;
            corpseRb.mass = corpseMass;
        }
        
        if (corpseGO.TryGetComponent<SpringJoint>(out var spring)) {
            spring.spring = ropeSpringStrength * Random.Range(1 - randSpringStrenthPerc, 1 + randSpringStrenthPerc);
        }
    }

    public void ValidateAllRules() {
        Debug.Log($"ValidateAllRules");
        
        var allRules = Manager_Content.Instance.GetRules();
        Debug.Log($"allRules Count: {allRules.Count}");
        
        var unlockedRules = allRules.Where(rule => rule.is_unlocked).ToList();
        Debug.Log($"unlockedRules: {unlockedRules.Count}");
        
        foreach (var rule in unlockedRules) {
            var check = ValidateRule(rule);
            Debug.Log($"check: {check}");
        }
    }
    private bool ValidateRule(Data_Rules rule) {
        // For universal rules (like "ALL MUST REMAIN"), the rule automatically applies
        if (rule.is_universal)
            return true;
        
        bool tattooValid = true;
        bool scarValid = true;
        bool auraValid = true;
        bool limbValid = true;
        
        // Check tattoo rules
        if (rule.rule_data.tattoo.enabled) {
            // Check if the tattoo type is present on the corpse
            bool hasTattoo = features.appliedTattoos.Any(t => t.ToLower().Contains(rule.rule_data.tattoo.type.ToLower()));
            
            // If this is an exception case, invert the result
            if (rule.rule_data.tattoo.exception) {
                tattooValid = !hasTattoo;
            } else {
                tattooValid = hasTattoo;
            }
        }

        // Check scar rules
        if (rule.rule_data.scar.enabled) {
            // Check if the scar type is present on the corpse
            bool hasScar = features.appliedScars.Any(s => s.ToLower().Contains(rule.rule_data.scar.type.ToLower()));
            
            // If this is an exception case, invert the result
            if (rule.rule_data.scar.exception) {
                scarValid = !hasScar;
            } else {
                scarValid = hasScar;
            }
        }

        // Check aura rules
        if (rule.rule_data.aura.enabled) {
            // Check if the aura color is present on the corpse
            bool hasAura = features.appliedAura.Any(a => a.ToLower().Contains(rule.rule_data.aura.color.ToLower()));
            
            // If this is an exception case, invert the result
            if (rule.rule_data.aura.exception) {
                auraValid = !hasAura;
            } else {
                auraValid = hasAura;
            }
        }
        
        // Check limb rules
        if (rule.rule_data.limb.enabled) {
            // Check if the specified limb is missing on the corpse
            bool missingLimb = features.missingLimbs.Any(l => l.ToLower().Contains(rule.rule_data.limb.missing.ToLower()));
            
            // If this is an exception case, invert the result
            if (rule.rule_data.limb.exception) {
                limbValid = !missingLimb;
            } else {
                limbValid = missingLimb;
            }
        }

        // All conditions must be met for the rule to be valid
        bool valid = tattooValid && scarValid && auraValid && limbValid;
        Debug.Log($"Rule validation: {rule.rule_content} = {valid}");
        
        return valid;
    }

    private List<RaycastHit> CastRaysInward(int rayCount = 100, float rayHitChance = 0.05f) {
        List<RaycastHit> allHits = new();
        
        var center = sphereCollider.transform.position;
        var radius = sphereCollider.radius;
        
        Vector3[] points = GeneratePointsOnSphere(rayCount, radius, center);
        
        foreach (Vector3 point in points) {
            if (Random.value > rayHitChance)
                continue;
                
            Vector3 direction = (center - point).normalized;
            float distance = Vector3.Distance(point, center);
            
            Ray ray = new(point, direction);

            bool didHit = Physics.Raycast(ray, out RaycastHit hit, distance, raycastLayerMask);
            
            if (!didHit && corpseMeshRend != null){
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
        
        if (corpseMeshRend == null || corpseMeshRend.sharedMesh == null)
            return false;
            
        Mesh mesh = corpseMeshRend.sharedMesh;
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
    public void SpawnWhisper() {
        if (whisperPrefab == null || spawnedWhisper)
            return;
        
        spawnedWhisper = true;

        GameObject whisper = Instantiate(whisperPrefab);

        whisper.transform.SetLocalPositionAndRotation(
            rope.endPoint.position, 
            rope.endPoint.rotation
        );
        
        var whisperStr = Manager_Content.Instance.PickWhisper();
        var whisperText = whisper.GetComponentInChildren<TMP_Text>();
        whisperText.text = whisperStr;

        DOTween.Sequence()
            .Append(
                whisper.transform
                    .DOMoveY(
                        whipserMoveUpAmount, 
                        whipserTweenTime
                    )
                    .SetEase(Ease.Linear)
            )
            .Join(
                whisperText
                    .DOFade(0, whipserTweenTime)
            )
            .OnComplete(() => {
                spawnedWhisper = false;
                Destroy(whisper);
            });
    }
}