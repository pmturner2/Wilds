using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LevelGenerator generator = (LevelGenerator) target;

        if (GUILayout.Button("Generate Level"))
        {
            generator.GenerateLevel(true);
        }
    }
}