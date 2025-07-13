using System;
using System.Collections;
using JetBrains.Annotations;
using Unity.VisualScripting;
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
    private bool _isTurning = false;

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
        // if (IsAtIntersection())
        // {
        //     TurnRight();
        // }

        // Use a coroutine to turn right
        if (IsAtIntersection() && !_isTurning)
        {
            StartCoroutine(TurnRight());
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

    IEnumerator TurnRight(float turnAngle = 90f)
    {
        _isTurning = true;

        float rotatedSoFar = 0f;
        float speed = turnSpeed * 3.7f * Time.deltaTime;

        while (rotatedSoFar < turnAngle)
        {
            // rotate by a small amount & update rotatedSoFar
            float delta = Mathf.Min(speed, turnAngle - rotatedSoFar);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, delta, 0f));
            rotatedSoFar += delta;

            print(rb.rotation.eulerAngles.y);

            // move forward a bit
            Vector3 forward = transform.forward * moveSpeed * Time.deltaTime * 0.5f; // move forward a bit less while turning
            rb.MovePosition(rb.position + forward);

            // tick frame
            yield return null;
        }

        // wait until off intersection before setting _isTurning to false
        yield return new WaitUntil(() => !IsAtIntersection());

        _isTurning = false;
    }

    private void TurnRandomly()
    {
        float randomTurn = UnityEngine.Random.Range(-1f, 1f); // Random value between -1 and 1
        Quaternion turn = Quaternion.Euler(0f, randomTurn * turnSpeed * Time.deltaTime, 0f);
        rb.MoveRotation(rb.rotation * turn);
    }
}
