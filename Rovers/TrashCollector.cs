using UnityEngine;

// Attached to the MouthCollider of the rover
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

            var roverDriver = GetComponentInParent<RoverDriver>();
            if (roverDriver.rover != null)
            {
                // Increment the trash collected count in the rover
                roverDriver.rover.trashCollected++;
            }
            else
            {
                Debug.LogError("Rover reference is missing in RoverDriver.");
            }

            var simManager = FindFirstObjectByType<SimManager>();
            if (simManager != null)
            {
                simManager.totalTrashCollected++;
            }

            Debug.Log("Trash collected: " + other.name);
        }
    }
}