#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Game.Data;
using Game.GameMode;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

#endregion



[RequireComponent(typeof(Gamefield))]
public class Level : MonoBehaviour
{   
    [HideInInspector]
    public Gamefield Gamefield;

    public TilesCollection Chuzzles = null;
    public CellCollection Cells = null;
   
    public SerializedLevel Serialized;

    public string LevelName { get { return string.Format(Localization.Get("LevelNumber"), Serialized.Name); } }

    void Awake()
    {
        Gamefield = GetComponent<Gamefield>();
        Chuzzles = FindObjectOfType<TilesCollection>();
        Cells = FindObjectOfType<CellCollection>();

    }

    public void InitFromFile(SerializedLevel level)
    {
        Serialized = level;

        Cells.Clear();
        Cells.Init(level);

        Random.seed = level.Seed == -1 ? Environment.TickCount : level.Seed;
        TilesFactory.Instance.NumberOfColors = level.NumberOfColors;

        Gamefield.GetComponent<Gamefield>().GameMode = GameModeFactory.CreateGameMode(level.GameMode);
        Gamefield.GetComponent<Gamefield>().GameMode.Init(Gamefield.GetComponent<Gamefield>());
    }


    public void OnChuzzleDeath(Chuzzle chuzzle)
    {
        chuzzle.Died -= OnChuzzleDeath;

        //remove chuzzle from game logic
        Gamefield.RemoveChuzzle(chuzzle);
    }

    public void Reset()
    {
        Chuzzles.Clear();
        Cells.Clear();
    }
   

  
}