using UnityEngine;

public class ToggleWall : MonoBehaviour
{
    private bool _locked;
    public bool WallUp;
    private bool _triggered;
    public bool LockAfterUse;


	void Start ()
	{
	    _triggered = false;
	}
	
	void FixedUpdate () {
	    if (_triggered)
	    {
	        TriggerWall();
	    }
	}

    private void TriggerWall()
    {
        if (_locked) return;

        if (WallUp && !animation["Retract"].enabled && !animation["Extend"].enabled)
        {
            animation["Retract"].speed = 2;
            animation.Play("Retract");
            WallUp = false;
            _triggered = false;
        }
        else if (!WallUp && !animation["Retract"].enabled && !animation["Extend"].enabled)
        {
            animation["Extend"].speed = 2;
            animation.Play("Extend");
            WallUp = true;
            _triggered = false;
        }

        if (LockAfterUse) _locked = true;
    }

    public void Trigger()
    {
        if (_locked)
        {
            Debug.Log("Wall was tried to be triggered but is locked.");
            return;
        }
        if (!_triggered) _triggered = true;
    }
}
