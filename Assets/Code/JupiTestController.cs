using UnityEngine;

public class JupiTestController : MonoBehaviour
{
    #region Properties
    [SerializeField] private float speedMultiplier = 5f;
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private float mass = 100f;
    [SerializeField] private Vector3 acceleration;

    [SerializeField] private float tiltFloat;
    [SerializeField] private float maxVelocity = 50f;
    [SerializeField] private float gravity = -9.82f;
    [SerializeField] private float checkGroundDistance = 1f;
    [SerializeField] private float friction;

    [SerializeField] private Vector3 velocity;
    [SerializeField] private float velocityMagnitude;
    [SerializeField] private Vector3 lastFrameVelocity;

    [SerializeField] private Vector2 inputVector;

    [SerializeField] private Vector3 projectedVector;
    [SerializeField] private Vector3 desiredTravelDirection;

    [Range(0, 1)]
    public float alpha;

    bool isGrounded;
    #endregion Properties

    #region Delegates
    delegate void OnLandedDelegate();
    OnLandedDelegate OnLanded;

    delegate void OnLeftGroundDelegate();
    OnLeftGroundDelegate OnLeftGround;

    private void OnEnable()
    {
        OnLanded += BroadcastLanded;
        OnLeftGround += BroadcastLeftGround;
    }

    private void OnDisable()
    {
        OnLanded -= BroadcastLanded;
        OnLeftGround -= BroadcastLeftGround;
    }
    #endregion Delegates

    #region Unity Updates
    void Update()
    {
        // TODO set tilt float
        inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        CheckGround();

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, checkGroundDistance))
        {
            if (isGrounded)
            {
                //velocity.y = projectedVector.y;
            }
            CalculateDesiredTravelDirection(hit);
            velocityMagnitude = velocity.magnitude;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        transform.rotation = CalculateRotation();
    }

    private void FixedUpdate()
    {
        velocity += CalculateAcceleration();
        if (isGrounded)
        {
            velocity.y = desiredTravelDirection.y;
        }

        transform.position += velocity * speedMultiplier * Time.fixedDeltaTime;
    }
    #endregion Unity Updates

    #region Physics? calculations
    private Vector3 CalculateVelocity()
    {
        return velocity;
    }

    private Vector3 CalculateAcceleration()
    {
        if (isGrounded)
        {
            acceleration = desiredTravelDirection / mass;
            //velocity += desiredTravelDirection / mass;
        }

        return acceleration;
    }

    private void CalculateDesiredTravelDirection(RaycastHit hit)
    {
        Vector3 normal = hit.normal;
        //Vector3 point = hit.point;

        Vector3 netForce = -transform.up + normal;

        projectedVector = Vector3.ProjectOnPlane(-transform.up, hit.normal);
        Vector3 flatProject = projectedVector;
        flatProject.y = 0;

        float forwardDot = Vector3.Dot(flatProject.normalized, transform.forward);
        float rightDot = Vector3.Dot(flatProject.normalized, transform.right);

        //desiredTravelDirection = (transform.position + netForce) - transform.position;
        desiredTravelDirection = Vector3.ProjectOnPlane(-transform.up, normal);
        desiredTravelDirection = Quaternion.AngleAxis(rightDot * -Mathf.Rad2Deg, hit.normal) * desiredTravelDirection;

        desiredTravelDirection.y *= new Vector3(velocity.x, 0f, velocity.z).magnitude;

        //         Vector3 groundCheckDirection = -transform.up * checkGroundDistance;
        //         bool facingUp = false;
        // 
        //         projectedVector = Vector3.ProjectOnPlane(groundCheckDirection, hit.normal);
        //         Vector3 flatProject = projectedVector;
        //         flatProject.y = 0;
        //         float forwardDot = Vector3.Dot(flatProject.normalized, transform.forward);
        //         float rightDot = Vector3.Dot(flatProject.normalized, transform.right);
        // 
        //         facingUp = forwardDot < 0;
        // 
        //         desiredTravelDirection = new Vector3(transform.forward.x * projectedVector.x, projectedVector.y * forwardDot, transform.forward.z * projectedVector.z);
        //         desiredTravelDirection = Quaternion.AngleAxis(rightDot * -Mathf.Rad2Deg, hit.normal) * desiredTravelDirection;
        // 
        //         if (facingUp)
        //         {
        //             desiredTravelDirection = Quaternion.AngleAxis(180, projectedVector) * -desiredTravelDirection;
        //         }
        // 
        //         desiredTravelDirection = Vector3.ProjectOnPlane(desiredTravelDirection, hit.normal);
        //         desiredTravelDirection.Normalize();
    }

    private Quaternion CalculateRotation()
    {
        float theRightDot = Vector3.Dot(transform.right, velocity);

        Vector3 eulerAngleVelocity = new Vector3(0f, inputVector.x * rotationSpeed, 0f);
        Quaternion deltaRotation = Quaternion.Euler(transform.rotation.eulerAngles + eulerAngleVelocity * Time.deltaTime);

        return deltaRotation;
    }
    #endregion Physics? calculations

    #region State checks
    private void CheckGround()
    {
        bool tmpLanded = Physics.Raycast(transform.position, -transform.up, checkGroundDistance);

        if (!isGrounded && tmpLanded)
        {
            OnLanded?.Invoke();
        }

        if (isGrounded && !tmpLanded)
        {
            OnLeftGround?.Invoke();
        }

        isGrounded = tmpLanded;
    }
    #endregion State checks

    #region Event broadcasts
    private void BroadcastLanded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, checkGroundDistance))
        {
            CalculateDesiredTravelDirection(hit);
            //velocity.y = desiredTravelDirection.y * new Vector3(velocity.x, 0f, velocity.z).magnitude;
            print("setting velocity.y: " + velocity.y);
        }

        //print("OnLanded!, velocity: " + velocity + " desired travel dir: " + desiredTravelDirection);
    }

    private void BroadcastLeftGround()
    {
        print("OnLeftGround!, velocity: " + velocity + " desired travel dir: " + desiredTravelDirection);
    }
    #endregion Event broadcasts

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + projectedVector * 10f);
        Gizmos.DrawWireSphere(transform.position + projectedVector * 10f, .2f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + desiredTravelDirection * 5f);
        Gizmos.DrawWireSphere(transform.position + desiredTravelDirection * 5f, .3f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + velocity);
        Gizmos.DrawWireSphere(transform.position + velocity, .4f);
    }
}
