using UnityEngine;

[CreateAssetMenu(fileName = "Tattoo", menuName = "Feature/Tattoo", order = 0)]
public class SO_Tattoo : ScriptableObject 
{
    public string tattoo_name = "";
    public Texture2D tattoo_tex;
    public GameObject tattoo_decal;    
}