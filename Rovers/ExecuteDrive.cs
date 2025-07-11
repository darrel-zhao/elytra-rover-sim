using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

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

        // Check for intersection and turn randomly
        if (IsAtIntersection())
        {
            TurnRight();
        }
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

    private void TurnRight(float turnAngle = 90f)
    {
        float currentYRotation = transform.rotation.eulerAngles.y;
        Quaternion turn = Quaternion.Euler(0f, turnSpeed * 2f * Time.deltaTime, 0f);
        float currentAngle = (rb.rotation * turn).eulerAngles.y;

        if (currentAngle < turnAngle)
        {
            rb.MoveRotation(rb.rotation * turn);
        }
        else if (currentAngle > turnAngle)
        {
            // correct it back to 90 degrees
            turn = Quaternion.Euler(0f, currentAngle - (currentAngle - turnAngle), 0f);
            rb.MoveRotation(turn);
        }
    }

    private void TurnRandomly()
    {
        float randomTurn = Random.Range(-1f, 1f); // Random value between -1 and 1
        Quaternion turn = Quaternion.Euler(0f, randomTurn * turnSpeed * Time.deltaTime, 0f);
        rb.MoveRotation(rb.rotation * turn);
    }
}
