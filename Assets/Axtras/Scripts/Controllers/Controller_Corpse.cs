using UnityEngine;

public class Controller_Corpse : MonoBehaviour 
{
    #region Vars
    [Header("Rope Settings")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    
    [Header("Corpse Settings")]
    [SerializeField] private MeshRenderer corpseMeshRend;
    #endregion 
    
    private void Start() {
        RandMaterial();
    }
    private void RandMaterial() {
        Material[] mats = corpseMeshRend.materials;
        var mat = mats[0];
        mat.mainTexture = Resources.Load<Texture2D>("Textures/Corpse/Corpse_" + Random.Range(1, 4));
        corpseMeshRend.materials = mats;
    }
}