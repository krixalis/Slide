using UnityEngine;

public class CameraFollow : MonoBehaviour 
{
    public Transform Player;
    public float TrackingSpeed = 8.0f;
    public float ZoomSpeed = 5.0f;
    public float AddedHeight = 4.0f;

    private float t;

    // Use this for initialization
    void Start () 
    {
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    // Update is called once per frame
    void Update () 
    {
        if (Player == null)
            return;
        Vector3 pos = Player.position;

        pos.z = transform.position.z;
        pos.y = Player.position.y + AddedHeight;
        

        transform.position = Vector3.Lerp(transform.position, pos, TrackingSpeed * Time.deltaTime);
        transform.LookAt(Player);
    }
}
