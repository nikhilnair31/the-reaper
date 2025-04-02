using UnityEngine;

[CreateAssetMenu(fileName = "Scar", menuName = "Feature/Scar", order = 0)]
public class SO_Scar : ScriptableObject 
{
    public string scar_name = "";
    public Texture2D scar_tex;
    public GameObject scar_decal;    
}