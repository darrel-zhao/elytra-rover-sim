using System.Collections;
using UnityEngine;
using Sim.Rover;

[RequireComponent(typeof(Rigidbody))]
public class RoverDriver : MonoBehaviour
{
    float moveSpeed = 0.5f; // Speed of the rover
    float turnSpeed = 50f; // Speed of rotation
    // float turnTolerance = 20f;
    bool _isTurning = false;
    Rigidbody rb;
    GridMapGenerator map;
    public Rover rover;
    Vector3 currentTarget;
    public bool active { get; private set; }

    public void Init(GridMapGenerator mapRef, Rover roverData)
    {
        // Get map and rover data
        map = mapRef;
        rover = roverData;

        // Initialize Rigidbody
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // Prep the first destination
        rover.path.Dequeue(); // Remove the first node, as that is the start node
        AdvanceWaypoint();
        active = true;
    }

    void FixedUpdate()
    {
        if (!active) return;

        Vector3 toTarget = currentTarget - rb.position;
        if (IsAtIntersection() && !_isTurning)
        {
            bool nextExists = AdvanceWaypoint();
            if (!nextExists)
            {
                print($"Route Completed for {gameObject.name}.");
                active = false;
                return;
            }

            toTarget = currentTarget - rb.position; // likely will have to change to account for side of road

            // check if turn is required
            float angle = Vector3.Angle(transform.forward, toTarget);
            if (isLeft(transform.forward, toTarget))
                StartCoroutine(TurnLeft());

            else if (isRight(transform.forward, toTarget))
                StartCoroutine(TurnRight());
                
            else
                StartCoroutine(CrossIntersection());
        }

        Vector3 forward = transform.forward * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + forward);
    }

    bool isRight(Vector3 from, Vector3 to)
    {
        float signedAngle = Vector3.SignedAngle(from, to, Vector3.up);
        return signedAngle > 20f;
    }

    bool isLeft(Vector3 from, Vector3 to)
    {
        float signedAngle = Vector3.SignedAngle(from, to, Vector3.up);
        return signedAngle < -20f;
    }

    private bool AdvanceWaypoint()
    {
        if (rover.path == null || rover.path.Count == 0) return false;

        int toNode = rover.path.Dequeue();
        currentTarget = map.NodeToWorld(toNode); // likely have to fix later to keep rover on right side of road
        return true;
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

    IEnumerator TurnLeft()
    {
        _isTurning = true;

        float crossDistance = 1.5f;
        float crossDuration = crossDistance / moveSpeed * 1.08f;
        float elapsed = 0f;

        // 1. Cross to opposite end of intersection in straight line
        while (elapsed < crossDuration)
        {
            Vector3 forward = transform.forward * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forward);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // 2. Rotate left (CCW) by 90 degrees
        float rotatedSoFar = 0f;
        float rotationSpeed = turnSpeed * 2.4f * Time.fixedDeltaTime;

        while (rotatedSoFar < 90f)
        {
            float delta = Mathf.Min(rotationSpeed, 90f - rotatedSoFar);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, -delta, 0f));
            rotatedSoFar += delta;

            Vector3 forward = transform.forward * moveSpeed * Time.fixedDeltaTime * 0.5f;
            rb.MovePosition(rb.position + forward);

            yield return new WaitForFixedUpdate();
        }

        // 3. Cross to opposite end until road is reached (not in intersection anymore)
        elapsed = 0f;
        while (elapsed < crossDuration)
        {
            Vector3 forward = transform.forward * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forward);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitUntil(() => !IsAtIntersection());
        _isTurning = false;
    }


    IEnumerator TurnRight(float turnAngle = 90f)
    {
        _isTurning = true;

        float rotatedSoFar = 0f;
        float speed = turnSpeed * 1.5f * Time.fixedDeltaTime;

        while (rotatedSoFar < turnAngle)
        {
            // rotate by a small amount & update rotatedSoFar
            float delta = Mathf.Min(speed, turnAngle - rotatedSoFar);
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, delta, 0f));
            rotatedSoFar += delta;

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

    IEnumerator CrossIntersection()
    {
        _isTurning = true;

        while (IsAtIntersection())
        {
            Vector3 forward = transform.forward * moveSpeed * 0.75f * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + forward);

            yield return new WaitForFixedUpdate();
        }

        // wait until off intersection before setting _isTurning to false
        yield return new WaitUntil(() => !IsAtIntersection());

        _isTurning = false;
    }
}