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
        HandleLook();

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
                    Debug.Log("Arm has attached to " + Arm.GetAttachedTo().name);
                    Player_Rigidbody.useGravity = false;

                }
                //if we have hit a collectable, tether the player to that object
                else if (hit.transform.gameObject.GetComponent<ItemObject>() != null && Vector3.Distance(this.transform.position, hit.transform.position) <= Arm.Range)
                {
                    Arm.GrabObject(hit.transform.gameObject.GetComponent<ItemObject>());
                    Debug.Log("Arm has grabbed " + Arm.GetAttachedTo().name);
                }
            }
        }
        else if(Arm.CurrentState == StickyArm.State.AttachedToItem || Arm.CurrentState == StickyArm.State.AttachedToSurface)
        {
            Arm.Release();
            Debug.Log("Arm released");
            Player_Rigidbody.useGravity = true;
        }
        
    }

    public void AttemptLeftArmReel()
    {
        if(Arm.IsAttached())
        {
            if(Arm.CurrentState == StickyArm.State.AttachedToSurface)
            {
                Vector3 targetPos = Arm.GetAttachedTo().GetComponent<Collider>().ClosestPointOnBounds(Arm.GetAttachedTo().gameObject.transform.position);

                Vector3 currentPos = Player_Rigidbody.position;

                Vector3 deltaPos = Vector3.Slerp(currentPos, targetPos, Time.fixedDeltaTime * ReelTowardsSpeed);

                Player_Rigidbody.MovePosition(deltaPos);
            }
            else if(Arm.CurrentState == StickyArm.State.AttachedToItem)
            {
                Vector3 targetPos = Arm.GetAttachedTo().GetComponent<Collider>().ClosestPointOnBounds(Arm.GetAttachedTo().gameObject.transform.position);

                Vector3 currentPos = Arm.GetArmOrigin();

                Vector3 deltaPos = Vector3.Slerp(targetPos, currentPos, Time.fixedDeltaTime * ReelInwardsSpeed);

                Arm.GetAttachedTo().GetComponent<Rigidbody>().MovePosition(deltaPos); 
            }
            
        }
        
    }

    internal void HandleLook()
    {
        float h = 0;
        float v = 0;

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            h = horizontalSpeed * Input.GetAxis("Horizontal");
            v = verticalSpeed * Input.GetAxis("Vertical");
        }
        else
        {
            h = horizontalSpeed * Input.GetAxis("Mouse X");
            v = verticalSpeed * Input.GetAxis("Mouse Y");
        }


        //rotate player transform left and right

        Quaternion deltaRotation = Quaternion.Euler(new Vector3(-1 * v * verticalSpeed, h * horizontalSpeed, 0) * Time.fixedDeltaTime);

        Quaternion targetRotation = Quaternion.Slerp(Player_Rigidbody.rotation, Player_Rigidbody.rotation * deltaRotation, Time.fixedDeltaTime * Lerp_Rotate);

        Player_Rigidbody.MoveRotation(targetRotation);
    }
}
