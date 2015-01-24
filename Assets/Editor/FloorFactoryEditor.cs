using System;
using Tower;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (FloorFactory))]
public class FloorFactoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var floorFactory = target as FloorFactory;

        if (GUILayout.Button("Populate"))
        {
            var types = Enum.GetValues(typeof (FloorType));
            Debug.Log("Count: " + types.Length);
            foreach (var type in types)
            {
                floorFactory.AddPrefabForType(type.ToString(), null);
            }
        }

        base.OnInspectorGUI();
    }
}