﻿using Assets.Scripts.Player;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public string DoorType;
    public bool IsDoorOpen;
    public float PullSpeed;
    public bool IsPulling;

    public GameObject TargetPoint;
    public Vector3 TargetPos;

    private GameObject _player;
    private CharControl _playerCharControl;
    private bool _playerIsDisabled;
    
    // Use this for initialization
    void Start ()
    {
        PullSpeed = 2.0f;
        if (TargetPoint == null)
        {
            TargetPoint = transform.FindChild("TargetPoint").gameObject;
            TargetPos = TargetPoint.transform.position;
        }
    
        if (DoorType == "")
        {
            DoorType = "Exit";
            IsDoorOpen = false;
        }
        if (DoorType == "Start")
            IsDoorOpen = true;
    }
    
    // Update is called once per frame
    void FixedUpdate () 
    {
        if (_player != null && DoorType == "Exit" && IsPulling)
        {
            if (!_playerIsDisabled) DisablePlayer();
            PullPlayer();
        }
    }
     
    
    private void OpenDoor()
    {
        animation["Open"].speed = 1f;
        animation.Play("Open");

        IsDoorOpen = true;
    }
    
    private void CloseDoor()
    {
        animation["Open"].speed = -1f;
        animation.Play("Open");
    
        IsDoorOpen = false;
    }

    private void DisablePlayer()
    {
        _player.rigidbody.useGravity = false;
        _playerCharControl.AllowControl = false;
        _player.transform.rigidbody.velocity = new Vector3(0, 0, 0);
        _playerIsDisabled = true;
    }
    
    private void PullPlayer()
    {
        _player.transform.position = Vector3.Lerp(_player.transform.position, TargetPos, PullSpeed * Time.deltaTime);
    }
    
    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.gameObject.tag == "Player")
        {
            if (_player == null)
            {
                _player = otherCollider.gameObject;
                _playerCharControl = _player.GetComponent<CharControl>();
            }
            switch (DoorType)
            {
                case "Exit":
                    if (!IsDoorOpen)
                    {
                        OpenDoor();
                        IsPulling = true;
                    }
                    break;
                case "Start":
                    if (IsDoorOpen) CloseDoor();
                    break;
                default:
                    Debug.Log("Error: Invalid DoorType.");
                    break;
            }
        }
    }
}
