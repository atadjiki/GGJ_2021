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

    public Transform Left_ItemSlot;
    public Transform Right_ItemSlot;

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
        
        if(Left_Arm.CurrentState == StickyArm.State.AttachedToSurface || Left_Arm.CurrentState == StickyArm.State.AttachedToItem)
        {
            AttemptLeftArmReel();
        }
        if(Right_Arm.CurrentState == StickyArm.State.AttachedToSurface || Right_Arm.CurrentState == StickyArm.State.AttachedToItem)
        {
            AttemptRightArmReel();
        }
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
                    Debug.Log("Hit on " + hit.transform.gameObject.GetComponent<GrappleObject>().name + " at " + hit.point);

                    arm.RegisterAnchor(hit.point, hit.transform.gameObject.GetComponent<GrappleObject>());

                }
                //if we have hit a collectable, tether the player to that object
                else if (hit.transform.gameObject.GetComponent<ItemObject>() != null && Vector3.Distance(this.transform.position, hit.transform.position) <= Target_Range)
                {
                    Debug.Log("Hit on " + hit.transform.gameObject.GetComponent<ItemObject>().name + " at " + hit.point);
                    arm.RegisterAnchor(hit.point, hit.transform.gameObject.GetComponent<ItemObject>());
                }
            }
        }
        else if (arm.CurrentState == StickyArm.State.AttachedToSurface)
        {
            arm.Release();
        }
        else if(arm.CurrentState == StickyArm.State.AttachedToItem)
        {
            //arm.GetAttachedTo().gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 100f, ForceMode.Impulse);
            arm.GetAttachedTo().gameObject.GetComponent<ItemObject>().Release();
            arm.GetAttachedTo().gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 50f, ForceMode.Impulse);
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
                Vector3 direction = (arm.GetAnchorPointLocation() - arm.GetArmOrigin()).normalized;

                Player_Rigidbody.AddForce(direction * ReelTowardsSpeed);
            }
            else if (arm.CurrentState == StickyArm.State.AttachedToItem)
            {
                Vector3 direction = (arm.GetAnchorPointLocation() - arm.GetArmOrigin()).normalized * -1;

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

    private void OnTriggerEnter(Collider other)
    {
        if (Left_Arm.CurrentState == StickyArm.State.AttachedToItem && Left_Arm.GetAttachedTo().name == other.name)
        {
            Left_Arm.GetAttachedTo().gameObject.GetComponent<ItemObject>().Grab(Left_ItemSlot);
        }
        if (Right_Arm.CurrentState == StickyArm.State.AttachedToItem && Right_Arm.GetAttachedTo().name == other.name)
        {
            Right_Arm.GetAttachedTo().gameObject.GetComponent<ItemObject>().Grab(Right_ItemSlot);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Left_Arm.CurrentState == StickyArm.State.AttachedToItem && Left_Arm.GetAttachedTo().name == other.name)
        {
            if (!Left_Arm.GetAttachedTo().gameObject.GetComponent<ItemObject>().attached)
            {
                Left_Arm.GetAttachedTo().gameObject.GetComponent<ItemObject>().Grab(Left_ItemSlot);
            }
        }
        if (Right_Arm.CurrentState == StickyArm.State.AttachedToItem && Right_Arm.GetAttachedTo().name == other.name)
        {
            if (!Right_Arm.GetAttachedTo().gameObject.GetComponent<ItemObject>().attached)
            {
                Right_Arm.GetAttachedTo().gameObject.GetComponent<ItemObject>().Grab(Right_ItemSlot);
            }
        }
    }
}
