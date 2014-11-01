using System;
using Assets.Scripts.Player;
using UnityEngine;

public class SwallowPlayer : MonoBehaviour
{
    public bool IsExit;

    public GameObject Door;
    public DoorTrigger DoorTrigger;

    public GameObject TargetPointBehindDoor;
    public Vector3 TargetPosBehindDoor;

    public float PullSpeed;
    public bool IsPulling;

    private GameObject _player;
    private CharControl _playerCharControl;
    private bool _playerIsDisabled;
    
	// Use this for initialization
	void Start ()
	{
	    if(PullSpeed == 0f) PullSpeed = 12.0f;

	    IsPulling = false;

	    if (Door == null)
	    {
	        Door = transform.root.gameObject;
	        DoorTrigger = Door.GetComponent<DoorTrigger>();
	    }

	    if (TargetPointBehindDoor == null)
	    {
	        TargetPointBehindDoor = transform.FindChild("TargetPointBehindDoor").gameObject;
	    }
        TargetPosBehindDoor = TargetPointBehindDoor.transform.position;

	    if (DoorTrigger.DoorType == "Exit") IsExit = true;
	}
    
    // Update is called once per frame
    void FixedUpdate ()
    {
        if (!IsExit) return;
        if (_player != null && DoorTrigger.DoorType == "Exit" && IsPulling)
        {
            _player.rigidbody.constraints = RigidbodyConstraints.None;
            if(!_playerIsDisabled) DisablePlayer();
            PullPlayer();
        }
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
        Debug.Log("Pulling into door");
        _player.transform.position = Vector3.Lerp(_player.transform.position, TargetPosBehindDoor, PullSpeed * Time.deltaTime);
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
            IsPulling = true;
            DoorTrigger.IsPulling = false;
        }
    }
}
