using UnityEngine;

public class TotalFuskController : MonoBehaviour
{
    #region Properties
    [Header("Settings")]
    [SerializeField] private float speedMultiplier = 5f;
    [SerializeField] private float brakeMultiplier = 10f;
    [SerializeField] private float brakeThreshold = .5f;
    [SerializeField] private float inputBoardRotationSpeed = 100;
    [SerializeField] private float boardRotationSpeed = 1000f;
    [SerializeField] private float travelDirectionLerpSpeed = 200f;
    [SerializeField] private float startTravelThreshold = .2f;

    [SerializeField] private float tiltFloat;
    [SerializeField] private float maxVelocity = 50f;
    [SerializeField] private float gravity = -9.82f;
    [SerializeField] private float friction;

    [SerializeField] private float checkGroundDownDistance = 10f;

    [SerializeField] private float stationaryVelocityThreshold = 2f;

    [Header("Statistics")]
    [SerializeField] private Vector3 inputVector;
    [SerializeField] private Vector3 horizontalTravelDirection;
    [SerializeField] private float velocityMagnitude;

    [SerializeField] private Vector3 boardDirection;

    [SerializeField] private Vector3 currentSlopeDirection;
    [SerializeField] private float currentSlopeAngle;

    [SerializeField] private float faceTowardSlopeAmount;
    [SerializeField] private float faceTowardsTravelDirectionAmount;

    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isBreaking;

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

    [System.Serializable]
    public enum State {
        Travel,
        Stationary
    }

    [SerializeField] private State currentState;

    #endregion Properties

    #region Unity Updates
    private void Start()
    {
        EnterStationaryState();
    }

    void Update()
    {
        CheckGround();

        inputVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        inputVector.Normalize();

        switch (currentState)
        {
            case State.Travel:
                ExecuteTravelState();
                break;
            case State.Stationary:
                ExecuteStationaryState();
                break;
            default:
                break;
        }

        // lean direction

        if (inputVector.magnitude >= .8f)
        {
            boardDirection = Vector3.Lerp(boardDirection, inputVector, inputBoardRotationSpeed * Time.deltaTime);
            //boardDirection = inputVector;
        }

        //horizontalTravelDirection = inputVector;

        Debug.DrawLine(transform.position, transform.position + horizontalTravelDirection, Color.blue);
        Debug.DrawLine(transform.position + horizontalTravelDirection, transform.position + horizontalTravelDirection + -transform.up * checkGroundDownDistance, Color.green);
    }

    #endregion Unity Updates

    #region Physics? calculations

    private Quaternion CalculateRotation()
    {
        float theRightDot = Vector3.Dot(transform.right, boardDirection);

        Vector3 eulerAngleVelocity = new Vector3(0f, theRightDot * boardRotationSpeed, 0f);
        Quaternion deltaRotation = Quaternion.Euler(transform.rotation.eulerAngles + eulerAngleVelocity * Time.deltaTime);

        return deltaRotation;
    }

    private void RotateBoard()
    {
        transform.rotation = CalculateRotation();
    }

    private void MoveBoarder(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    #endregion Physics? calculations

    #region State functions
    private void ExecuteTravelState()
    {
        RotateBoard();
        //print("Execute travel state");
        RaycastHit hit;
        if (Physics.Raycast(transform.position + horizontalTravelDirection * velocityMagnitude * Time.deltaTime, -transform.up, out hit, checkGroundDownDistance))
        {
            currentSlopeDirection = Vector3.ProjectOnPlane(-transform.up, hit.normal);
            Vector3 flatSlopeDirection = new Vector3(currentSlopeDirection.x, 0f, currentSlopeDirection.z);
            currentSlopeAngle = Vector3.Angle(currentSlopeDirection, flatSlopeDirection);

            horizontalTravelDirection = hit.point - transform.position;
            horizontalTravelDirection.y = 0f;
            horizontalTravelDirection.Normalize();

            faceTowardsTravelDirectionAmount = Vector3.Dot(boardDirection, horizontalTravelDirection);
            isBreaking = Mathf.Abs(faceTowardsTravelDirectionAmount) < brakeThreshold;

            faceTowardSlopeAmount = Vector3.Dot(boardDirection.normalized, flatSlopeDirection.normalized);

            isBreaking = true;
            if (isBreaking)
            {
                velocityMagnitude += speedMultiplier * Time.deltaTime * faceTowardSlopeAmount;
                velocityMagnitude -= brakeMultiplier * Time.deltaTime * (1 - faceTowardsTravelDirectionAmount);
                horizontalTravelDirection = Vector3.Lerp(horizontalTravelDirection, boardDirection, travelDirectionLerpSpeed * Time.deltaTime);
            }
            else
            {
                //horizontalTravelDirection = Vector3.Lerp(horizontalTravelDirection, flatSlopeDirection, travelDirectionLerpSpeed * Time.deltaTime);
            }
        }

        if (velocityMagnitude < stationaryVelocityThreshold && Mathf.Abs(faceTowardSlopeAmount) < stationaryVelocityThreshold ) 
        {
            //EnterStationaryState();
            //return;
        }

        transform.position = hit.point + transform.up * 1f;
    }

    private void ExecuteStationaryState()
    {
        RotateBoard();

        Vector3 flatSlopeDirection = new Vector3(currentSlopeDirection.x, 0f, currentSlopeDirection.z);
        faceTowardSlopeAmount = Vector3.Dot(boardDirection, flatSlopeDirection);

        if (Mathf.Abs(faceTowardSlopeAmount) > startTravelThreshold)
        {
            EnterTravelState();
        }
    }

    private void EnterTravelState()
    {
        /*print("Entered: " + currentState);*/
        currentState = State.Travel;
        RaycastHit hit;
        if (Physics.Raycast(transform.position + horizontalTravelDirection * velocityMagnitude * Time.deltaTime, -transform.up, out hit, checkGroundDownDistance))
        {
            currentSlopeDirection = Vector3.ProjectOnPlane(-transform.up, hit.normal);
            Vector3 flatSlopeDirection = new Vector3(currentSlopeDirection.x, 0f, currentSlopeDirection.z);
            currentSlopeAngle = Vector3.Angle(currentSlopeDirection, flatSlopeDirection);

            horizontalTravelDirection = hit.point - transform.position;
            horizontalTravelDirection.y = 0f;
            horizontalTravelDirection.Normalize();

            faceTowardsTravelDirectionAmount = Vector3.Dot(boardDirection, horizontalTravelDirection);
            isBreaking = Mathf.Abs(faceTowardsTravelDirectionAmount) < brakeThreshold;

            faceTowardSlopeAmount = Vector3.Dot(boardDirection.normalized, flatSlopeDirection.normalized);
        }

        horizontalTravelDirection = boardDirection;// * forwards/backwards inverted special variable 
        velocityMagnitude = 2f;
    }

    private void EnterStationaryState()
    {
/*        print("Entered: " + currentState);*/
        currentState = State.Stationary;
    }
    #endregion State functions

    #region State checks
    private void CheckGround()
    {
        bool tmpLanded = Physics.Raycast(transform.position, -transform.up, checkGroundDownDistance);

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
        if (Physics.Raycast(transform.position + horizontalTravelDirection * velocityMagnitude * Time.deltaTime, -transform.up, out hit, checkGroundDownDistance))
        {
            currentSlopeDirection = Vector3.ProjectOnPlane(-transform.up, hit.normal);
        }
    }

    private void BroadcastLeftGround()
    {
        
    }
    #endregion Event broadcasts

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + horizontalTravelDirection);
        Gizmos.DrawWireSphere(transform.position + horizontalTravelDirection, .4f);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + currentSlopeDirection * 5f);
        Gizmos.DrawWireSphere(transform.position + currentSlopeDirection * 5f, .3f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + boardDirection * 5f);
        Gizmos.DrawWireSphere(transform.position + boardDirection * 5f, .3f);
    }
}
