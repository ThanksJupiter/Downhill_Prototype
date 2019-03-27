using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerCollisionSphere collisionSphere;

    float horizontalRotation;
    float verticalInput;

    public float speed = 5;
    public float rotationSpeed = 5;
    public float slowDownThreshold = .5f;
    public float slowDownAmount = 1f;
    public float jumpForce = 15f;

    [SerializeField]
    private Vector3 velocity;
    bool jump = false;

    bool isGrounded;

    private void Awake()
    {

    }

    void Start()
    {
        collisionSphere.OnBecomeGrounded += BecomeGrounded;
        collisionSphere.OnLeftGround += StopBeingGrounded;
    }

    private void OnDisable()
    {
        collisionSphere.OnBecomeGrounded -= BecomeGrounded;
        collisionSphere.OnLeftGround -= StopBeingGrounded;
    }

    private void FixedUpdate()
    {
        if (horizontalRotation != 0)
        {
            Vector3 eulerAngleVelocity = new Vector3(0f, horizontalRotation * rotationSpeed, 0f);
            Quaternion deltaRotation = Quaternion.Euler(transform.rotation.eulerAngles + eulerAngleVelocity * Time.deltaTime);

            transform.SetPositionAndRotation(transform.position, deltaRotation);
        }

        if (verticalInput != 0)
        {
            Vector3 torque = transform.right * verticalInput * speed;
            collisionSphere.rb.AddTorque(torque);
        }

        if (jump)
        {
            collisionSphere.rb.velocity += transform.up * jumpForce;
            collisionSphere.collision = null;
            jump = false;
        }

        velocity = collisionSphere.rb.velocity;
        Vector3 flatVelocity = new Vector3(velocity.x, 0f, velocity.z);
        float dot = Vector3.Dot(flatVelocity.normalized, transform.forward);

        Debug.DrawLine(transform.position, transform.position + flatVelocity, Color.blue);
        Debug.DrawLine(transform.position, transform.position + transform.forward * 5f, Color.red);

        if (isGrounded && Mathf.Abs(dot) < slowDownThreshold)
        {
            collisionSphere.rb.velocity -= velocity * slowDownAmount * Time.deltaTime;
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            horizontalRotation = -1;
        } else if (Input.GetKey(KeyCode.D))
        {
            horizontalRotation = 1;
        }
        else
        {
            horizontalRotation = 0;
        }

        if (Input.GetKey(KeyCode.W))
        {
            verticalInput = 1;
        } else if (Input.GetKey(KeyCode.S))
        {
            verticalInput = -1;
        }
        else
        {
            verticalInput = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }
    }

    void BecomeGrounded()
    {
        isGrounded = true;
    }

    void StopBeingGrounded()
    {
        isGrounded = false;
    }
}
