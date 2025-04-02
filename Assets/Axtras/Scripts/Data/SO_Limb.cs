using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Limb", menuName = "Feature/Limb", order = 0)]
public class SO_Limb : ScriptableObject
{
    [Header("Main")]
    public string limb_name = "";

    public void SpawnLimb(Transform corpse, SO_Limb limbobj) {
        // Get limb names
        var limbsList = corpse.Find("Spine").GetComponentsInChildren<Transform>();

        // Rename limb
        var limb_name = limbobj.limb_name;
        var limb_name_wo_side = limb_name[..(limb_name.Length - 2)];

        // Find the limb in the corpse
        var limb = limbsList.FirstOrDefault(l => l.name.Replace(".L", "").Replace(".R", "").Contains(limb_name_wo_side));
        if (limb != null) {
            // Scale limb
            limb.localScale = Vector3.zero;
        }
    }
}