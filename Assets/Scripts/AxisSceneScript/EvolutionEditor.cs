using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AxisManager))]
public class EvolutionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AxisManager axisManager = (AxisManager)target;

        if (DrawDefaultInspector())
        {

        }

        if (GUILayout.Button("Generate"))
        {
            axisManager.Generate();
        }
    }
}
