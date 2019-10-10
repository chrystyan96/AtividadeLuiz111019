using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController controller;
	public float runSpeed = 40f;
	float horizontalMove = 0f;
    float verticalMove = 0f;
	bool jump = false;

    void Start() {
        controller = GetComponent<PlayerController>();
    }
	
	// Update is called once per frame
	void Update () {
		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        controller.GetAnimator().SetFloat("xSpeed", Mathf.Abs(horizontalMove));

		if (Input.GetButtonDown("Jump"))
		{
            jump = true;
            controller.GetAnimator().SetBool("isJumping", true);
		}
	}

	void FixedUpdate ()
	{
		// Move our character
		controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
	}

    public void OnLanding() 
    {
        jump = false;
        controller.GetAnimator().SetBool("isJumping", false);
    }
}
