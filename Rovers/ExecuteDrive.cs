using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// This script handles the driving a single rover within the simulation. The rover will move forward 
/// at a constant speed and will turn randomly when it reaches an intersection. This script is attached
/// to the Rover prefab, which is instantiated by the RoverSpawner script.
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class ExecuteDrive : MonoBehaviour
{
    float moveSpeed = 1f; // Speed of the rover
    float turnSpeed = 50f; // Speed of rotation

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        // Move forward
        Vector3 forward = transform.forward * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + forward);
        IsAtIntersection();

        // Check for intersection and turn randomly
        // if (IsAtIntersection())
        // {
        //     TurnRandomly();
        // }
    }

    private bool IsAtIntersection()
    {
        return false;
    }

    private void TurnRandomly()
    {
        float randomTurn = Random.Range(-1f, 1f); // Random value between -1 and 1
        Quaternion turn = Quaternion.Euler(0f, randomTurn * turnSpeed * Time.deltaTime, 0f);
        rb.MoveRotation(rb.rotation * turn);
    }
}
