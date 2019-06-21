using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public struct Patch
{
    Vector2 id;
    Vector3 centerPos;
    Vector3[] vertices;
    Vector3[] controlPoints;
}

public class SurfaceSystem : MonoBehaviour
{
    public const int RESOLUTION = 8;

    Patch[][] patchGrid;
    Vector3[] currentHandles;


    public void AddPatch(Vector2 id)
    {

    }

    public void RemovePatch(Vector2 id)
    {

    }

    public void ShowHandlesForPatch(Vector2 id)
    {
        HideHandles();


    }

    public void HideHandles()
    {

    }

    public void GenerateMeshFromAllPatches()
    {

    }

    public Vector2 CalculateCoordinatesOnSurface(Vector3 position)
    {
        return Vector2.zero;
    }

    public bool GetPatchNeihbours(Vector2 id, out Vector2[] neighbourPatches)
    {
        neighbourPatches = new Vector2[1];
        return true;
    }

}