using UnityEngine;

[CreateAssetMenu(fileName = "Aura", menuName = "Feature/Aura", order = 0)]
public class SO_Aura : ScriptableObject
{
    [Header("Main")]
    public string aura_name = "";
    public Color aura_color;
    public GameObject aura_prefab;

    public void SpawnAura(Mesh mesh, Transform parent) {
        // Instantiate aura particles
        var auraInst = Instantiate(aura_prefab);

        // Parent
        auraInst.transform.SetParent(parent);

        // Position the aura particles
        auraInst.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        // Change color
        if (auraInst.TryGetComponent(out ParticleSystem ps)) {
            var main = ps.main;
            main.startColor = aura_color;
            var shape = ps.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Mesh;
            shape.meshShapeType = ParticleSystemMeshShapeType.Triangle;
            shape.mesh = mesh;
        }
    }
}