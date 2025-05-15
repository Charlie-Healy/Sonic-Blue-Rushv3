using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ThirdPersonMovement : MonoBehaviour
{
    RingCollecter ringCollecter;
    public int value;

    public Animator animator;

    public float attackSpeed;

    public AudioClip jumpClip;
    public AudioClip ringClip;
    public AudioClip runClip;
    public AudioClip windClip;
    public AudioSource source;
    public AudioSource stepSound;
    public AudioSource windSound;
    //public AudioSource ringSound;

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

    public bool isAttacking;
    public bool hasRings;

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
        stepSound.loop = false;
        stepSound.playOnAwake = false;
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
            source.PlayOneShot(ringClip);
            hasRings = true;


        }

        if (other.gameObject.CompareTag("Death"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);


        }

        if (other.gameObject.CompareTag("Slope"))
        {
            gravity = -500000;
            animator.SetBool("SonicRun", true);
            print("Colliding");
        }
        else if (other.gameObject.CompareTag("Reset"))
        {
            gravity = -40;
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            if (isAttacking)
            {
                Destroy(other.gameObject);
            }
            else if (!isAttacking && hasRings == true)
            {
                hasRings = false;
                RingCollecter.instance.DecreaseRings(value);
            }
            else if (!isAttacking && hasRings == false)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            if(isAttacking && !isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
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
            //source.PlayOneShot(runClip);

        }
        else
        {
            animator.SetBool("SonicRun", false);
            //Audio.runClip = false;
        }

        if (!isGrounded)
        {
            animator.SetBool("SonicFall", true);
            //windSound.PlayOneShot(windClip);
        }
        else
        {
            animator.SetBool("SonicFall", false);
            
        }

        if(isGrounded && direction.magnitude < 0)
        {
            stepSound.Play();
        }
        else
        {
            
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
        {

            if ((isGrounded) || doubleJump)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

                doubleJump = !doubleJump;

            }



        }
        
        

        

        if(Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump") && !isGrounded)
        {
            animator.SetTrigger("DoubleJump");
        }
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump") && !isGrounded)
        {
            animator.SetBool("SonicFall", true);
        }
        
        

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump") && isGrounded)
        {
            animator.SetTrigger("SonicJump");
            source.PlayOneShot(jumpClip);
        }


        
        /*if(Input.GetButtonDown("Fire2") && !isGrounded)
        {
            direction.magnitude += 0.1f;
        }*/
        

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Fire3") && Stamina > 0 && direction.magnitude >= 0.1f)
        {
            speed = 250f;
            //gravity = -50f;
            Stamina -= RunCost * Time.deltaTime;
            if (Stamina < 0) Stamina = 0;
            if (Stamina > MaxStamina) Stamina = MaxStamina;
            StaminaBar.fillAmount = Stamina / MaxStamina;
            //if (recharge != null) StopCoroutine(recharge);
            //recharge = StartCoroutine(RechargeStamina());
            animator.SetBool("SonicBoost", true);
                       
            //windSound.loop = true;
            //windSound.Play();
            

        }
        else
        {
            speed = 175f;
            //gravity = -40f;
            animator.SetBool("SonicBoost", false);
            //stepSound.loop = false;
            //windSound.loop = false;
           // windSound.Stop();
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetButtonDown("Fire3") && Stamina > 0 )
        {
            windSound.Play();
            stepSound.Play();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetButtonUp("Fire3") && Stamina > 0 || Stamina <= 0 )
        {
            windSound.Stop();
            stepSound.Stop();
        }
            if (Input.GetKeyDown(KeyCode.Tab))
        {
            Stamina = 100;
            StaminaBar.fillAmount = Stamina;
        }

        /*if (Input.GetKey(KeyCode.C))
        {
            controller.height = 2.5f;
            targetTime = 3f;
        }
        else if ((targetTime <= 0.0f) || Input.GetKeyUp(KeyCode.C))
        {
            controller.height = 5.44f;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetButton("Fire2") && !isGrounded)
        {
            stomp = true;

        }
        else if (isGrounded)
        {
            stomp = false;
            //gravity = -40f;

        }
        if (stomp == true)
        {
            speed = 0f;
            //gravity = -500f;
            animator.SetBool("SonicStomp", true);
        }
        else
        {
            animator.SetBool("SonicStomp", false);
        }*/

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetButton("Fire2") && isGrounded)
        {
            animator.SetBool("SonicRoll", true);
            isAttacking = true;
        }
        else
        {
            animator.SetBool("SonicRoll", false);
            isAttacking = false;
        }

        if(!isGrounded || Input.GetKey(KeyCode.LeftControl) || Input.GetButton("Fire2"))
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }

        

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetButtonDown("Fire1") && isGrounded)
        {
            speed = 0f;

            if (Input.GetKeyUp(KeyCode.Z) || Input.GetButtonUp("Fire1") && isGrounded)
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

    

    

    

    public void PlayStepSound()
    {
        stepSound.Play();
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

    
