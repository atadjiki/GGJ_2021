using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : Interactable
{
    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        float randomForce = Random.Range(0.25f, 1f);
        _rb.AddForce(randomDirection * randomForce, ForceMode.Impulse);
        Vector3 randomRotation = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        float randomRotationForce = Random.Range(0.1f, 0.25f);
        _rb.AddTorque(randomRotation * randomRotationForce, ForceMode.Impulse);
    }
}
