using Assets.Scripts.Player;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public string DoorType;
    public bool IsDoorOpen;

    private float _startTime;
    private float _fracDistance;
    private float _distance;
    public float DistanceCovered;
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
        if(PullSpeed == 0f) PullSpeed = 3.5f;
        
        TargetPoint = transform.FindChild("TargetPoint").gameObject;
        TargetPos = TargetPoint.transform.position;
        
    
        if (DoorType == "")
        {
            Debug.Log("Did not set DoorType!");
            DoorType = "Exit";
            IsDoorOpen = false;
        }
    }
    
    // Update is called once per frame
    void FixedUpdate () 
    {
        DistanceCovered = (Time.time - _startTime) * PullSpeed;
        _fracDistance = DistanceCovered / _distance;

        if (_player != null && DoorType == "Exit" && IsPulling)
        {
            if (!_playerIsDisabled) DisablePlayer();
            PullPlayer();
        }
    }
     
    
    private void OpenDoor()
    {
        GetComponent<Animation>()["Open"].speed = 1f;
        GetComponent<Animation>().Play("Open");

        IsDoorOpen = true;
    }
    
    private void CloseDoor()
    {
        GetComponent<Animation>()["Open"].speed = -1f;
        GetComponent<Animation>().Play("Open");
    
        IsDoorOpen = false;
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
        _player.transform.position = Vector3.Lerp(_player.transform.position, TargetPos, _fracDistance);
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
            _distance = Vector3.Distance(_player.transform.position, TargetPos);

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
