using UnityEngine;

public class WallTrigger : MonoBehaviour
{
    private bool _locked;

    private bool triggered;
    public bool Triggered
    {
        get
        {
            return triggered;
        }
        set 
        {
            if (value != triggered)
            {
                _sentTrigger = false;
                triggered = value;
            }
            
        }
    }
    private bool _sentTrigger;

    public ToggleWall _toggleWall;

	// Use this for initialization
	void Start ()
	{
	    Triggered = false;
	    _sentTrigger = false;

	    if (_toggleWall == null)
	    {
	        _toggleWall = transform.parent.GetComponent<ToggleWall>();
	    }
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (Triggered && !_sentTrigger)
	    {
	        _toggleWall.Trigger();
	        _sentTrigger = true;
	    }
	}

    private void Lock()
    {
        if (!_locked) _locked = true;
        else Debug.Log("Tried to lock an already locked door.");
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (_locked) return;
        if (otherCollider.gameObject.tag == "Player")
        {
            Triggered = true;
        }
    }

    private void OnTriggerExit(Collider otheCollider)
    {
        if (otheCollider.gameObject.tag == "Player")
        {
            Triggered = false;
        }
    }
}
