using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public RawImage crosshair;

    private static UIManager _instance;

    public static UIManager Instance { get { return _instance; } }

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

    private void ToggleCrosshair(bool flag)
    {
        crosshair.enabled = flag;
    }

    public void SetDefault()
    {
        crosshair.color = Color.white;
    }

    public void SetObjectInRange()
    {
        crosshair.color = Color.green;
    }

    public void SetObjectGrabbed()
    {
        ToggleCrosshair(false);
    }

    public void SetObjectReleased()
    {
        ToggleCrosshair(true);
    }

}
