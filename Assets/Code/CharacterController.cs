using System.Collections;
using System.Collections.Generic;
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

    [Range(0, 1)]
    public float alpha;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // TODO set tilt float
        inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        Vector3 groundCheckDirection = -transform.up * checkGroundDistance;

        //if (IsGrounded())
        //    velocity *= -friction;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, groundCheckDirection.normalized, out hit, checkGroundDistance))
        {
            projectedVector = Vector3.ProjectOnPlane(groundCheckDirection, hit.normal);
            Vector3 flatProject = projectedVector;
            flatProject.y = 0;
            float forwardDot = Vector3.Dot(flatProject.normalized, transform.forward);

            //velocity = Vector3.Lerp(transform.forward, projectedVector.normalized, forwardDot);

            velocity = projectedVector * forwardDot * speedMultiplier;
            velocity = Quaternion.AngleAxis(inputVector.x * rotationSpeed, transform.up) * velocity;

            //velocity *= /*projectedVector.magnitude **/ forwardDot * speedMultiplier;
            

            print("forwardDot: " + forwardDot + " projVel.y: " + projectedVector.y);
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        float theRightDot = Vector3.Dot(transform.right, velocity);

        Vector3 eulerAngleVelocity = new Vector3(0f, inputVector.x * rotationSpeed, 0f);
        Quaternion deltaRotation = Quaternion.Euler(transform.rotation.eulerAngles + eulerAngleVelocity * Time.deltaTime);

        velocityMagnitude = velocity.magnitude;
        transform.rotation = deltaRotation;
        transform.position += velocity * Time.deltaTime;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -transform.up, checkGroundDistance);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + projectedVector * 10f);
        Gizmos.DrawWireSphere(transform.position + projectedVector * 10f, .3f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + velocity * 10f);
        Gizmos.DrawWireSphere(transform.position + velocity * 10f, .3f);
    }
}
