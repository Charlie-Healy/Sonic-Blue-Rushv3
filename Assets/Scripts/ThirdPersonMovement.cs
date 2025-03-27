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
    public bool stomp = false;

    public float targetTime = 2f;

    private void Awake()
    {
        //animator = GetComponent<Animator>();
    }

    

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        speed = 0f;
        animator = GetComponent<Animator>();
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
            animator.SetBool("SonicRun", true);

        }
        else
        {
            animator.SetBool("SonicRun", false);
        }

        if(!isGrounded)
        {
            animator.SetBool("SonicFall" , true);
        }
        else
        {
            animator.SetBool("SonicFall", false);
        }

        if (Input.GetKeyDown(KeyCode.Space)|| Input.GetButtonDown("Jump"))
        {
            
            if ((isGrounded) || doubleJump)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

                doubleJump = !doubleJump;
            }
            
        }

        if(Input.GetKeyDown(KeyCode.Space)|| Input.GetButtonDown("Jump")&& isGrounded )
        {
            animator.SetTrigger("SonicJump");
        } 

        if (Input.GetKey(KeyCode.LeftShift)|| Input.GetButton("Fire3") && Stamina > 0 && direction.magnitude >= 0.1f)
        {
            speed = 125f;
            gravity = -50f;
            Stamina -= RunCost * Time.deltaTime;
            if (Stamina < 0) Stamina = 0;
            if (Stamina > MaxStamina) Stamina = MaxStamina;
            StaminaBar.fillAmount = Stamina / MaxStamina;
            //if (recharge != null) StopCoroutine(recharge);
            //recharge = StartCoroutine(RechargeStamina());
            animator.SetBool("SonicBoost", true);
        }
        else
        {
            speed = 80f;
            gravity = -40f;
            animator.SetBool("SonicBoost", false);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Stamina = 100;
            StaminaBar.fillAmount = Stamina;
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

        if (Input.GetKeyDown(KeyCode.LeftControl)|| Input.GetButton("Fire2") && !isGrounded)
        {
            stomp = true;
            
        }
        else if (isGrounded)
        {
            stomp = false;
            gravity = -40f;
            
        }
        if(stomp == true)
        {
            speed = 0f;
            gravity = -500f;
            animator.SetBool("SonicStomp", true);
        }
        else
        {
            animator.SetBool("SonicStomp", false);
        }

        if (Input.GetKeyDown(KeyCode.Z)|| Input.GetButtonDown("Fire1") && isGrounded)
        {
            speed = 0f;

            if (Input.GetKeyUp(KeyCode.Z)|| Input.GetButtonUp("Fire1") && isGrounded)
            {
                speed = 80f;
                animator.SetBool("SpinDash", true);
            }
            else
            {
                animator.SetBool("SpinDash", false);
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
