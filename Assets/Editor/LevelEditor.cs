using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Game;
using Game.Data;
using Game.GameMode;
using UnityEditor;
using UnityEngine;

public class LevelEditor : EditorWindow
{
    public LevelPackManager LevelPackManager;
    private readonly Dictionary<string, bool> _foldouts = new Dictionary<string, bool>();

    #region Event Handlers

    private void OnGUI()
    {

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load pack"))
        {
            var filepath = EditorUtility.OpenFilePanel("Open packed levels", "", ".json");
            var serialized = File.ReadAllText(filepath);
            LevelPackManager = LevelPackManager.Deserialize(serialized);
        }

        if (GUILayout.Button("Generate new"))
        {
            LevelPackManager = new LevelPackManager();
        }
        EditorGUILayout.EndHorizontal();

        if (LevelPackManager != null)
        {
            GUILayout.Label("Loaded packs: "+LevelPackManager.Packs.Count);

            for (int index = 0; index < LevelPackManager.Packs.Count; index++)
            {
                var levelPack = LevelPackManager.Packs[index];
                DrawLevelPack(levelPack);
                if (!LevelPackManager.Packs.Contains(levelPack))
                {
                    index--;
                }
            }

            if (GUILayout.Button("Add pack"))
            {
                LevelPackManager.Packs.Add(new LevelPack() {Name = "New pack"});
            }

            if (GUILayout.Button("Save"))
            {
                var savePath = EditorUtility.SaveFilePanel("", "", "levels", "json");
                File.WriteAllText(savePath, LevelPackManager.Serialize().ToString());
            }
        }
    }

    private void DrawLevelPack(LevelPack levelPack)
    {
        var fo = LevelPackManager.Packs.IndexOf(levelPack).ToString(CultureInfo.InvariantCulture);
        if (!_foldouts.ContainsKey(fo))
        {
            _foldouts[fo] = false;
        }

        _foldouts[fo] = EditorGUILayout.Foldout(_foldouts[fo], levelPack.Name);
        if (_foldouts[fo])
        {
            EditorGUILayout.BeginHorizontal();
            levelPack.Name = EditorGUILayout.TextField("Name", levelPack.Name);
            if (GUILayout.Button("Remove"))
            {
                LevelPackManager.Packs.Remove(levelPack);
            }
            EditorGUILayout.EndHorizontal();

            foreach (var levelDescription in levelPack.LoadedLevels)
            {
                DrawLevel(levelPack, levelDescription);
            }

            if (GUILayout.Button("Add level"))
            {
                levelPack.LoadedLevels.Add(new LevelDescription());
            }
        }
    }

    private void DrawLevel(LevelPack levelPack, LevelDescription levelDescription)
    {
        var fo = LevelPackManager.Packs.IndexOf(levelPack) + "_" + levelPack.LoadedLevels.IndexOf(levelDescription);
        if (!_foldouts.ContainsKey(fo))
        {
            _foldouts[fo] = false;
        }

        _foldouts[fo] = EditorGUILayout.Foldout(_foldouts[fo], levelDescription.Name);
        if (_foldouts[fo])
        {
            EditorGUI.indentLevel++;
            levelDescription.Name = EditorGUILayout.TextField("Name", levelDescription.Name);
            DrawCondition(levelDescription.Condition);
            DrawFieldDescription(levelDescription.Field);
            if (GUILayout.Button("Generate"))
            {
                var cells = FindObjectsOfType<Cell>();
                for (int index = 0; index < cells.Length; index++)
                {
                    var cell = cells[index];
                    DestroyImmediate(cell);
                }

                var gamefield = FindObjectOfType<Gamefield>();
                gamefield.LevelDescription = levelDescription;
                gamefield.Init();
            }
            EditorGUI.indentLevel--;
        }
    }

    private void DrawFieldDescription(FieldDescription field)
    {
        EditorGUILayout.BeginHorizontal();
        field.Width = EditorGUILayout.IntSlider("Width", field.Width, 6,8);
        field.Height = EditorGUILayout.IntSlider("Height", field.Height, 6,10);
        field.Seed = EditorGUILayout.IntField("Seed", field.Seed);
        field.NumberOfColors = EditorGUILayout.IntSlider("Number of colors",field.NumberOfColors, 6, 8);
        EditorGUILayout.EndHorizontal();
        for (int index = 0; index < field.SpecialCells.Count; index++)
        {
            var cell = field.SpecialCells[index];
            EditorGUILayout.BeginHorizontal();
            cell.Type = (CellTypes) EditorGUILayout.EnumPopup("Type", cell.Type);
            cell.X = EditorGUILayout.IntSlider("x", cell.X, 0, field.Width-1);
            cell.Y = EditorGUILayout.IntSlider("y", cell.Y, 0, field.Height-1);
            if (GUILayout.Button("X"))
            {
                field.SpecialCells.Remove(cell);
                index--;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add cell"))
        {
            field.SpecialCells.Add(new CellDescription());
        }
    }

    private void DrawCondition(ConditionDescription condition)
    {
        
        EditorGUILayout.BeginHorizontal();
        condition.Star1Score = EditorGUILayout.IntField("Star 1", condition.Star1Score);
        condition.Star2Score = EditorGUILayout.IntField("Star 2", condition.Star2Score);
        condition.Star3Score = EditorGUILayout.IntField("Star 3", condition.Star3Score);
        EditorGUILayout.EndHorizontal();

        DrawConditionGameMode(condition.GameMode);
    }

    private void DrawConditionGameMode(GameModeDescription gameMode)
    {
        EditorGUILayout.BeginHorizontal();
        gameMode.Mode = (GameModes) EditorGUILayout.EnumPopup("Mode", gameMode.Mode);
        gameMode.Turns = EditorGUILayout.IntField("Turns", gameMode.Turns);
        switch (gameMode.Mode)
        {
            case GameModes.TargetScore:
                gameMode.TargetScore = EditorGUILayout.IntField("Score", gameMode.TargetScore);
                break;
            case GameModes.TargetPlace:
                break;
            case GameModes.TargetChuzzle:
                gameMode.Amount = EditorGUILayout.IntField("Amount", gameMode.TargetScore);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        EditorGUILayout.EndHorizontal();
    }

    #endregion

    [MenuItem("Window/Level editor #]")]
    private static void Init()
    {
        var window = GetWindow<LevelEditor>();
        window.Show();
    }
}

