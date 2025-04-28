using UnityEngine;
using UnityEngine.InputSystem;

public class JumpAction : PlayerAction
{
    public void OnJump(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
            Jump();
    }

    void OnEnable()
    {
        playerPhysics.onGroundEnter += OnGroundEnter;
    }

    void OnDisable()
    {
       playerPhysics.onGroundEnter -= OnGroundEnter;
    }

    void OnGroundEnter()
    {
        currentJumps = jumps;
    }

    [SerializeField] int jumps;

    [SerializeField] float jumpForce;

    [SerializeField] float airJumpForce;

    int currentJumps;

    void Jump()
    {     
        if (currentJumps <= 0) return;  
        
        currentJumps--;

        float jumpForce = groundInfo.ground ? this.jumpForce : airJumpForce;

        rb.linearVelocity = (groundInfo.normal * jumpForce)
            + playerPhysics.horizontalVelocity;
    }
}
