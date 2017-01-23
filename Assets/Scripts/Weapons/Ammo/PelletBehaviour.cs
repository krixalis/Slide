﻿using UnityEngine;

public class PelletBehaviour : MonoBehaviour
{
    public int PelletDirection;

    public float PelletVelocity { get; set; }

    private Rigidbody _rigidBody;

    void Start()
    {
        if (PelletVelocity == 0) Debug.Log("Velocity on pellet not set!");
        _rigidBody = transform.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        _rigidBody.velocity = new Vector3(PelletVelocity, 0, 0);
    }
}