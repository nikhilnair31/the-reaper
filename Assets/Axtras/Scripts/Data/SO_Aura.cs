using UnityEngine;

[CreateAssetMenu(fileName = "Aura", menuName = "Feature/Aura", order = 0)]
public class SO_Aura : ScriptableObject
{
    [Header("Main")]
    public string aura_name = "";
    public Color aura_color;
    public GameObject aura_prefab;

    public void SpawnAura(Transform parent) {
        // Instantiate tattoo decal
        var auraInst = Instantiate(aura_prefab);

        // Position the tattoo decal
        auraInst.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        // Parent
        auraInst.transform.SetParent(parent);

        // Change color
        if (auraInst.TryGetComponent(out ParticleSystem ps)) {
            var main = ps.main;
            main.startColor = aura_color;
        }
    }
}