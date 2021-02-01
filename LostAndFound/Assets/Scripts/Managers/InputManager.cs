using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;

    public static InputManager Instance { get { return _instance; } }

    private PlayerInputActions Actions;

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

        Build();
    }

    internal void Build()
    {
        Actions = new PlayerInputActions();

        Actions.Player.LeftArmFire.performed += ctx => HandleLeftArmFire();
        Actions.Player.RightArmFire.performed += ctx => HandleRightArmFire();

        Actions.Enable();

        Cursor.visible = false;
    }

    public void ToggleControls(bool flag)
    {
        if(flag)
        {
            Actions.Enable();
        }
        else
        {
            Actions.Disable();
        }
    }

    public void HandleLeftArmFire()
    {
        PlayerController.Instance.AttemptLeftArmFire();
    }

    public void HandleLeftArmReel()
    {
        if(Actions.Player.LeftArmReel.ReadValue<float>() != 0)
        {
            PlayerController.Instance.AttemptLeftArmReel();
        }
    }

    public void HandleRightArmFire()
    {
        PlayerController.Instance.AttemptRightArmFire();
    }

    public void HandleRightArmReel()
    {
        if (Actions.Player.RightArmReel.ReadValue<float>() != 0)
        {
            PlayerController.Instance.AttemptRightArmReel();
        }
    }

    public void HandleLook()
    {
        Vector2 LookVector = Actions.Player.Look.ReadValue<Vector2>();

        PlayerController.Instance.HandleLook(LookVector.x, LookVector.y);
    }

    private void FixedUpdate()
    {
        if(Actions.Player.Start.triggered)
        {
            //pull up menu
        }

        HandleLook();
        HandleLeftArmReel();
        HandleRightArmReel();
    }
}
