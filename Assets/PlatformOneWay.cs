﻿using UnityEngine;

public class PlatformOneWay : MonoBehaviour
{
    public GameObject Player;
    private float _playerFeetPosY;

    private float _platformTopPosY;
    private float _platformBottomPosY;

	// Use this for initialization
	void Start ()
    {
	    if (Player == null) Player = GameObject.FindWithTag("Player");

	    _platformTopPosY = transform.position.y + (transform.localScale.y/2);
	    _platformBottomPosY = transform.position.y - (transform.localScale.y/2);
    }
	
	// Update is called once per frame
	void FixedUpdate ()
	{
	    if (!Player.transform.root.gameObject.activeSelf) return;
        _playerFeetPosY = Player.transform.position.y - (Player.transform.localScale.y / 2); // Player position changes, therefore we need to update the feet position here regularly

        // Here I am using the platform's width as a buffer for determining whether to ignore collisions, because Unity showed some weird inaccuracies if you only compare the player's position to the top of the platform.
        // This means, however, that dropping from the platform and moving back into it will still collide, so the player collides with the sides. Maybe I can find a way to fix this, else I'll have to design around this.
	    if (_platformTopPosY < _playerFeetPosY)
	    {
	        Physics.IgnoreCollision(GetComponent<Collider>(), Player.GetComponent<Collider>(), false);
	    }
        else if (_platformBottomPosY >= _playerFeetPosY)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), Player.GetComponent<Collider>(), true);
        }


	    if (Input.GetAxis("Vertical") < 0 && Input.GetAxis("Fire2") == 1) // If I use Jump here, the jump itself takes priority. Probably have to check for Vertical Axis in the jump functions for this.
	    {
	        Physics.IgnoreCollision(GetComponent<Collider>(), Player.GetComponent<Collider>(), true);
	    }
    }
}
