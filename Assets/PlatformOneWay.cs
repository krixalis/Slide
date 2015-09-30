﻿using UnityEngine;

public class PlatformOneWay : MonoBehaviour
{
    public GameObject Player;
    private float _playerFeetPosY;
    public bool help;

	// Use this for initialization
	void Start ()
    {
	    if (Player == null) Player = GameObject.FindWithTag("Player");
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        _playerFeetPosY = Player.transform.position.y - (Player.transform.localScale.y / 2); //player position changes, therefore we need to update the feet position here
	    
	    Physics.IgnoreCollision(GetComponent<Collider>(), Player.GetComponent<Collider>(), transform.position.y > _playerFeetPosY);
    }
}
