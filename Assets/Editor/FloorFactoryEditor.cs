using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tower;
using Tower.Floors;
using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(FloorFactory))]
public class FloorFactoryEditor : Editor{
    public override void OnInspectorGUI()
    {
        var floorFactory = target as FloorFactory;

        if (GUILayout.Button("Popuplate"))
        {
            IEnumerable<Type> types =
           Assembly.GetAssembly(typeof(IFloorDesc)).GetTypes().Where(x => typeof(IFloorDesc).IsAssignableFrom(x));
            Debug.Log("Count: "+types.Count());
            foreach (var type in types)
            {
                floorFactory.AddPrefabForType(type.FullName, null);
            }
        }
        
        base.OnInspectorGUI();
    }
}
