using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Vector3 randomRotation = new Vector3(0f, -1f, 0f).normalized;
        float randomRotationForce = Random.Range(5f, 10f);
        _rb.AddTorque(randomRotation * randomRotationForce, ForceMode.Impulse);
    }
}
