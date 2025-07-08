using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleRoverDrive : MonoBehaviour
{
    float moveSpeed = 1f;
    float turnSpeed = 50f;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    // Update is called once per frame
    void Update()
    {
        // Get input from WASD keys
        float r = Input.GetAxis("Horizontal"); // Left/Right causes rotation
        float v = Input.GetAxis("Vertical"); // Forward/Backward

        Vector3 forward = transform.forward * v * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + forward);

        Quaternion turn = Quaternion.Euler(0f, r * turnSpeed * Time.deltaTime, 0f);
        rb.MoveRotation(rb.rotation * turn);
    }
}
