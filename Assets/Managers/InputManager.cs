/// Note: that the more namespaces we use the more loading this screen has to do
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class InputManager : Singleton<InputManager> {
    protected InputManager() { }

    public PlatformerController player1;
    public PlatformerController player2;

    public static GameObject Player1 { get { return Instance.player1.gameObject; } }
    public static GameObject Player2 { get { return Instance.player2.gameObject; } }

    public bool isPlaying = false;
    public static bool IsPlaying { get { return Instance.isPlaying; } set { Instance.isPlaying = value; } }

    Vector2 player1Aim;
    Vector2 player2Aim;

    void Update() {
        if (!isPlaying) {
            return;
        }
        
        player1Aim = player1.isFacingRight ? Vector2.right : Vector2.left;
        if (Input.GetAxis("Vertical") > 0)
            player1Aim = Vector2.up;
        if (Input.GetAxis("Vertical") < 0)
            player1Aim = Vector2.down;

        player2Aim = player2.isFacingRight ? Vector2.right : Vector2.left;
        if (Input.GetAxis("Vertical") > 0)
            player2Aim = Vector2.up;
        if (Input.GetAxis("Vertical") < 0)
            player2Aim = Vector2.down;
            
        player1.GetComponent<WallBreaker>().BreakWall(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        player2.GetComponent<WallBreaker>().BreakWall(new Vector2(Input.GetAxis("Horizontal P2"), Input.GetAxis("Vertical P2")));

        player1.Move(Input.GetAxis("Horizontal"));
        player2.Move(Input.GetAxis("Horizontal P2"));

        if (Input.GetButtonDown("Jump")) {
            player1.Jump();
        }
        if (Input.GetButtonDown("Jump P2"))
        {
            player2.Jump();
        }

        if (Input.GetButtonUp("Jump"))
        {
            player1.StopJump();
        }
        if (Input.GetButtonUp("Jump P2"))
        {
            player2.StopJump();
        }

        if (Input.GetButtonDown("Throw1"))
        {
            player1.GetComponentInChildren<WallChucker>().ThrowWall(player1Aim, false);
        }
        if (Input.GetButtonDown("Throw1 P2"))
        {
            player2.GetComponentInChildren<WallChucker>().ThrowWall(player2Aim, false);
        }

        if (Input.GetButtonDown("Throw2"))
        {
            player1.GetComponentInChildren<WallChucker>().ThrowWall(player1Aim, true);
        }
        if (Input.GetButtonDown("Throw2 P2"))
        {
            player2.GetComponentInChildren<WallChucker>().ThrowWall(player2Aim, true);
        }
    }
}