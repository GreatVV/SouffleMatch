using UnityEditor;
using UnityEngine;

public class EditorExtension
{
    [MenuItem("Game features/Clear Player Prefs")]
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}