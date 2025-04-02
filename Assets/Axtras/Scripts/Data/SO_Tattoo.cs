using UnityEngine;

[CreateAssetMenu(fileName = "Tattoo", menuName = "Feature/Tattoo", order = 0)]
public class SO_Tattoo : ScriptableObject
{
    [Header("Main")]
    public string tattoo_name = "";
    public Texture2D tattoo_tex;
    public GameObject tattoo_prefab;

    [Header("Spawn Attributes")]
    public float tattooSize = 0.1f;

    public void SpawnTattoo(RaycastHit hit) {
        // Instantiate tattoo decal
        var decalInst = Instantiate(tattoo_prefab, hit.point, Quaternion.identity);

        // Position the tattoo decal
        decalInst.transform.forward = hit.normal;
        decalInst.transform.position += hit.normal * 0.01f;
        decalInst.transform.Rotate(hit.normal, Random.Range(0, 360), Space.World);

        // Scale tattoo decal
        var scale = Random.Range(tattooSize * 0.7f, tattooSize * 1.3f);
        decalInst.transform.localScale = Vector3.one * scale;

        // Set parent as corpse
        if (hit.collider != null)
            decalInst.transform.SetParent(hit.collider.transform);
    }
}