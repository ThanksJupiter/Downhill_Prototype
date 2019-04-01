using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public JupiTestController target;
    public Vector3 offset;
    public float yOffset;
    public float zOffset;

    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = target.transform.position;
        newPos.z -= zOffset;
        newPos.y += yOffset;
        newPos -= target.travelDirection * 10f;
        transform.LookAt(target.transform.position + target.travelDirection);
        transform.position = newPos;
    }
}
