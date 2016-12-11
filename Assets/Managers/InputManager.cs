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
    Vector2 player1Aim;
    Vector2 player2Aim;

    void Update() {
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

        player1.Move(Input.GetAxis("Horizontal"));
        player2.Move(Input.GetAxis("Horizontal P2"));

        if (Input.GetButtonDown("Jump")) {
            player1.Jump();
        }
        if (Input.GetButtonDown("Jump P2"))
        {
            player2.Jump();
        }

        if (Input.GetButtonDown("Swap"))
        {
            player1.GetComponent<WallChucker>().SwapDirection();
        }
        if (Input.GetButtonDown("Swap P2"))
        {
            player2.GetComponent<WallChucker>().SwapDirection();
        }

        if (Input.GetButtonDown("Throw"))
        {
            player1.GetComponent<WallChucker>().ThrowWall(player1Aim);
        }
        if (Input.GetButtonDown("Throw P2"))
        {
            player2.GetComponent<WallChucker>().ThrowWall(player2Aim);
        }


    }
}