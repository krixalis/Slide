using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Vector3 Position;
    public Vector3 Velocity;
    public Vector3 Gravity;
    public float MaxXVelocity;
    public float MaxYVelocity;
    public bool ApplyGravity;

    private CharacterController _charCtrlr;
    
    // Use this for initialization
    void Start ()
    {
        _charCtrlr = GetComponent<CharacterController>();
        Position = transform.position;

        MaxXVelocity = 0.4f;
        MaxYVelocity = 2f;
        Gravity = new Vector3(0, -0.025f, 0);
    }
    
    // FixedUpdate is called once per tick
    void FixedUpdate()
    {
        Position = transform.position;

        if (ApplyGravity) AccelerateBy(Gravity);
        else Velocity.y = 0f;

        MoveBy(Velocity);
    }

    public void ClampVelocity()
    {
        if (MaxXVelocity != 0f) Velocity.x = Mathf.Clamp(Velocity.x, -MaxXVelocity, MaxXVelocity);
        if (MaxYVelocity != 0f) Velocity.y = Mathf.Clamp(Velocity.y, -MaxYVelocity, MaxYVelocity);
    }

    public void AccelerateBy(Vector3 desAccel)
    {
        Velocity += desAccel;
        ClampVelocity();
    }


    
    public void MoveBy(Vector3 velocity)
    {
        Position += velocity;
        _charCtrlr.Move(velocity);
    }

    public void MoveTo(Vector3 desPosition)
    {
        Position = desPosition;
        transform.position = Position;
    }
}
