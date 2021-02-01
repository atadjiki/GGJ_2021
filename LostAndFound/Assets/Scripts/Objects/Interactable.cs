using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    public Vector3 GetAnchorPointLocation()
    {
        if(GetComponentInChildren<AnchorPoint>() != null)
        {
            return GetComponentInChildren<AnchorPoint>().transform.position;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
