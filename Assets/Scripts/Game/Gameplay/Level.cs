#region

using System;
using Game.Data;
using Game.Utility;
using Utils;
using Random = UnityEngine.Random;

#endregion


namespace Game.Gameplay
{
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
            Instance.TilesFactory.NumberOfColors = fieldDescription.NumberOfColors;
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
}