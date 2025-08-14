using System.Collections.Generic;
using Sim.Rover;
using UnityEngine;

// Attached to DetectionCollider of the rover prefab
public class TrashFinder : MonoBehaviour
{
    public Vector3 driveToPos { get; private set; } 
    List<TrashIdentifier> detectedTrash = new List<TrashIdentifier>();
    public int detectedCount{ get; private set; }
    public RoverDriver roverDriver;
    BoxCollider boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        roverDriver = GetComponentInParent<RoverDriver>(); 
        if (!roverDriver)
        {
            Debug.LogError("RoverDriver component not found in parent.");
        }
    }

    void Update()
    {
        if (roverDriver.IsAtIntersection()) { boxCollider.enabled = false; }
        else { boxCollider.enabled = true; }
    }

    public void OnTriggerEnter(Collider other)
    {
        TrashIdentifier trash = other.GetComponent<TrashIdentifier>();
        if (trash)
        {
            detectedTrash.Add(trash);
        }

        UpdateDriveVector();
    }
    
    void UpdateDriveVector()
    {
        if (detectedTrash.Count > 0)
        {
            // Calculate the average position of detected trash
            Vector3 averagePosition = Vector3.zero;
            foreach (var t in detectedTrash)
            {
                if (t == null || t.transform == null) continue; // Skip if the trash object is destroyed
                averagePosition += t.transform.position;
                detectedCount++;
            }
            averagePosition /= detectedTrash.Count;

            // Set the drive vector towards the average position
            driveToPos = averagePosition;
            detectedTrash.Clear(); // Clear the list after updating the drive vector
        }
    }
}