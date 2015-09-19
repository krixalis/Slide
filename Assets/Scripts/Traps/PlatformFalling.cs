using UnityEngine;

public class PlatformFalling : MonoBehaviour
{
    public bool StartFalling;
    private bool _isFalling;
    private float _startTime;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
	    if (StartFalling)
	    {
	        transform.root.GetComponent<Rigidbody>().isKinematic = false;
            transform.root.GetComponent<Rigidbody>().AddForce(new Vector3(0f, 0f, -50f));
	    }
	}

    public void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.tag == "Player")
        {
            _startTime = Time.time;
            StartFalling = true;
        }
    }
}
