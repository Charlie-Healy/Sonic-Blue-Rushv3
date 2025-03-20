using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ThirdPersonMovement : MonoBehaviour
{
    public Animator animator;

    public Image StaminaBar;
    public float Stamina, MaxStamina;
    public float RunCost;
    private Coroutine recharge;
    public float ChargeRate;

    public CharacterController controller;
    public Transform cam;

    public float SpinDashTime = 0f;
    public int SpinDashCounter = 5;

    public float speed = 6f;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public float jumpHeight = 2f;
    private int numberOfJumps;
    [SerializeField] private int maxNumberOfJumps = 2;
    private bool doubleJump;

    Vector3 velocity;
    Vector3 targetPosition;
    Vector3 refVelocity = Vector3.zero;
    float smoothing = 0.5f;
    public float gravity = -9.81f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public bool isGrounded;

    public float targetTime = 2f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        speed = 0f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectable"))
        {
            StaminaBar.fillAmount += 0.05f;
            Stamina += 5f;
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        targetTime -= Time.deltaTime;

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

        

        if (direction.magnitude >= 0.1f)
        {
            
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);


        }

        if(Input.GetKey(KeyCode.W)&& isGrounded)
        {
            animator.SetBool("SonicIdle", false);
            animator.SetBool("SonicRun", true);
        }
        else
        {
            animator.SetBool("SonicIdle", true);
            animator.SetBool("SonicRun", false);
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            if ((isGrounded) || doubleJump)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

                doubleJump = !doubleJump;
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) && Stamina > 0)
        {
            speed = 125f;
            gravity = -40f;
            Stamina -= RunCost * Time.deltaTime;
            if (Stamina < 0) Stamina = 0;
            if (Stamina > MaxStamina) Stamina = MaxStamina;
            StaminaBar.fillAmount = Stamina / MaxStamina;
            //if (recharge != null) StopCoroutine(recharge);
            //recharge = StartCoroutine(RechargeStamina());
        }
        else
        {
            speed = 80f;
            gravity = -20f;
        }

        if (Input.GetKey(KeyCode.C))
        {
            controller.height = 2.5f;
            targetTime = 3f;
        }
        else if ((targetTime <= 0.0f) || Input.GetKeyUp(KeyCode.C))
        {
            controller.height = 5.44f;
        }

        if (Input.GetKey(KeyCode.LeftControl) && !isGrounded)
        {
            speed = 0f;
            gravity = -500f;
        }

        if (Input.GetKeyDown(KeyCode.Z) && isGrounded)
        {
            speed = 0f;

            if (Input.GetKeyUp(KeyCode.Z) && SpinDashCounter > 0 && isGrounded)
            {
                SpinDashTime = 5f;
                SpinDashCounter = SpinDashCounter - 1;
            }
            if (SpinDashTime > 0f)
            {
                speed = 135;
            }
            else if (SpinDashTime < 0)
            {
                speed = 0f;
            }
        }
    }

    /*private IEnumerator ChargeRun()
    {
        yield return new WaitForSeconds(2f);
        while (Input.GetKeyDown(KeyCode.W))
        {
            speed += Time.deltaTime;
            if (speed < 110f)
            {
                speed = 110f;
            }
            
        }


    }*/

    private IEnumerator RechargeStamina()
    {
        yield return new WaitForSeconds(1f);

        while (Stamina < MaxStamina)
        {
            Stamina += ChargeRate / 33f;
            if (Stamina > MaxStamina) Stamina = MaxStamina;
            StaminaBar.fillAmount = Stamina / MaxStamina;
            yield return new WaitForSeconds(1f);

            //if(recharge != null) StopCoroutine(recharge);
            //recharge = StartCoroutine(RechargeStamina());
        }
    }
}
