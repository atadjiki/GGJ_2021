using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    Transform player;

    private void Start()
    {
        player = PlayerController.Instance.gameObject.transform;
    }

    private void Update()
    {
        transform.LookAt(player);
    }
}
