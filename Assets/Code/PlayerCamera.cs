using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform focusTransform;
    public float zOffset = -10;
    public float yOffset = 5f;
    public float targetForwardOffset = 10f;
    public float movementIncrement = .3f;

    public float debugSphereSize = .4f;

    Vector3 focusTargetPosition;
    Vector3 newPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void LateUpdate()
    {
        Vector3 targetOffset = Vector3.right * targetForwardOffset;
        focusTargetPosition = focusTransform.position + targetOffset;
        
        //         Vector3 newPosition = Vector3.MoveTowards(
        //             transform.position,
        //             focusTransform.transform.position + targetOffset,
        //             movementIncrement);

        Vector3 direction = focusTargetPosition - transform.position;

        Vector3 startPosition = transform.position;
        //focusTargetPosition.z = zOffset;

        Debug.DrawLine(transform.position, focusTargetPosition);

        newPosition = focusTargetPosition + direction * movementIncrement * Time.deltaTime;

        newPosition.y += yOffset;
        newPosition.z += zOffset;

        //newPosition.y = yOffset;
        transform.position = newPosition;
        //transform.position = new Vector3(transform.position.x, transform.position.y, zOffset);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(focusTargetPosition, debugSphereSize);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, focusTargetPosition);
    }
}
