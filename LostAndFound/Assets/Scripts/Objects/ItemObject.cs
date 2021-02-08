using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : Interactable
{
    [HideInInspector]
    public Rigidbody _rb;

    [HideInInspector]
    public bool attached;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        float randomForce = Random.Range(0.5f, 2f);
        _rb.AddForce(randomDirection * randomForce, ForceMode.Impulse);
        Vector3 randomRotation = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        float randomRotationForce = Random.Range(0.08f, 0.125f);
        _rb.AddTorque(randomRotation * randomRotationForce, ForceMode.Impulse);
    }

    public void Grab(Transform itemSlot)
    {
        _rb.isKinematic = true;
        attached = true;
        transform.SetParent(itemSlot);
        transform.position = itemSlot.position;
        transform.rotation = itemSlot.rotation;
    }

    public void Release()
    {
        _rb.isKinematic = false;
        attached = false;
        transform.SetParent(null);
    }
}
