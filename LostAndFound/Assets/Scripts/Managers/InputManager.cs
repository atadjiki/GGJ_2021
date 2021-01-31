using System.Collections;
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
    }
}
