using UnityEngine;

public class AnimationScript : MonoBehaviour
{

    public Animator animator;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W) && isGrounded)
        {
            animator.SetBool("SonicIdle", false);
            animator.SetBool("SonicRun", true);
            //SonicDust.gameObject.SetActive(true);
        }
        else
        {
            animator.SetBool("SonicIdle", true);
            animator.SetBool("SonicRun", false);
            // SonicDust.gameObject.SetActive(false);
        }

        if (Input.GetKey(KeyCode.A))
        {
            animator.SetBool("SonicIdle", false);
            animator.SetBool("SonicRun", true);
        }

        if (Input.GetKey(KeyCode.D))
        {
            animator.SetBool("SonicIdle", false);
            animator.SetBool("SonicRun", true);
        }
    }
}
