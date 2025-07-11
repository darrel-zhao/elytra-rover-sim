using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

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
    float originalYRotation;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        originalYRotation = transform.rotation.eulerAngles.y;
    }

    void Update()
    {
        // First do a simple test drive: drive in an o shape
        Vector3 forward = transform.forward * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + forward);

        // Calculate the current angle of rotation
        float currentYRotation = transform.rotation.eulerAngles.y;
        float angleDelta = Mathf.DeltaAngle(originalYRotation, currentYRotation);

        // Check for intersection and turn randomly
        if (IsAtIntersection() && Mathf.Abs(angleDelta) < 90f)
        {
            TurnRight();
        }

        print(currentYRotation);
    }

    private bool IsAtIntersection()
    {
        Collider[] hitcolliders = Physics.OverlapSphere(transform.position, 0.1f);
        foreach (var hit in hitcolliders)
        {
            if (hit.CompareTag("Intersection"))
            {
                return true; // At an intersection
            }
        }

        return false; // Not at an intersection
    }

    private void TurnRight()
    {
        Quaternion turn = Quaternion.Euler(0f, turnSpeed * 2f * Time.deltaTime, 0f);
        rb.MoveRotation(rb.rotation * turn);
    }

    private void TurnRandomly()
    {
        float randomTurn = Random.Range(-1f, 1f); // Random value between -1 and 1
        Quaternion turn = Quaternion.Euler(0f, randomTurn * turnSpeed * Time.deltaTime, 0f);
        rb.MoveRotation(rb.rotation * turn);
    }
}
