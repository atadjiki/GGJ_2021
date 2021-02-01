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

    AnchorPoint currentAnchor;

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

    public void RegisterAnchor(Vector3 HitPoint, ItemObject item)
    {
        CreateAnchorObject(HitPoint, item.transform);
        GrabObject(item);
    }

    public void RegisterAnchor(Vector3 HitPoint, GrappleObject surface)
    {
        CreateAnchorObject(HitPoint, surface.transform);
        GrabSurface(surface);
    }

    internal void GrabObject(ItemObject itemObject)
    {
        AttachedTo = itemObject;
        ArmRenderer.enabled = true;
        UpdateArmRenderer();

        CurrentState = State.AttachedToItem;
    }

    internal void GrabSurface(GrappleObject grappleObject)
    {
        AttachedTo = grappleObject;
        ArmRenderer.enabled = true;

        UpdateArmRenderer();

        CurrentState = State.AttachedToSurface;
    }

    public void Release()
    {

        if(currentAnchor != null && currentAnchor.gameObject != null)
        {
            Destroy(currentAnchor.gameObject);
            currentAnchor = null;
        }

        AttachedTo = null;
        ArmRenderer.enabled = false;

        CurrentState = State.Empty;
    }

    private void UpdateArmRenderer()
    {
        Vector3[] points = new Vector3[ArmDensity];
        points[0] = originObject.transform.position;
        points[points.Length - 1] = GetAnchorPointLocation();

        ArmRenderer.SetPositions(points);
    }

    public Vector3 GetArmOrigin()
    {
        return originObject.transform.position;
    }

    public void CreateAnchorObject(Vector3 hitPoint, Transform parentTransform)
    {
        GameObject AnchorObject = new GameObject();
        AnchorObject.AddComponent<AnchorPoint>();
        AnchorObject.name = "Anchor - " + parentTransform.gameObject.name + " " + (parentTransform.childCount + 1);
        AnchorObject.transform.position = hitPoint;
        AnchorObject.transform.parent = parentTransform;

        currentAnchor = AnchorObject.GetComponent<AnchorPoint>();
    }

    public Vector3 GetAnchorPointLocation()
    {
        if(currentAnchor != null && currentAnchor.gameObject != null)
        {
            return currentAnchor.gameObject.transform.position;
        }
        else
        {
            return Vector3.zero;
        }
    }
}