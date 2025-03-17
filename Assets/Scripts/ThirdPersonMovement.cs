using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{

    public CharacterController controller;
    public Transform cam;

    public float speed = 6f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public float jumpHeight = 2f;
    private int numberOfJumps;
    [SerializeField] private int maxNumberOfJumps = 2;
    private bool doubleJump;

    Vector3 velocity;
    public float gravity = -9.81f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public bool isGrounded;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {

        

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        isGrounded = controller.isGrounded;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //print("grounded=" + isGrounded);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            numberOfJumps = 2;
            //print("griddylicious");
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if ((isGrounded) || doubleJump)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

                doubleJump = !doubleJump;
            }
        }
    }
}
