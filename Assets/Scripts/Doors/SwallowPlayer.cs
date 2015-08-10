using Assets.Scripts.Player;
using UnityEngine;

public class SwallowPlayer : MonoBehaviour
{
    public bool IsExit;

    public GameObject Door;
    public DoorTrigger DoorTrigger;

    public GameObject TargetPointBehindDoor;
    public Vector3 TargetPosBehindDoor;

    private float _startTime;
    private float _fracDistance;
    private float _distance;
    public float DistanceCovered;

    public float PullSpeed;
    public bool IsPulling;

    private GameObject _player;
    private CharControl _playerCharControl;
    private bool _playerIsDisabled;

    public string SceneName;
    
	// Use this for initialization
	void Start ()
	{
	    if(PullSpeed == 0f) PullSpeed = 18.0f;

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

        DistanceCovered = (Time.time - _startTime) * PullSpeed;
        _fracDistance = DistanceCovered / _distance;

        if (_player != null && DoorTrigger.DoorType == "Exit" && IsPulling)
        {
            _player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            if(!_playerIsDisabled) DisablePlayer();
            PullPlayer();
        }
    }

    private void DisablePlayer()
    {
        _player.GetComponent<Rigidbody>().useGravity = false;
        _playerCharControl.AllowControl = false;
        _player.transform.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        _playerIsDisabled = true;
    }

    private void PullPlayer()
    {
        GameObject.FindGameObjectWithTag("CameraParent").GetComponent<CameraFollow>().IsDisabled = true; //TODO: Need to center camera on door
        _player.transform.position = Vector3.Lerp(_player.transform.position, TargetPosBehindDoor, _fracDistance);
        Debug.Log("pullin him in lads");

        if (!string.IsNullOrEmpty(SceneName)) Application.LoadLevel(SceneName); //TODO: actually only load the next scene/position when the player reached the target point
        else
        {
            Debug.Log("Failed to load scene: String is null or empty.");
        }
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
            _startTime = Time.time;
            _distance = Vector3.Distance(_player.transform.position, TargetPosBehindDoor);

            IsPulling = true;
            DoorTrigger.IsPulling = false;
        }
    }
}
