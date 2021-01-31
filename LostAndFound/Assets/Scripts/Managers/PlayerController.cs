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

    private float Lerp_Rotate = 15;

    public StickyArm Arm;

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
            if (hit.transform.gameObject.GetComponent<Interactable>() != null && Vector3.Distance(this.transform.position, hit.transform.position) <= Arm.Range)
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

        if(Arm.CurrentState != StickyArm.State.Empty)
        {
            Player_Rigidbody.velocity = Player_Rigidbody.velocity * 0.9f;
        }
        
    }

    public void AttemptLeftArmFire()
    {

        if(Arm.CurrentState == StickyArm.State.Empty)
        {
            Ray ray = PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //if we have hit a grapple object, tether the player to that object
                if (hit.transform.gameObject.GetComponent<GrappleObject>() != null && Vector3.Distance(this.transform.position, hit.transform.position) <= Arm.Range)
                {
                    Arm.GrabSurface(hit.transform.gameObject.GetComponent<GrappleObject>());

                }
                //if we have hit a collectable, tether the player to that object
                else if (hit.transform.gameObject.GetComponent<ItemObject>() != null && Vector3.Distance(this.transform.position, hit.transform.position) <= Arm.Range)
                {
                    Arm.GrabObject(hit.transform.gameObject.GetComponent<ItemObject>());
                }
            }
        }
        else if(Arm.CurrentState == StickyArm.State.AttachedToItem || Arm.CurrentState == StickyArm.State.AttachedToSurface)
        {
            Arm.Release();
        }
        
    }

    public void AttemptLeftArmReel()
    {
        if(Arm.IsAttached())
        {
            if(Arm.CurrentState == StickyArm.State.AttachedToSurface)
            {
                Vector3 direction = (Arm.GetAttachedTo().transform.position - Arm.GetArmOrigin()).normalized;

                Player_Rigidbody.AddForce(direction * ReelTowardsSpeed);
            }
            else if(Arm.CurrentState == StickyArm.State.AttachedToItem)
            {
                Vector3 direction = (Arm.GetAttachedTo().transform.position - Arm.GetArmOrigin()).normalized * -1;

                Arm.GetAttachedTo().GetComponent<Rigidbody>().AddForce(direction * ReelInwardsSpeed);

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
