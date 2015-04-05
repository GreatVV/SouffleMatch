using UnityEngine;
using System.Collections;
using Tower;
using UnityEditor;

[CustomEditor(typeof(FloorDrawer))]
public class FloorDrawerEditor : Editor {

    void OnSceneGUI()
    {
        var floorDrawer = target as FloorDrawer;
        var bottomPosition = floorDrawer.transform.position + floorDrawer.BottomAnchor;
        Handles.color = Color.red;
        Handles.DrawSolidDisc(bottomPosition, new Vector3(0, 0, 1), 0.25f);
        floorDrawer.BottomAnchor = Handles.PositionHandle(bottomPosition, Quaternion.identity) - floorDrawer.transform.position;

        var upperPosition = floorDrawer.transform.position + floorDrawer.UpperAnchor;
        Handles.color = Color.green;
        Handles.DrawSolidDisc(upperPosition, new Vector3(0,0,1), 0.25f);
        floorDrawer.UpperAnchor = Handles.PositionHandle(upperPosition, Quaternion.identity) - floorDrawer.transform.position;
        if (GUI.changed)
        {
            EditorUtility.SetDirty (target);
        }
		
    }
}
