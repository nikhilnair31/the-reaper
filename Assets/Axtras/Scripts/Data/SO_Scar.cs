using UnityEngine;

[CreateAssetMenu(fileName = "Scar", menuName = "Feature/Scar", order = 0)]
public class SO_Scar : ScriptableObject
{
    [Header("Main")]
    public string scar_name = "";
    public Texture2D scar_tex;
    public GameObject scar_prefab;

    [Header("Spawn Attributes")]
    public float scarSize = 0.15f;

    public void SpawnScar(RaycastHit hit) {
        // Instantiate tattoo decal
        var decalInst = Instantiate(scar_prefab, hit.point, Quaternion.identity);

        // Position the tattoo decal
        decalInst.transform.forward = hit.normal;
        decalInst.transform.position += hit.normal * 0.01f;
        decalInst.transform.Rotate(hit.normal, Random.Range(0, 360), Space.World);

        // Scale tattoo decal
        var scale = Random.Range(scarSize * 0.7f, scarSize * 1.3f);
        decalInst.transform.localScale = Vector3.one * scale;

        // Set parent as corpse
        if (hit.collider != null)
            decalInst.transform.SetParent(hit.collider.transform);
    }
}