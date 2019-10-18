using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaycastEngine : MonoBehaviour { 

    private enum JumpState {
        None = 0, Holding,
    }

    // layer do chão
    [SerializeField] private LayerMask platformMask;
    // gravidade a ser aplicada
    [SerializeField] private float gravity;
    [SerializeField] private float parallelInsetLen;
    [SerializeField] private float perpendicularInsetLen;
    [SerializeField] private float groundTestLen;
    // movimento
    [SerializeField] private float horizontalSpeedUpAccel;
    [SerializeField] private float horizontalSpeedDownAccel;
    [SerializeField] private float horizontalMaxSpeed;
    [SerializeField] private float horizontalStartUpSpeed;
    // pulo
    [SerializeField] private float jumpInputLeewayPeriod;
    [SerializeField] private float jumpStartSpeed;
    [SerializeField] private float jumpMaxHoldPeriod;
    [SerializeField] private float jumpMinSpeed;
    [SerializeField] private int extraJumpsValue;
    [SerializeField] private float doubleJumpStartSpeed;

    [SerializeField] private GameManager gameManager;
    // guarda a velocidade do objeto
    private Vector2 velocity;

    // atira o raycast
    private RaycastMoveDirection moveDown;
    private RaycastMoveDirection moveLeft;
    private RaycastMoveDirection moveRight;
    private RaycastMoveDirection moveUp;

    // detecção pro que está em baixo
    private RaycastCheckTouch groundDown;

    private float jumpStartTimer;
    private float jumpHoldTimer;
    private bool jumpInputDown;
    private JumpState jumpState;
    private float ySpeed;
    private float xSpeed;
    private bool facingRight;
    private bool grounded;
    private bool haveDoubleJumped = false;
    private bool jumpHit;
    private bool canDoubleJump;
    private int extraJumps;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Weapon weapon;

    [Space]
	[Header("PowerUps")]
	[SerializeField] private bool doubleJump = false;	
	private bool rapidShoot = false;

    void Start() {
        facingRight = true;
        // atira o raycast
        moveDown = new RaycastMoveDirection(new Vector2(-0.5f, -0.70f), new Vector2(0.5f, -0.70f), Vector2.down, platformMask, 
                                            Vector2.right * parallelInsetLen, Vector2.up * perpendicularInsetLen);
        moveLeft = new RaycastMoveDirection(new Vector2(-0.5f, -0.70f), new Vector2(-0.5f, 0.70f), Vector2.left, platformMask, 
                                            Vector2.up * parallelInsetLen, Vector2.right * perpendicularInsetLen);
        moveRight = new RaycastMoveDirection(new Vector2(0.5f, -0.70f), new Vector2(0.5f, 0.70f), Vector2.right, platformMask, 
                                            Vector2.up * parallelInsetLen, Vector2.left * perpendicularInsetLen);
        moveUp = new RaycastMoveDirection(new Vector2(-0.5f, 0.70f), new Vector2(0.5f, 0.70f), Vector2.up, platformMask, 
                                            Vector2.right * parallelInsetLen, Vector2.down * perpendicularInsetLen);


        groundDown = new RaycastCheckTouch(new Vector2(-0.5f, -0.70f), new Vector2(0.5f, -0.70f), Vector2.down, platformMask, 
                                            Vector2.right * parallelInsetLen, Vector2.up * perpendicularInsetLen, groundTestLen);

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        extraJumps = extraJumpsValue;
        weapon = GetComponent<Weapon>();
    }

    private int GetSign(float v) {
        if(Mathf.Approximately(v, 0)) {
            return 0;
        } else if(v > 0) {
            return 1;
        } else {
            return -1;
        } 
    }

    private void Update() {
        if(grounded) {
			extraJumps = extraJumpsValue;
		}

        canDoubleJump = doubleJump;

        jumpStartTimer -= Time.deltaTime;
        bool jumpButton = Input.GetButton("Jump");
        jumpHit = Input.GetButtonDown("Jump");
        if(jumpButton && !jumpInputDown) {
            jumpStartTimer = jumpInputLeewayPeriod;
        }
        jumpInputDown = jumpButton;
    }

    void FixedUpdate() {
        grounded = groundDown.DoRaycast(transform.position);

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        int wantedDirection = GetSign(horizontalInput);
        int velocityDirection = GetSign(velocity.x);

        switch(jumpState) {
            case JumpState.None:
                if(grounded && jumpStartTimer >= 0) {
                    jumpStartTimer = 0;
                    jumpState = JumpState.Holding;
                    jumpHoldTimer = 0;
                    velocity.y = jumpStartSpeed;
                }

                else if(!grounded) {
                    if(doubleJump) {
                        if(jumpHit && extraJumps > 0) {
                            jumpStartTimer = 0;
                            jumpState = JumpState.Holding;
                            jumpHoldTimer = 0;

                            velocity.y = doubleJumpStartSpeed;
                            extraJumps--;
                        }
                    }
                }
                break;
            case JumpState.Holding:
                jumpHoldTimer += Time.deltaTime;
                if(!jumpInputDown || jumpHoldTimer >= jumpMaxHoldPeriod) {
                    jumpState = JumpState.None;
                    jumpHit = false;

                    // lerp!
                    // float p = jumpHoldTimer / jumpMaxHoldPeriod;
                    // velocity.y = jumpMinSpeed + (jumpStartSpeed - jumpMinSpeed) * p;
                    velocity.y = Mathf.Lerp(jumpMinSpeed, jumpStartSpeed, jumpHoldTimer / jumpMaxHoldPeriod);
                }
                break;
        }

        if(jumpState == JumpState.None) {
            velocity.y -= gravity * Time.deltaTime;
        }

        if(wantedDirection != 0) {
            if(wantedDirection != velocityDirection) {
                velocity.x = horizontalStartUpSpeed * wantedDirection;
            } else {
                velocity.x = Mathf.MoveTowards(velocity.x, horizontalMaxSpeed * wantedDirection, horizontalSpeedUpAccel * Time.deltaTime);
            }
        } else {
             velocity.x = Mathf.MoveTowards(velocity.x, 0, horizontalSpeedDownAccel * Time.deltaTime);
        }
        
        if(!Mathf.Approximately(horizontalInput, 0)) {
            velocity.x = Mathf.MoveTowards(velocity.x, horizontalMaxSpeed * Mathf.Sign(horizontalInput), horizontalSpeedUpAccel * Time.deltaTime);
        } else {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, horizontalSpeedDownAccel * Time.deltaTime);
        }

        if(!grounded) {
            ySpeed = velocity.y;
            animator.SetFloat("ySpeed", ySpeed);
            animator.SetBool("isGrounded", false);
            if(ySpeed > 0.01f)
            {
                animator.SetBool("isGrounded", false);
                animator.SetBool("isJumping", true);
            } else {
                animator.SetBool("isJumping", false);
            }
        } else {
            animator.SetBool("isGrounded", true);
            animator.SetBool("isJumping", false);
            xSpeed = velocity.x;
            animator.SetFloat("xSpeed", xSpeed);
            if(xSpeed != 0) {
                animator.SetBool("isWalking", true);
            } else {
                animator.SetBool("isWalking", false);
            }
        }
        
        Vector2 displacement = Vector2.zero;
        Vector2 wantedDisplacement = velocity * Time.deltaTime;
        if(velocity.x > 0) {
            displacement.x = moveRight.DoRaycast(transform.position, wantedDisplacement.x);
        } else if(velocity.x < 0) {
           displacement.x = -moveLeft.DoRaycast(transform.position, -wantedDisplacement.x);
        }

        if(velocity.y > 0) {
            displacement.y = moveUp.DoRaycast(transform.position, wantedDisplacement.y);
        } else if(velocity.y < 0) {
           displacement.y = -moveDown.DoRaycast(transform.position, -wantedDisplacement.y);
        }

        if(!Mathf.Approximately(displacement.x, wantedDisplacement.x)) {
            velocity.x = 0;
        }

        if(!Mathf.Approximately(displacement.y, wantedDisplacement.y)) {
            velocity.y = 0;
        }

        transform.Translate(displacement);

        // if(wantedDirection != 0) {
            // spriteRenderer.flipX = wantedDirection < 0;
        // }

        if(horizontalInput > 0 && !facingRight) {
            Flip();
        } else if (horizontalInput < 0 && facingRight) {
            Flip();
        }   
    }

    private void Flip() { 
		facingRight = !facingRight;
		// transform.Rotate(Vector3.up * 180);

        Vector3 theScale = gameObject.transform.localScale;
        theScale.x *= -1;
        gameObject.transform.localScale = theScale;

        weapon.setFacing(facingRight);
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Collectables")) {
            Collectable collec = col.gameObject.GetComponent<Collectable>();
            verifyPowerUp(collec.GetAbilityName());
            gameManager.UpdateScore(collec.getPoints());
            Destroy(col.gameObject);
        }

        if(col.gameObject.CompareTag("Enemy")) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void verifyPowerUp(string ability) {
        if(ability.Equals("Double Jump") && !doubleJump)
            doubleJump = true;
        if(ability.Equals("Rapid Shoot") && !rapidShoot)
            rapidShoot = true;
    }
}
