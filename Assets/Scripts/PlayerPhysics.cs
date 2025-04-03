using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    public Rigidbody rb;

    public LayerMask layermask;

    bool ground;

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
        Ground();

        Snap();

        Move();        

        if (!ground)        
            Gravity();        
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

    [SerializeField] float groundDistance;

    Vector3 normal;

    void Ground()
    {
        ground = Physics.Raycast(rb.worldCenterOfMass, -rb.transform.up, out RaycastHit hit, groundDistance, layermask, QueryTriggerInteraction.Ignore);
        normal = ground ? hit.normal : Vector3.up;
    }

    void Snap()
    {
        transform.up = normal;
    }
}
