#region

using System;
using System.Linq;
using UnityEngine;

#endregion

namespace GamefieldStates
{
    [Serializable]
    public class CreateNewChuzzlesState : GamefieldState
    {
        #region Event Handlers
        void OnDestroy()
        {
            TilesCollection.AnimationFinished -= OnAnimationFinished;
        }
        public override void OnEnter()
        {
            TilesCollection.AnimationFinished += OnAnimationFinished;

            if (CreateNew())
            {
               // Debug.Log("Has new");
                if (!TilesCollection.IsAnyAnimated)
                {
                  //  Debug.Log("No Moving");
                    OnAnimationFinished();
                }
                else
                {
                   // Debug.Log("Moving: "+TilesCollection.AnimatedCount);
                }
            }
        }

        public override void OnExit()
        {
            TilesCollection.AnimationFinished -= OnAnimationFinished;
            if (TilesCollection.IsAnyAnimated)
            {
                Debug.LogError("FUCK YOU FROM CREATE NEW STATE: " + TilesCollection.AnimatedCount);
            }
            Gamefield.Level.Chuzzles.NewTilesInColumns = new int[Gamefield.Level.Cells.Width];
        }

        public void OnAnimationFinished()
        {
            TilesCollection.SyncFromMoveTo();

            var combinations = GamefieldUtility.FindCombinations(Gamefield.Level.Chuzzles);
            if (combinations.Count > 0)
            {
                Gamefield.SwitchStateTo(Gamefield.CheckSpecialState);
            }
            else
            {
                if (!Gamefield.GameMode.IsWin && !Gamefield.GameMode.IsGameOver)
                {
                    Gamefield.SwitchStateTo(Gamefield.FieldState);
                }
                else
                {
                    Gamefield.GameMode.Check();
                }
            }

        }



        #endregion

        public override void UpdateState()
        {
        }

        public override void LateUpdateState()
        {
        }

        public bool CreateNew()
        {
            var hasNew = Gamefield.Level.Chuzzles.NewTilesInColumns.Any(x => x > 0);
            if (!hasNew)
            {
                Gamefield.SwitchStateTo(Gamefield.CheckSpecialState);
                return false;
            }

            //check if need create new tiles
            for (var x = 0; x < Gamefield.Level.Chuzzles.NewTilesInColumns.Length; x++)
            {
                var newInColumn = Gamefield.Level.Chuzzles.NewTilesInColumns[x];
                if (newInColumn > 0)
                {
                    for (var j = 0; j < newInColumn; j++)
                    {
                        //create new tiles
                        var chuzzle = TilesFactory.Instance.CreateChuzzle(Gamefield.Level.Cells.GetCellAt(x, Gamefield.Level.Cells.Height + j));
                        chuzzle.Current.IsTemporary = true;
                    }
                }
            }

            //move tiles to fill positions
            for (var x = 0; x < Gamefield.Level.Chuzzles.NewTilesInColumns.Length; x++)
            {
                var newInColumn = Gamefield.Level.Chuzzles.NewTilesInColumns[x];
                if (newInColumn > 0)
                {
                    for (var y = 0; y < Gamefield.Level.Cells.Height; y++)
                    {
                        var cell = Gamefield.Level.Cells.GetCellAt(x, y, false);
                        if (Gamefield.Level.Chuzzles.GetTileAt(x, y) == null && cell.Type != CellTypes.Block)
                        {
                            while (cell != null)
                            {
                                var chuzzle = Gamefield.Level.Chuzzles.GetTileAt(cell);
                                if (chuzzle != null)
                                {
                                    chuzzle.MoveTo = chuzzle.MoveTo.GetBottomWithType();
                                    //Level.GetCellAt(chuzzle.MoveTo.x, chuzzle.MoveTo.y - 1);                                
                                }
                                cell = cell.Top;
                            }
                        }
                    }
                }
            }

            foreach (var c in TilesCollection)
            {
                if (c.MoveTo.Y != c.Current.Y)
                {   
                    c.AnimateMoveTo(c.MoveTo.Position);
                }
            }
            return true;
        }

  
    }
}