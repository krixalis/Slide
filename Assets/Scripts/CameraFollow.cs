using UnityEngine;

public class CameraFollow : MonoBehaviour 
{
    public Transform _player;
    public float TrackingSpeed = 8.0f;
    public float ZoomSpeed = 5.0f;

    private float t;

	// Use this for initialization
	void Start () 
    {
        if (_player == null)
            _player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (_player == null)
            return;
        Vector3 pos = _player.position;

        pos.z = transform.position.z;

        transform.position = Vector3.Lerp(transform.position, pos, TrackingSpeed * Time.deltaTime);
	}
}
