using UnityEngine;

public class PelletBehaviour : MonoBehaviour
{
    public Vector3 PelletDirection;

    public float PelletVelocity { get; set; }

    private Rigidbody _rigidBody;
    private AimManager _aimManager;
    private Vector3 _target;
    private Vector3 _direction;
    private float _timeShot;
    private float _maxLiveTime;
    void Start()
    {
        if (PelletVelocity == 0) Debug.Log("Velocity on pellet not set!");

        _timeShot = Time.time;
        _maxLiveTime = 2f;

        _aimManager = GameObject.Find("Aim").GetComponent<AimManager>();
        _target = _aimManager.AimAtPosition;

        transform.LookAt(_target);
        transform.Rotate(Vector3.right, 90f); //point one end to where it's "looking"

        _direction = (_target - transform.position).normalized;
        
        _rigidBody = transform.GetComponent<Rigidbody>();
        _rigidBody.velocity = _direction * PelletVelocity;
    }

    void Update()
    {
        if(Time.time >= _timeShot + _maxLiveTime) Destroy(transform.gameObject);
    }

    void OnTriggerEnter(Collider otherCollider) //OnTriggerEnter, rather
    {
        if (otherCollider.gameObject.tag != "Player" && !otherCollider.isTrigger)
        {
            Destroy(transform.gameObject); //Do this after dealing damage etc.
        }
    }
}
