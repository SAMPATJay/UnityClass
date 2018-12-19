using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstController : MonoBehaviour
{
    //SerializeField

    [Range(1, 10), SerializeField]
    //Inital player speed
    float playerSpeed = 2f;
    [Range(0, 10), SerializeField]
    //Player soeed when sprinting
    float sprintSpeed;
    [Range(100, 500), SerializeField]
    //Player's initial jump force for the first jump
    float initialJumpForce;
    [Range(1, 5), SerializeField]
    //Deciding the number of jumps allowed including the first one
    int numberOfJumpsPermitted;
    [Range(0.1f, 1f), SerializeField]
    //Reduction factor for each progressive jump
    float jumpForceReduction = 0.8f;
    [SerializeField]
    bool bkinematic;


    //Private

    Rigidbody2D rigidBody;
    bool bFacingRight = true;
    float moveX = 0f, moveY = 0f;
    float jumpForce=0f;
    float jumpCount = 0f;
    bool isMoving, isRunning,isJumping, bgrounded;


    //Public

    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundColliderRadius;
    public Animator anim;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        if (rigidBody.bodyType == RigidbodyType2D.Kinematic)
        {
            bkinematic = true;
        }
        jumpCount = 0f;
    }

    void Update()
    {
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        //Calling the Jump and checkPlayerState functions 
        Jump();
        checkPlayerState();
        bgrounded = Physics2D.OverlapCircle(groundCheck.position, groundColliderRadius, groundMask);
        if (bkinematic)
        {
            //if Kinematic, we allow the player to sprint in both axes
            if (isRunning)
            {
                rigidBody.velocity = new Vector2(sprintSpeed * moveX, sprintSpeed * moveY);
            }
            else
            {
                rigidBody.velocity = new Vector2(playerSpeed * moveX, playerSpeed * moveY);
            }
        }
        else
        {
            //if not kinematice, we only allow the player to sprint only on the horizontal axes
            if (isRunning)
            {
                rigidBody.velocity = new Vector2(sprintSpeed * moveX, rigidBody.velocity.y);
            }
            else
            {
                rigidBody.velocity = new Vector2(playerSpeed * moveX, rigidBody.velocity.y);
            }
        }

        if (moveX > 0 && !bFacingRight || moveX < 0 && bFacingRight)
        {
            FlipSprite();
        }
    }

    void FlipSprite()
    {
        //Using localScale to flip the sprite
        bFacingRight = !bFacingRight;
        Vector3 spriteLocalScale = transform.localScale;
        spriteLocalScale.x *= -1;
        transform.localScale = spriteLocalScale;
    }

    void checkPlayerState()
    {
        //This function allows setting animation variables for the animation controller
        anim.SetBool("Moving", isMoving);
        anim.SetBool("Running", isRunning);
        anim.SetBool("Jumping", isJumping);
        if (rigidBody.velocity.x != 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
        if (bgrounded)
        {
            isJumping = false;
        }
        else
        {
            isJumping = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        //Enabling gizmos for the editor
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.position, groundColliderRadius);
    }

    void Jump()
    {
        if (bgrounded)
        {
            //resetting jumpCount when player hits the ground
            jumpCount = (numberOfJumpsPermitted - 1);
            jumpForce = initialJumpForce;
        }
        if ((bgrounded || jumpCount>0f)&& Input.GetKeyDown(KeyCode.Space))
        {
            //jumping when conditions are satisfied and reducing number of available jumps
            rigidBody.AddForce(new Vector2(0, jumpForce));
            jumpCount--;
            jumpForce *= jumpForceReduction;
        }
        
    }
}
