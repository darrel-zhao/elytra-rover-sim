using System.Collections;
using UnityEngine;
using Sim.Rover;
using Unity.VisualScripting;
using System;

[RequireComponent(typeof(Rigidbody))]
public class RoverDriver : MonoBehaviour
{
    float moveSpeed = 0.5f; // Speed of the rover
    float turnSpeed = 50f; // Speed of rotation
    // float turnTolerance = 20f;
    bool _isTurning = false;
    bool _isAdjusting = false;
    int completedNodes = 0;
    int previousNode ;
    Rigidbody rb;
    GridMapGenerator map;
    public Rover rover;
    Vector3 currentTarget;
    TrashFinder trashFinder;
    Vector3 previousPos = Vector3.zero;
    public bool active { get; private set; }

    public void Init(GridMapGenerator mapRef, Rover roverData)
    {
        // Get map and rover data
        map = mapRef;
        rover = roverData;
        trashFinder = GetComponentInChildren<TrashFinder>();

        // Initialize Rigidbody
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f); // Adjust center of mass for stability

        // Prep the first destination
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

            toTarget = currentTarget - rb.position;

            // check if turn is required
            float angle = Vector3.Angle(transform.forward, toTarget);
            if (isLeft(transform.forward, toTarget))
                StartCoroutine(TurnLeftNinety());

            else if (isRight(transform.forward, toTarget))
                StartCoroutine(TurnRightNinety());

            else
                StartCoroutine(CrossIntersection());
        }

        // Move towards next target
        Vector3 nextTrashPath = trashFinder.driveToPos;
        Vector3 dispToPos = Vector3.zero;

        Vector3 roverPos;
        // calculate orthogonal normalized vector to rover's forward direction
        Vector3 ortho = Vector3.Cross(transform.forward, Vector3.up).normalized;
        Vector3 offset = ortho * 0.3f;
        roverPos = rb.position - offset;

        if (nextTrashPath != previousPos)
        {
            dispToPos = nextTrashPath - roverPos;
        }
        Debug.DrawRay(roverPos, dispToPos, Color.green);

        // If there's a trash target and it's not too close
        if (nextTrashPath != Vector3.zero && dispToPos.magnitude > 1f && Vector3.Angle(transform.forward, dispToPos) > 0f)
        {
            previousPos = nextTrashPath;
            if (!_isAdjusting)
                StartCoroutine(AdjustCourse(dispToPos)); // Start adjusting course towards the trash
        }

        // Move towards the target
        Vector3 forward = transform.forward * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + forward);

        // Create a ternary that assigns the smaller component of toTarget to distError
        float distError = toTarget.x <= toTarget.z ? toTarget.x : toTarget.z;
        // print($"Distance Error: {distError}");
        Debug.DrawLine(rb.position, currentTarget, Color.red);

        if (Math.Abs(distError) > 0.1f && !_isAdjusting && !_isTurning)
        {
            StartCoroutine(AdjustCourse(currentTarget - rb.position));
        }
    }

    IEnumerator AdjustCourse(Vector3 targetDir)
    {
        _isAdjusting = true;

        // print("Adjusting Course...");
        while (Vector3.Angle(transform.forward, targetDir) > 3f)
        {
            if (Vector3.Angle(transform.forward, targetDir) < 4f)
            {
                _isAdjusting = false;
                yield break; // Exit if the angle is small enough
            }
            if (isRight(transform.forward, targetDir, 2f))
            {
                rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turnSpeed * 0.5f * Time.fixedDeltaTime, 0f));
            }
            else if (isLeft(transform.forward, targetDir, 2f))
            {
                rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, -turnSpeed * 0.5f * Time.fixedDeltaTime, 0f));
            }

            
            yield return new WaitForFixedUpdate();
        }

        // print("Course Adjusted.");

        _isAdjusting = false;
    }
    bool isRight(Vector3 from, Vector3 to, float tolerance = 20f)
    {
        float signedAngle = Vector3.SignedAngle(from, to, Vector3.up);
        // print(signedAngle);
        return signedAngle > tolerance;
    }

    bool isLeft(Vector3 from, Vector3 to, float tolerance = 20f)
    {
        float signedAngle = Vector3.SignedAngle(from, to, Vector3.up);
        return signedAngle < -tolerance;
    }

    private bool AdvanceWaypoint()
    {
        if (rover.path == null || rover.path.Count == 0) return false;

        Vector3 nextDirection = Vector3.zero;
        if (completedNodes == 0)
        {
            previousNode = rover.path.Dequeue(); // Remove the first node, as that is the start node
        }

        int toNode = rover.path.Dequeue();
        currentTarget = map.NodeToWorld(toNode);
        nextDirection = (currentTarget - map.NodeToWorld(previousNode)).normalized;
        previousNode = toNode;

        // Shift currentTarget to the right side of the road
        Vector3 ortho = Vector3.Cross(nextDirection, Vector3.up).normalized;
        Vector3 offset = ortho * 1.5f; // Adjust the offset as needed
        currentTarget -= offset;

        completedNodes++;
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

    IEnumerator TurnLeftNinety()
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

    IEnumerator TurnRightNinety()
    {
        _isTurning = true;

        float rotatedSoFar = 0f;
        float speed = turnSpeed * 1.5f * Time.fixedDeltaTime;

        while (rotatedSoFar < 90f)
        {
            // rotate by a small amount & update rotatedSoFar
            float delta = Mathf.Min(speed, 90f - rotatedSoFar);
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