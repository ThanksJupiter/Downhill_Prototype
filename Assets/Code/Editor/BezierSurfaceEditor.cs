using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierSurface))]
public class BezierSurfaceEditor : Editor
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

    public override void OnInspectorGUI()
    {
        BezierSurface bs = target as BezierSurface;

        base.OnInspectorGUI();

        bool test = false;
        EditorGUI.BeginChangeCheck();

        if (GUILayout.Button("Create Surface"))
        {
            bs.AddBezierSurface(bs.startPosition);
            test = true;
        }

        if (GUILayout.Button("Reset"))
        {
            bs.Reset();
        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(bs, "add bezier surface");
        }
    }

    public void OnSceneGUI()
    {
        BezierSurface bs = target as BezierSurface;

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
