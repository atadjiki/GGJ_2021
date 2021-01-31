﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;

    public static InputManager Instance { get { return _instance; } }

    public KeyCode Interact_Left = KeyCode.A;
    public KeyCode Interact_Right = KeyCode.D;
    public KeyCode Pause = KeyCode.Tab;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(Interact_Left) || Input.GetMouseButtonDown(0) || Input.GetButtonDown("LeftArmFire"))
        {
            PlayerController.Instance.AttemptLeftArmFire();
        }
        else if(Input.GetAxis("LeftArmReel") != 0)
        {
            PlayerController.Instance.AttemptLeftArmReel();
        }

        float h, v;
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            PlayerController.Instance.HandleLook(h, v);
        }
        else if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            h = Input.GetAxis("Mouse X");
            v = Input.GetAxis("Mouse Y");
            PlayerController.Instance.HandleLook(h, v);
        }

    }
}
