using System.Collections;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    public Rigidbody rb;

    public LayerMask layermask;

    

    public Vector3 horizontalVelocity => Vector3.ProjectOnPlane(rb.linearVelocity, rb.transform.up);

    public Vector3 verticalVelocity => Vector3.Project(rb.linearVelocity, rb.transform.up);

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            Jump();
        
    }

    [SerializeField] float jumpForce;

    void Jump()
    {
        if (!ground) return;

        rb.linearVelocity = (Vector3.up * jumpForce)
            + horizontalVelocity;
    }

    void FixedUpdate()
    {
        

        Move();        

        if (!ground)        
            Gravity();

        StartCoroutine(LateFixedUpdateRoutine());

        IEnumerator LateFixedUpdateRoutine()
        {
            yield return new WaitForFixedUpdate();

            LateFixedUpdate();
        }
    }

    [SerializeField] float speed;

    void Move()
    {
        rb.linearVelocity = (Vector3.right * Input.GetAxis("Horizontal") * speed) + (Vector3.forward * Input.GetAxis("Vertical") * speed)
            + verticalVelocity;
    }

    [SerializeField] float gravity;

    void Gravity()
    {
        rb.linearVelocity -= Vector3.up * gravity * Time.deltaTime;
    }

    void LateFixedUpdate()
    {
        Ground();
        Snap();
    }

    [SerializeField] float groundDistance;

    Vector3 point;

    Vector3 normal;

    bool ground;

    void Ground()
    {
        ground = Physics.Raycast(rb.worldCenterOfMass, -rb.transform.up, out RaycastHit hit, groundDistance, layermask, QueryTriggerInteraction.Ignore);
        
        point = ground ? hit.point : rb.transform.position;

        normal = ground ? hit.normal : Vector3.up;
    }

    void Snap()
    {
        rb.transform.up = normal;

        Vector3 goal = point;

        Vector3 difference = goal - rb.transform.position;

        if (rb.SweepTest(difference, out _, difference.magnitude, QueryTriggerInteraction.Ignore)) return;

        rb.transform.position = goal;
    }
}
