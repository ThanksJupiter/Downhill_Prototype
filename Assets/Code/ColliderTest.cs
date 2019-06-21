using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTest : MonoBehaviour
{

    public BezierSurface_V2 currentPatch;
    public Vector3[] currentPatchVerts;

    public float radius = 0.5f;

    void Start()
    {
        if (currentPatch)
        {
            currentPatchVerts = currentPatch.vertices;
        }
    }

    void Update()
    {
        foreach (Vector3 vert in currentPatchVerts)
        {
            if ((transform.position - vert).magnitude < radius)
            {
                transform.position = vert + (transform.position - vert).normalized * radius;
            }
        }
    }
}
