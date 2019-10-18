using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
	private int extraJumps;
	[SerializeField] private int extraJumpsValue;
    [SerializeField] private float jumpForce = 15f;						// Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool airControl = false;						// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask whatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform groundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform ceilingCheck;							// A position marking where to check for ceilings
	private bool haveDoubleJumped = false;
    private Animator animator;

	const float groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool grounded;            // Whether or not the player is grounded.
	const float ceilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D rigidbody2D;
    float ySpeed = 0f;

	private bool facingRight = true;  // For determining which way the player is currently facing.
	private Vector3 velocity = Vector3.zero;

	[Space]
	[Header("PowerUps")]
	[SerializeField] private bool doubleJump = false;	
	[SerializeField] private bool rapidShoot = false;

	[Space]
	[Header("Events")]
	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	private void Awake()
	{
		rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
        
        animator = GetComponent<Animator>();

		extraJumps = extraJumpsValue;
	}

	private void Update()
	{
		if(grounded)
		{
			extraJumps = extraJumpsValue;
		}
	}

	private void FixedUpdate()
	{
		bool wasGrounded = grounded;
		grounded = false;

		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);

		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				grounded = true;
				if (!wasGrounded && rigidbody2D.velocity.y < 0)
					OnLandEvent.Invoke();
			}
		}

        animator.SetBool("isGrounded", grounded);
	}

	public void Move(float move, bool jump)
	{
		if (grounded || airControl)
		{
			Vector3 targetVelocity = new Vector2(move * 10f, rigidbody2D.velocity.y);

			rigidbody2D.velocity = Vector3.SmoothDamp(rigidbody2D.velocity, targetVelocity, ref velocity, movementSmoothing);

			if (move > 0 && !facingRight)
			{
				Flip();
			} else if (move < 0 && facingRight)
			{
				Flip();
			}
		}

		if(Input.GetButtonDown("Jump")) {
			if(grounded) {
				grounded = false;
				// rigidbody2D.AddForce(Vector2.up * jumpForce);
				rigidbody2D.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
			} else 
			{
				if(doubleJump && extraJumps > 0) {
					rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
					//rigidbody2D.AddForce(Vector2.up * doubleJumpForce);
					rigidbody2D.AddForce(new Vector2(0f, jumpForce / 1.2f), ForceMode2D.Impulse);
					extraJumps--;
				}
			}
		}

        ySpeed = rigidbody2D.velocity.y;
        animator.SetFloat("ySpeed", ySpeed);
        if(ySpeed > 0.01f)
        {
            animator.SetBool("isGrounded", false);
        } else if (ySpeed > 0.01) {
            animator.SetBool("isGrounded", true);
        }
	}

	private void Flip()
	{
		facingRight = !facingRight;
		transform.Rotate(0f, 180f, 0f);
	}

    public bool isGrounded()
    {
        return grounded;
    }

    public Animator GetAnimator()
    {
        return animator;
    }

	public void setDoubleJump(bool status)
	{
		doubleJump = status;
	}

	public void setRapidShoot(bool status)
	{
		rapidShoot = status;
	}
}