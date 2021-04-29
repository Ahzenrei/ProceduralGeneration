using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PointGenerator))]
class PointGeneratorHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
            ((PointGenerator)target).CreatePoint();
        if (GUILayout.Button("Reset"))
            ((PointGenerator)target).Reset();
    }
}
