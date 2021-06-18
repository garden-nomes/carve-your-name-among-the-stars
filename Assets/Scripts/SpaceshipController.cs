using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    public float maxSpeed = 30f;
    public float timeToMaxSpeed = 3f;
    public float timeToStop = 1f;
    private Vector3 velocity;
    public Vector3 Velocity => velocity;

    public float maxRotationalSpeed = 90f;
    public float timeToMaxRotationalSpeed = 1f;
    public float timeToStopRotating = 0.25f;
    private Vector2 rotationalVelocity;

    private new Rigidbody rigidbody;

    private void Start() { }

    private void Update()
    {
        // we *could* do this in FixedUpdate (and maybe should), but since this isn't using any of
        // Unity's physics system, I think it looks a little smoother updating frame-by-frame

        UpdateRotateShip();
        UpdateAccelerateShip();
    }

    // smoothly rotate ship
    private void UpdateRotateShip()
    {
        // read arrow keys and/or joystick
        var axis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (axis.sqrMagnitude > 0f)
        {
            // accelerate ship rotation
            float rotationalVelocityDelta = (maxRotationalSpeed / timeToMaxRotationalSpeed) * Time.deltaTime;
            rotationalVelocity += axis * rotationalVelocityDelta;

            // clamp rotational speed
            if (rotationalVelocity.sqrMagnitude > maxRotationalSpeed * maxRotationalSpeed)
                rotationalVelocity = rotationalVelocity.normalized * maxRotationalSpeed;
        }
        else if (rotationalVelocity.sqrMagnitude > 0f)
        {
            // decelerate rotation
            float rotationalVelocityDelta = (maxRotationalSpeed / timeToStopRotating) * Time.deltaTime;

            if (rotationalVelocity.sqrMagnitude <= rotationalVelocityDelta * rotationalVelocityDelta)
            {
                rotationalVelocity = Vector2.zero;
            }
            else
            {
                float newMagnitude = rotationalVelocity.magnitude - rotationalVelocityDelta;
                rotationalVelocity = rotationalVelocity.normalized * newMagnitude;
            }
        }

        transform.rotation *= Quaternion.AngleAxis(rotationalVelocity.x * Time.deltaTime, Vector3.up);
        transform.rotation *= Quaternion.AngleAxis(rotationalVelocity.y * Time.deltaTime, Vector3.left);
    }

    private void UpdateAccelerateShip()
    {
        // map forwards/backwards buttons to a 1-dimensional axis
        float axis = 0f;
        if (Input.GetKey(KeyCode.Z)) axis += 1f;
        if (Input.GetKey(KeyCode.X)) axis -= 1f;

        if (axis != 0)
        {
            // accelerate ship
            float velocityDelta = (maxSpeed / timeToMaxSpeed) * Time.deltaTime * axis;
            velocity += transform.forward * velocityDelta;

            // clamp speed
            if (velocity.sqrMagnitude > maxSpeed * maxSpeed)
                velocity = velocity.normalized * maxSpeed;
        }
        else if (velocity.sqrMagnitude > 0f)
        {
            // deccelerate ship
            float velocityDelta = (maxSpeed / timeToStop) * Time.deltaTime;

            if (velocity.sqrMagnitude <= velocityDelta * velocityDelta)
            {
                velocity = Vector3.zero;
            }
            else
            {
                float newMagnitude = velocity.magnitude - velocityDelta;
                velocity = velocity.normalized * newMagnitude;
            }
        }

        transform.position += velocity * Time.deltaTime;
    }
}
