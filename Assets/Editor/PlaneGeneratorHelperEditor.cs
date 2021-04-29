using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlaneGenerator))]
class PlaneGeneratorHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
            ((PlaneGenerator)target).CreatePlane();

        if (GUILayout.Button("Recalculate normal bounds"))
            ((PlaneGenerator)target).RecalculateNormalBounds();
    }
}
