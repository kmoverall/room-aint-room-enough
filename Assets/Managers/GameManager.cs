/// Note: that the more namespaces we use the more loading this screen has to do
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager> {
    protected GameManager() { }

    public Animator UIAnim;

    public GameObject player1;
    public GameObject player2;

    public Text player1AmmoText;
    public Text player2AmmoText;


    void Update() {
        player1AmmoText.text = player1.GetComponentInChildren<WallChucker>().ammo.ToString(); 
        player2AmmoText.text = player2.GetComponentInChildren<WallChucker>().ammo.ToString();

        if (Input.anyKeyDown && !InputManager.IsPlaying) {
            UIAnim.SetTrigger("Continue");
        }
    }

    public static void ShowRedWin() 
    {
        InputManager.IsPlaying = false;
        Instance.UIAnim.SetTrigger("RedWin");
    }

    public static void ShowBlueWin()
    {
        InputManager.IsPlaying = false;
        Instance.UIAnim.SetTrigger("BlueWin");
    }

    public static void ShowDraw()
    {
        InputManager.IsPlaying = false;
        Instance.UIAnim.SetTrigger("Draw");
    }
}