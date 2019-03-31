using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheFuskestController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speedMultiplier = 5f;
    [SerializeField] private float brakeMultiplier = 5f;
    [SerializeField] private float boardRotationSpeedMultiplier = 5f;
    [SerializeField] private float redirectVelocitySpeed = 5f;
    [SerializeField] private float checkGroundDownDistance = 1.2f;

    [Header("Input")]
    [SerializeField] private float horizontalInput;

    [Header("Player stats")]
    [SerializeField] private float velocity;
    [SerializeField] private Vector3 boardDirection;
    [SerializeField] private float facingSlopeDot;
    [SerializeField] private float facingVelocityDot;

    [Header("Mountain stats")]
    [SerializeField] private Vector3 slopeDirection;

    [Header("Travel stats")]
    public Vector3 travelDirection;
    [SerializeField] private Vector3 slopeAdjustedTravelDirection;
    [SerializeField] private float speedUpAmount;
    [SerializeField] private float slowDownAmount;

    private RaycastHit hit;

    void Start()
    {
        travelDirection = transform.forward;
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        RotateBoard();
        FindGround();

        speedUpAmount = SpeedUp();
        slowDownAmount = SlowDown();

        //slopeAdjustedTravelDirection = new Vector3(travelDirection.x, slopeDirection.y * facingSlopeDot * velocity, travelDirection.z);
        velocity += speedUpAmount; //speed up amount based on board rotation towards slope
        velocity -= slowDownAmount; // speed down amount based on board rotation towards velocity

        //if (velocity < 0)
          //  velocity = 0;

        Vector3 oldPosition = transform.position;
        transform.position = hit.point + transform.up;
        travelDirection = (transform.position - oldPosition).normalized;
        //transform.position = new Vector3(transform.position.x, slopeAdjustedTravelDirection.y, transform.position.z);
    }

    float SpeedUp()
    {
        Vector3 flatSlopeDirection = new Vector3(slopeDirection.x, 0f, slopeDirection.z);
        facingSlopeDot = Vector3.Dot(boardDirection, flatSlopeDirection.normalized);

        return speedMultiplier * facingSlopeDot * Time.deltaTime;
    }

    float SlowDown()
    {
        if (velocity < 0)
            return 0;

        facingVelocityDot = Vector3.Dot(boardDirection, travelDirection.normalized);

        return brakeMultiplier * (1 - facingVelocityDot ) * Time.deltaTime;
    }

    void RotateBoard()
    {
        Vector3 eulerAngleVelocity = new Vector3(0f, horizontalInput * boardRotationSpeedMultiplier * Time.deltaTime, 0f);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + eulerAngleVelocity);
        boardDirection = transform.forward;
    }

    private void FindGround()
    {
        Vector3 raycastDirection = Vector3.Lerp(travelDirection, boardDirection, redirectVelocitySpeed * Time.deltaTime);

        if (Physics.Raycast(transform.position + raycastDirection.normalized * velocity * Time.deltaTime, -transform.up, out hit, checkGroundDownDistance))
        {
            slopeDirection = Vector3.ProjectOnPlane(-transform.up, hit.normal);

            /*
            // do elsewher
            Vector3 rightDirection = Vector3.Cross(hit.normal, slopeDirection);

            Vector3 flatSlopeDir = new Vector3(slopeDirection.x, 0f, slopeDirection.z);
            float rightDot = Vector3.Dot(flatSlopeDir, transform.right);

            travelDirection = Vector3.Lerp(slopeDirection, boardDirection, Mathf.Abs(rightDot));

            DrawDebugLine(rightDirection, 5f, Color.black);
            float angle = Vector3.Angle(slopeDirection.normalized, travelDirection.normalized);
            slopeAdjustedTravelDirection = travelDirection;
            slopeAdjustedTravelDirection.Normalize();
            slopeAdjustedTravelDirection = Quaternion.AngleAxis(angle * facingSlopeDot, rightDirection) * slopeAdjustedTravelDirection;
            print(slopeAdjustedTravelDirection.y);*/
        }
    }

    private void DrawDebugLine(Vector3 direction, float distance, Color color)
    {
        Debug.DrawLine(transform.position, transform.position + direction * distance, color);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + slopeAdjustedTravelDirection.normalized * 5f);
        Gizmos.DrawWireSphere(transform.position + slopeAdjustedTravelDirection.normalized * 5f, .4f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + slopeDirection.normalized * 5f);
        Gizmos.DrawWireSphere(transform.position + slopeDirection.normalized * 5f, .4f);
    }
}
