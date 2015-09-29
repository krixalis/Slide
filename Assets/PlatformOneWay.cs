using UnityEngine;
using System.Collections;

public class PlatformOneWay : MonoBehaviour
{
    public Transform PlayerTransform;
    public float _playerFeetPosY;
    public bool test;

	// Use this for initialization
	void Start ()
    {
        Physics.IgnoreLayerCollision(8, 9, false);

	    _playerFeetPosY = PlayerTransform.position.y - (PlayerTransform.localScale.y / 2);
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        _playerFeetPosY = PlayerTransform.position.y - (PlayerTransform.localScale.y / 2);
        if (transform.position.y <= _playerFeetPosY)
	    {
	        Physics.IgnoreLayerCollision(8, 9, false);
	        test = false;
	    }
	    else
	    {
	        Physics.IgnoreLayerCollision(8, 9, true);
	        test = true;
	    }
    }
}
