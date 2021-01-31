using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyArm : MonoBehaviour
{ 

    private Interactable AttachedTo = null;

    private LineRenderer ArmRenderer;
    public int ArmDensity = 2;//how many points to have in the arm

    private LineOrigin originObject;
    
    public enum State { Empty, AttachedToSurface, AttachedToItem };
    public State CurrentState = State.Empty;

    private void Awake()
    {
        ArmRenderer = GetComponent<LineRenderer>();
        originObject = GetComponentInChildren<LineOrigin>();

        ArmRenderer.widthMultiplier = 0.2f;
        ArmRenderer.positionCount = ArmDensity;
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        if(IsAttached())
        {
            UpdateArmRenderer();
        }
    }

    public bool IsAttached()
    {
        return (AttachedTo != null);
    }

    public Interactable GetAttachedTo()
    {
        if(IsAttached())
        {
            return AttachedTo;
        }
        else
        {
            return null;
        }
    }

    public void RegisterInteractable(Interactable newObject)
    {
        AttachedTo = newObject;
        ArmRenderer.enabled = true;
        UpdateArmRenderer();
    }

    public void GrabObject(ItemObject itemObject)
    {
        AttachedTo = itemObject;
        ArmRenderer.enabled = true;
        UpdateArmRenderer();

        CurrentState = State.AttachedToItem;
    }

    public void GrabSurface(GrappleObject grappleObject)
    {
        AttachedTo = grappleObject;
        ArmRenderer.enabled = true;

        UpdateArmRenderer();

        CurrentState = State.AttachedToSurface;
    }

    public void Release()
    {

        AttachedTo = null;
        ArmRenderer.enabled = false;

        CurrentState = State.Empty;
    }

    private void UpdateArmRenderer()
    {
        Vector3[] points = new Vector3[ArmDensity];
        points[0] = originObject.transform.position;
        points[points.Length-1] = AttachedTo.transform.position;

        ArmRenderer.SetPositions(points);
    }

    public Vector3 GetArmOrigin()
    {
        return originObject.transform.position;
    }
}