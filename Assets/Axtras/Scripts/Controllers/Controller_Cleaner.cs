using UnityEngine;

public class Controller_Cleaner : MonoBehaviour 
{
    private void OnTriggerEnter(Collider other) {
        Debug.Log($"Cleaner collided with {other.name}");

        if (other.CompareTag("Corpse")) {
            Destroy(other.transform.parent.gameObject);
        }
    }    
}