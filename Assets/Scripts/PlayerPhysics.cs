using System;
using System.Collections;
using UnityEngine;


public class PlayerPhysics : MonoBehaviour
{
    public Rigidbody rb;

    public LayerMask layermask;

    // Movement velocity properties
    public Vector3 horizontalVelocity => Vector3.ProjectOnPlane(rb.linearVelocity, rb.transform.up);
    public Vector3 verticalVelocity => Vector3.Project(rb.linearVelocity, rb.transform.up);
    public float verticalSpeed => Vector3.Dot(rb.linearVelocity, rb.transform.up);
    public float speed => horizontalVelocity.magnitude;

    public Action onPlayerPhysicsUpdate;

    void FixedUpdate()
    {
        onPlayerPhysicsUpdate?.Invoke();

        if (!groundInfo.ground)
            Gravity();

        if (groundInfo.ground && verticalSpeed < rb.sleepThreshold)
            rb.linearVelocity = horizontalVelocity;

        StartCoroutine(LateFixedUpdateRoutine());

        IEnumerator LateFixedUpdateRoutine()
        {
            yield return new WaitForFixedUpdate();
            LateFixedUpdate();
        }
    }

    [SerializeField] float gravity;

    // Apply gravity to the player
    void Gravity()
    {
        rb.linearVelocity -= Vector3.up * gravity * Time.deltaTime;
    }

    void LateFixedUpdate()
    {
        Ground();
        Snap();

        if (groundInfo.ground)
            rb.linearVelocity = horizontalVelocity;
    }

    [SerializeField] float groundDistance;

    public struct GroundInfo
    {
        public Vector3 point;
        public Vector3 normal;
        public bool ground;
    }

    [HideInInspector] public GroundInfo groundInfo;

    public Action onGroundEnter;
    public Action onGroundExit;

    // Check if the player is on the ground
    void Ground()
    {
        float maxDistance = Mathf.Max(rb.centerOfMass.y, 0) + (rb.sleepThreshold * Time.fixedDeltaTime);

        bool ground = Physics.Raycast(rb.worldCenterOfMass, -rb.transform.up, out RaycastHit hit, groundDistance, layermask, QueryTriggerInteraction.Ignore);

        if (ground && verticalSpeed < rb.sleepThreshold)
            maxDistance += groundDistance;

        Vector3 point = ground ? hit.point : rb.transform.position;
        Vector3 normal = ground ? hit.normal : Vector3.up;

        if (ground != groundInfo.ground)
        {
            if (ground)
                onGroundEnter?.Invoke();
            else
                onGroundExit?.Invoke();
        }

        groundInfo = new()
        {
            point = point,
            normal = normal,
            ground = ground,
        };
    }

    // Adjust the player's position based on ground information
    void Snap()
    {
        rb.transform.up = groundInfo.normal;

        Vector3 goal = groundInfo.point;
        Vector3 newPosition = new Vector3(goal.x, goal.y, rb.transform.position.z);
        Vector3 difference = newPosition - rb.transform.position;

        if (rb.SweepTest(difference, out _, difference.magnitude, QueryTriggerInteraction.Ignore)) return;

        rb.transform.position = newPosition;    // Only modify X and Y
    }

    // New method to handle Y-axis rotation based on input
    public void RotatePlayer(Vector3 moveDirection)
    {
        if (moveDirection.magnitude > 0f)
        {
            // Create a rotation based on the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            // Smoothly rotate the player to face the movement direction
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 10f)); // Adjust the speed of rotation here
        }
    }
}
