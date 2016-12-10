﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterController2D))]
public class PlatformerController : MonoBehaviour {

    CharacterController2D controller;
    Animator anim;

    //All parameters are in meters/second
    [SerializeField]
    private float maxWalkSpeed = 5f;
    [SerializeField]
    private float walkAcceleration = 10f;
    [SerializeField]
    private float walkDeceleration = 20f;
    [SerializeField]
    private float jumpVelocity = 9f;
    [SerializeField]
    private float gravityScale = 1f;
    [SerializeField][Range(0,1)]
    private float airControl = 1f;
    [SerializeField]
    private float terminalVelocity = 40f;

    //currentMotion is effectively the current velocity vector in (meters/second)
    private Vector3 currentMotion;
    private bool isFacingRight = true;


    void Awake () {
        controller = gameObject.GetComponent<CharacterController2D>();
        anim = gameObject.GetComponentInChildren<Animator>();
    }

    //Alters currentMotion based on horizontal input
    public void Move (float xIn) {

        //Handle animation parameter
        anim.SetFloat("Speed", Mathf.Abs(xIn));

        //Sprite should face in input direction
        if ((isFacingRight && xIn < 0) || (!isFacingRight && xIn > 0)) {
            FlipSprite();
        }

        //Acceleration is not scaled by deltaTime until it is added to currentMotion
        Vector3 acceleration = new Vector3();

        if (xIn == 0) {
            //Apply deceleration when there is no input
            acceleration.x = Mathf.Sign(currentMotion.x) * walkDeceleration * -1;
        }
        else if (Mathf.Sign(xIn) == Mathf.Sign(currentMotion.x)) {
            //Apply acceleration when moving same direction as input
            acceleration.x = xIn * walkAcceleration;
        }
        else {
            //Apply deceleration and acceleration when moving opposite direction of input
            acceleration.x = xIn * (walkDeceleration + walkAcceleration);
        }
        
        if (!controller.isGrounded) {
            acceleration.x *= airControl;
        }

        //Prevent jittering by setting motion to 0 if acceleration would turn character around and player is putting in no input
        if (xIn == 0 && Mathf.Sign(acceleration.x) != Mathf.Sign(currentMotion.x) && Mathf.Abs(acceleration.x * Time.deltaTime) > Mathf.Abs(currentMotion.x)) {
            currentMotion.x = 0;
        }
        else {
            //Otherwise apply motion as normal
            //All accelerations neeed to be scaled by deltaTime before adding to currentMotion, velocities do not
            currentMotion += acceleration * Time.deltaTime;
            //Clamp to max speed, max speed is reduced if input is < 1.0
            currentMotion.x = Mathf.Clamp(currentMotion.x, -1 * maxWalkSpeed * Mathf.Abs(xIn), maxWalkSpeed * Mathf.Abs(xIn));
        }
    }

    public void Jump () {
        if (controller.isGrounded && !controller.collisionState.above) {
            controller.isJumping = true;
            currentMotion.y = jumpVelocity;
        }
    }
	
    public void FlipSprite () {
        isFacingRight = !isFacingRight;
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }

    void FixedUpdate() {
        currentMotion += new Vector3(Physics2D.gravity.x * gravityScale, Physics2D.gravity.y * gravityScale, 0) * Time.deltaTime;

        currentMotion.x = Mathf.Clamp(currentMotion.x, -1 * terminalVelocity, terminalVelocity);
        currentMotion.y = Mathf.Clamp(currentMotion.y, -1 * terminalVelocity, terminalVelocity);

        controller.move(currentMotion * Time.deltaTime);

        anim.SetBool("Ground", controller.isGrounded);

        // Set the vertical animation
        anim.SetFloat("vSpeed", currentMotion.y);

        //Negate motion if blocked, prevents "hanging" on ceilings and walls
        if ((controller.isGrounded && currentMotion.y < 0) || (controller.collisionState.above && currentMotion.y > 0)) {
            currentMotion.y = 0;
        }
        if ((controller.collisionState.left && currentMotion.x < 0) || (controller.collisionState.right && currentMotion.x > 0)) {
            currentMotion.x = 0;
        }
    }
}
