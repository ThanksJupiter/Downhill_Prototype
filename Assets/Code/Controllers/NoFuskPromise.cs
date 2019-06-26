using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoFuskPromise : MonoBehaviour
{
    public BezierSurface_V2 bz;

    public float speed = 1f;
    public float fallSpeed = 2f;
    public float riseSpeed = 3f;

    Vector3[] vertices;
    Vector3 closestVec;

    float verticalInput;
    float horizontalInput;

    // Start is called before the first frame update
    void Start()
    {
        vertices = bz.vertices;
    }

    // Update is called once per frame
    void Update()
    {
        vertices = bz.vertices;

        float closestDst = 100f;
        for (int i = 0; i < vertices.Length; i++)
        {
            float dst = Vector3.Distance(transform.position, vertices[i]);
            if (dst < closestDst)
            {
                closestDst = dst;
                closestVec = vertices[i];
            }
        }

        Debug.DrawLine(transform.position, closestVec, Color.green);
        SetInput();
        Descend();
        Move();
    }

    void Descend()
    {
        if (transform.position.y > closestVec.y)
        {
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y - fallSpeed * Time.deltaTime,
                transform.position.z
                );
        }

        if (transform.position.y < closestVec.y)
        {
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y + riseSpeed * Time.deltaTime,
                transform.position.z
                );

            return;
        }
    }

    void SetInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
    }

    void Move()
    {
        float spd = speed * Time.deltaTime;
        transform.position = new Vector3(
            transform.position.x + horizontalInput * spd,
            transform.position.y,
            transform.position.z + verticalInput * spd
            );
    }
}
