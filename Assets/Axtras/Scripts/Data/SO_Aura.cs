using UnityEngine;

[CreateAssetMenu(fileName = "Aura", menuName = "Feature/Aura", order = 0)]
public class SO_Aura : ScriptableObject 
{
    public string aura_name = "";
    public Color aura_color;
    public GameObject aura_particles;    
}