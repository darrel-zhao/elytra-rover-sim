using System;
using System.Collections;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
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

    private Rigidbody rb;
    private bool _isTurning = false;

    // for debugging purposes
    [Header("Manual Drive Mode")]
    public bool keyboardOverride = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void FixedUpdate()
    {
        // WASD controls for debugging
        if (keyboardOverride)
            handleKeyboardInput();
        else
        {
            // First do a simple test drive: drive in an o shape
            Vector3 forward = transform.forward * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forward);

            // Use a coroutine to turn right
            if (IsAtIntersection() && !_isTurning)
            {
                StartCoroutine(TurnRight());
            }
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
        float speed = turnSpeed * 3f * Time.fixedDeltaTime;

        while (rotatedSoFar < turnAngle)
        {
            // rotate by a small amount & update rotatedSoFar
            float delta = Mathf.Min(speed, turnAngle - rotatedSoFar);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, delta, 0f));
            rotatedSoFar += delta;

            print(rb.rotation.eulerAngles.y);

            // move forward a bit
            Vector3 forward = transform.forward * moveSpeed * Time.fixedDeltaTime * 0.5f; // move forward a bit less while turning
            rb.MovePosition(rb.position + forward);

            // tick frame
            yield return new WaitForFixedUpdate();
        }

        // wait until off intersection before setting _isTurning to false
        yield return new WaitUntil(() => !IsAtIntersection());

        _isTurning = false;
    }

    void handleKeyboardInput()
    {
        if (Keyboard.current.wKey.isPressed)
            rb.MovePosition(rb.position + transform.forward * moveSpeed * Time.fixedDeltaTime);
        if (Keyboard.current.sKey.isPressed)
            rb.MovePosition(rb.position - transform.forward * moveSpeed * Time.fixedDeltaTime);
        if (Keyboard.current.aKey.isPressed)
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, -turnSpeed * Time.fixedDeltaTime, 0f));
        if (Keyboard.current.dKey.isPressed)
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turnSpeed * Time.fixedDeltaTime, 0f));
    }
}
