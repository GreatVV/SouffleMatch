using System.IO;
using Assets.Game.Gameplay;
using UnityEditor;
using UnityEngine;

public class EditorExtension
{
    [MenuItem("Custom/Clear Player Prefs")]
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Custom/Create Tile Factory")]
    public static void CreateTileFactory()
    {
        CreateAssetFile<TilesFactory>();
    }

    public static void CreateAssetFile<T>() where T : ScriptableObject, new()
    {
        string path = string.Format("Assets/Resources/{0}.asset", typeof(T).Name);
        if (Selection.activeObject)
        {
            string selectionPath = AssetDatabase.GetAssetPath(Selection.activeObject); // relative path
            if (Directory.Exists(selectionPath))
            {
                path = Path.Combine(selectionPath, string.Format("{0}.asset", typeof(T).Name));
            }
        }

        var asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}