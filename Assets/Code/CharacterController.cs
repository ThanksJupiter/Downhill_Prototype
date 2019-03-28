using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 5;
    [SerializeField] private float rotationSpeed;


    [SerializeField] private float tiltFloat;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float gravity;
    [SerializeField] private float checkGroundDistance;
    [SerializeField] private float friction;

    [SerializeField] private Vector3 velocity;
    [SerializeField] private float velocityMagnitude;
    [SerializeField] private Vector3 lastFrameVelocity;

    [SerializeField] private Vector2 inputVector;

    [SerializeField] private Vector3 projectedVector;
    [SerializeField] private Vector3 desiredTravelDirection;

    [Range(0, 1)]
    public float alpha;

    delegate void OnLandedDelegate();
    OnLandedDelegate OnLanded;
    bool isGrounded;

    private void OnEnable()
    {
        OnLanded += BroadcastLanded;
    }

    private void OnDisable()
    {
        OnLanded -= BroadcastLanded;
    }

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
                velocity.y = 0;
            }
            CalculateDesiredTravelDirection(hit);
            velocityMagnitude = velocity.magnitude;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        transform.rotation = CalculateRotation();
        transform.position += CalculateVelocity();
    }

    private Vector3 CalculateVelocity()
    {
        velocity += desiredTravelDirection * Time.deltaTime;
        return velocity;
    }

    private void CalculateDesiredTravelDirection(RaycastHit hit)
    {
        Vector3 groundCheckDirection = -transform.up * checkGroundDistance;
        bool facingUp = false;

        projectedVector = Vector3.ProjectOnPlane(groundCheckDirection, hit.normal);
        Vector3 flatProject = projectedVector;
        flatProject.y = 0;
        float forwardDot = Vector3.Dot(flatProject.normalized, transform.forward);
        float rightDot = Vector3.Dot(flatProject.normalized, transform.right);

        facingUp = forwardDot < 0;

        desiredTravelDirection = new Vector3(transform.forward.x * projectedVector.x, projectedVector.y * forwardDot, transform.forward.z * projectedVector.z);
        desiredTravelDirection = Quaternion.AngleAxis(rightDot * -Mathf.Rad2Deg, hit.normal) * desiredTravelDirection;

        if (facingUp)
        {
            desiredTravelDirection = Quaternion.AngleAxis(180, projectedVector) * -desiredTravelDirection;
        }

        desiredTravelDirection.Normalize();
    }

    private Quaternion CalculateRotation()
    {
        float theRightDot = Vector3.Dot(transform.right, velocity);

        Vector3 eulerAngleVelocity = new Vector3(0f, inputVector.x * rotationSpeed, 0f);
        Quaternion deltaRotation = Quaternion.Euler(transform.rotation.eulerAngles + eulerAngleVelocity * Time.deltaTime);

        return deltaRotation;
    }

    private void CheckGround()
    {
        if (!isGrounded && Physics.Raycast(transform.position, -transform.up, checkGroundDistance))
        {
            OnLanded?.Invoke();
        }

        isGrounded = Physics.Raycast(transform.position, -transform.up, checkGroundDistance);
    }

    private void BroadcastLanded()
    {
        print("OnLanded invoke");
    }

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
