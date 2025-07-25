using UnityEngine;

public class TrashCollector : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is tagged as "Trash"
        TrashIdentifier trash = other.GetComponent<TrashIdentifier>();
        if (trash)
        {
            // Destroy the trash object
            Destroy(other.gameObject);

            // Optionally, you can add logic to update the score or notify the player
            Debug.Log("Trash collected: " + other.name);
        }
    }
}