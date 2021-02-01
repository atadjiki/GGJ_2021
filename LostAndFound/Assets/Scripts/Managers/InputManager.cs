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

        if (Input.GetKeyDown(Interact_Right) || Input.GetMouseButtonDown(0) || Input.GetButtonDown("RightArmFire"))
        {
            PlayerController.Instance.AttemptRightArmFire();
        }
        else if (Input.GetAxis("RightArmReel") != 0)
        {
            PlayerController.Instance.AttemptRightArmReel();
        }

        float h, v;
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.5f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.5f)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            PlayerController.Instance.HandleLook(h, v);
        }
        else if (Mathf.Abs(Input.GetAxis("Mouse X")) > 0.5f || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.5f)
        {
            h = Input.GetAxis("Mouse X");
            v = Input.GetAxis("Mouse Y");
            PlayerController.Instance.HandleLook(h, v);
        }

    }
}
