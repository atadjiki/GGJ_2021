using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static PlayerController _instance; //singleton

    [SerializeField] public float horizontalSpeed = 10.0f;
    [SerializeField] public float verticalSpeed = 10.0f;

    [SerializeField] public float ReelTowardsSpeed = 0.1f;
    [SerializeField] public float ReelInwardsSpeed = 0.5f;

    private float Target_Range = 100;

    private float Lerp_Rotate = 15;

    public StickyArm Left_Arm;
    public StickyArm Right_Arm;

    private Rigidbody Player_Rigidbody;
    private Camera PlayerCamera;

    public static PlayerController Instance { get { return _instance; } }

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
        Player_Rigidbody = GetComponent<Rigidbody>();
        PlayerCamera = GetComponent<Camera>();

        Cursor.visible = false;
    }

    private void FixedUpdate()
    {

        Ray ray = PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.GetComponent<Interactable>() != null && Vector3.Distance(this.transform.position, hit.transform.position) <= Target_Range)
            {
                UIManager.Instance.SetObjectInRange();
                
            }
            else
            {
                UIManager.Instance.SetDefault();
            }
        }
        else
        {
            UIManager.Instance.SetDefault();
        }

        if(Left_Arm.CurrentState != StickyArm.State.Empty)
        {
            Player_Rigidbody.velocity = Player_Rigidbody.velocity * 0.9f;
        }
        
    }

    public void CreateAnchorObject(Vector3 hitPoint, Transform parentTransform)
    {
        GameObject AnchorObject = new GameObject();
        AnchorObject.AddComponent<AnchorPoint>();
        AnchorObject.name = "Anchor - " + parentTransform.gameObject.name + " " + parentTransform.childCount+1;
        AnchorObject.transform.position = hitPoint;
        AnchorObject.transform.parent = parentTransform;
    }

    public void AttemptArmFire(StickyArm arm)
    {
        if (arm.CurrentState == StickyArm.State.Empty)
        {
            Ray ray = PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //if we have hit a grapple object, tether the player to that object
                if (hit.transform.gameObject.GetComponent<GrappleObject>() != null && Vector3.Distance(this.transform.position, hit.transform.position) <= Target_Range)
                {
                    Debug.Log("Hit on surface " + hit.transform.gameObject.GetComponent<GrappleObject>().name);
                    CreateAnchorObject(hit.point, hit.transform.gameObject.GetComponent<Interactable>().transform);

                    arm.GrabSurface(hit.transform.gameObject.GetComponent<GrappleObject>());

                }
                //if we have hit a collectable, tether the player to that object
                else if (hit.transform.gameObject.GetComponent<ItemObject>() != null && Vector3.Distance(this.transform.position, hit.transform.position) <= Target_Range)
                {
                    Debug.Log("Hit on object " + hit.transform.gameObject.GetComponent<ItemObject>().name);
                    CreateAnchorObject(hit.point, hit.transform.gameObject.GetComponent<Interactable>().transform);

                    arm.GrabObject(hit.transform.gameObject.GetComponent<ItemObject>()); 
                }
            }
        }
        else if (arm.CurrentState == StickyArm.State.AttachedToItem || arm.CurrentState == StickyArm.State.AttachedToSurface)
        {
            arm.Release();
        }
    }

    public void AttemptRightArmFire()
    {
        AttemptArmFire(Right_Arm);
    }

    public void AttemptRightArmReel()
    {
        AttemptArmReel(Right_Arm);
    }

    public void AttemptLeftArmFire()
    {
        AttemptArmFire(Left_Arm);
    }

    public void AttemptLeftArmReel()
    {
        AttemptArmReel(Left_Arm);
    }


    public void AttemptArmReel(StickyArm arm)
    {
        if (arm.IsAttached())
        {
            if (arm.CurrentState == StickyArm.State.AttachedToSurface)
            {
                Vector3 direction = (arm.GetAttachedTo().GetAnchorPointLocation() - arm.GetArmOrigin()).normalized;

                Player_Rigidbody.AddForce(direction * ReelTowardsSpeed);
            }
            else if (arm.CurrentState == StickyArm.State.AttachedToItem)
            {
                Vector3 direction = (arm.GetAttachedTo().GetAnchorPointLocation() - arm.GetArmOrigin()).normalized * -1;

                arm.GetAttachedTo().GetComponent<Rigidbody>().AddForce(direction * ReelInwardsSpeed);

            }

        }
    }

    
    internal void HandleLook(float h, float v)
    {

        //rotate player transform left and right

        Quaternion deltaRotation = Quaternion.Euler(new Vector3(-1 * v * verticalSpeed, h * horizontalSpeed, 0) * Time.fixedDeltaTime);

        Quaternion targetRotation = Quaternion.Slerp(Player_Rigidbody.rotation, Player_Rigidbody.rotation * deltaRotation, Time.fixedDeltaTime * Lerp_Rotate);

        Player_Rigidbody.MoveRotation(targetRotation);
    }
}
