using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Vector3 Pos;
    public Vector3 Vel;
    public Vector3 Grav;
    public float MaxXVel;
    public float MaxYVel;
    public bool ApplyGrav;
    public bool ApplyTinyGrav;

    private CharacterController _charCtrlr;
    
    // Use this for initialization
    void Start ()
    {
        _charCtrlr = GetComponent<CharacterController>();
        Pos = transform.position;

        MaxXVel = 0.4f;
        MaxYVel = 2f;
        Grav = new Vector3(0, -0.025f, 0);
    }
    
    // FixedUpdate is called once per tick
    void FixedUpdate()
    {
        Pos = transform.position;

        if (ApplyGrav) AccelerateBy(Grav);
        else Vel.y = 0f;

        MoveBy(Vel);
    }

    public void ClampVelocity() //Don't think this is a good thing to do, actually.
    {
        if (MaxXVel != 0f) Vel.x = Mathf.Clamp(Vel.x, -MaxXVel, MaxXVel);
        if (MaxYVel != 0f) Vel.y = Mathf.Clamp(Vel.y, -MaxYVel, MaxYVel);
    }

    public void AccelerateBy(Vector3 desAccel)
    {
        Vel += desAccel;
        ClampVelocity(); //TODO: Take this away without breaking regular movement. (put the clamp in a MovementHandler)
    }
    
    public void MoveBy(Vector3 velocity)
    {
        Pos += velocity;
        _charCtrlr.Move(velocity);
    }

    public void MoveTo(Vector3 desPosition)
    {
        Pos = desPosition;
        transform.position = Pos;
    }
}
