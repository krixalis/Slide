using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject Player;
    public Vector3 PlayerPosition;
    public CharacterController _charCtrlr;
    public float TrackingSpeed = 8.0f;
    public float ZoomSpeed = 5.0f;
    public float AddedHeight = 4.0f;
    public bool IsDisabled;

    // Use this for initialization
    void Start ()
    {
        IsDisabled = false;
        if (Player == null) Player = GameObject.Find("Player");
        PlayerPosition = Player.transform.position;
        _charCtrlr = Player.GetComponent<CharacterController>();
    }
    
    // LateUpdate is called after all other Updates every frame.
    void LateUpdate ()
    {
        if (IsDisabled) return;
        if (Player == null) return;

        PlayerPosition = Player.transform.position;
        Vector3 pos = PlayerPosition;

        pos.z = transform.position.z;
        pos.y = PlayerPosition.y + AddedHeight;


        transform.position = pos;
        transform.LookAt(PlayerPosition);
    }
     
}
