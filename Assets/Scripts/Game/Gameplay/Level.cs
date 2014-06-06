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


[Serializable]
public class Level : IJsonSerializable
{   
    public TilesCollection Chuzzles = new TilesCollection();
    public CellCollection Cells = new CellCollection();
   
    public FieldDescription InitDescription;
    public Gamefield Gamefield;

    public void InitFromFile(FieldDescription fieldDescription)
    {
        InitDescription = fieldDescription;

        Cells.root = Gamefield.transform;

        Chuzzles.DestroyChuzzles();
        Cells.DestroyCells();   
        Cells.Init(fieldDescription);

        Chuzzles.InitInCells(Cells);

        Chuzzles.NewTilesInColumns = new int[fieldDescription.Width];
        Random.seed = fieldDescription.Seed == -1 ? Environment.TickCount : fieldDescription.Seed;
        TilesFactory.Instance.NumberOfColors = fieldDescription.NumberOfColors;
    }

    public JSONObject Serialize()
    {
        //TODO make save
        throw new NotImplementedException();
    }

    public void Deserialize(JSONObject json)
    {
        //Restore from save
        throw new NotImplementedException();
    }
}