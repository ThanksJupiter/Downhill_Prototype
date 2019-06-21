using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierSurface_V2))]
public class BezierSurface_V2Editor : Editor
{
    private Tool lastTool;
    const string undoMsg = "Undo move beziér vector";

    private void OnEnable()
    {
        lastTool = Tools.current;
        Tools.current = Tool.None;
    }

    private void OnDisable()
    {
        Tools.current = lastTool;
    }

    public void OnSceneGUI()
    {
        BezierSurface_V2 bs = target as BezierSurface_V2;

        if (bs.controlVertices == null)
        {
            return;
        }

        if (!bs.drawHandles)
        {
            return;
        }

        Vector3[] vertices = bs.controlVertices;
        int uResolution = 4;
        int vResolution = 4;


        EditorGUI.BeginChangeCheck();

        for (int i = 0; i < bs.controlVertices.Length; ++i)
        {
            vertices[i] = Handles.PositionHandle(bs.controlVertices[i], Quaternion.identity);
        }

        uResolution = bs.uResolution;
        vResolution = bs.vResolution;

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(bs, undoMsg);
            bs.controlVertices = vertices;
            bs.uResolution = uResolution;
            bs.vResolution = vResolution;
            bs.RecalculateVertices();
        }
    }
}
