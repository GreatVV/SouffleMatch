#region

using System;
using System.Collections.Generic;
using System.Linq;
using Game.Gameplay.Cells;
using Game.Gameplay.Chuzzles;
using Game.Utility;
using UniRx;
using UnityEngine;
using Utils;

#endregion

namespace Game.Gameplay.GamefieldStates
{
    [Serializable]
    public class CreateState : GameState
    {
        #region Event Handlers

        public CreateState(Gamefield gamefield) : base(gamefield)
        {}

        public override void OnEnter()
        {
            if (HasNew())
            {
                CreateNew();
            }
            else
            {
                Gamefield.SwitchStateTo(Gamefield.PowerUpAnalyzeState);
            }
        }

        public override void OnExit()
        {
            if (TilesCollection.IsAnyAnimated)
            {
                Debug.LogError("FUCK YOU FROM CREATE NEW STATE: " + TilesCollection.AnimatedCount);
            }
            Gamefield.Level.Chuzzles.NewTilesInColumns = new int[Gamefield.Level.Cells.Width];
        }

        public void FinishHim()
        {
            TilesCollection.SyncFromMoveTo();

            List<List<Chuzzle>> combinations = GamefieldUtility.FindCombinations(Gamefield.Level.Chuzzles);
            if (combinations.Count > 0)
            {
                Gamefield.SwitchStateTo(Gamefield.PowerUpAnalyzeState);
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
        {}

        public override void LateUpdateState()
        {}

        public bool HasNew()
        {
            return Gamefield.Level.Chuzzles.NewTilesInColumns.Any(x => x > 0);
        }

        public void CreateNew()
        {
            //check if need create new tiles
            for (int x = 0; x < Gamefield.Level.Chuzzles.NewTilesInColumns.Length; x++)
            {
                int newInColumn = Gamefield.Level.Chuzzles.NewTilesInColumns[x];
                if (newInColumn > 0)
                {
                    for (int j = 0; j < newInColumn; j++)
                    {
                        //create new tiles
                        Chuzzle chuzzle =
                            Instance.TilesFactory.CreateChuzzle(
                                                                Gamefield.Level.Cells.GetCellAt(
                                                                                                x,
                                                                                                Gamefield.Level.Cells
                                                                                                         .Height + j));
                        chuzzle.Current.IsTemporary = true;
                    }
                }
            }

            //move tiles to fill positions
            for (int x = 0; x < Gamefield.Level.Chuzzles.NewTilesInColumns.Length; x++)
            {
                int newInColumn = Gamefield.Level.Chuzzles.NewTilesInColumns[x];
                if (newInColumn > 0)
                {
                    for (int y = 0; y < Gamefield.Level.Cells.Height; y++)
                    {
                        Cell cell = Gamefield.Level.Cells.GetCellAt(x, y, false);
                        if (Gamefield.Level.Chuzzles.GetTileAt(x, y) == null && cell.Type != CellTypes.Block)
                        {
                            while (cell != null)
                            {
                                Chuzzle chuzzle = Gamefield.Level.Chuzzles.GetTileAt(cell);
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

            foreach (Chuzzle c in TilesCollection.Where(c => c.MoveTo.Y != c.Current.Y))
            {
                ChuzzleMover.Instance.MoveTo(c, c.transform.position, c.MoveTo.Position);
            }

            Observable.EveryUpdate().First(_ => !TilesCollection.IsAnyAnimated).Subscribe(_ => FinishHim());
        }
    }
}