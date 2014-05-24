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
        public bool isAlreadyChangedState;
        #region Event Handlers

        public override void OnEnter()
        {
            isAlreadyChangedState = false;
            AnimatedChuzzles.Clear();
            Chuzzle.DropEventHandlers();
            Chuzzle.AnimationStarted += OnAnimationStarted;
            CreateNew();
        }

        private void OnAnimationStarted(Chuzzle chuzzle)
        {
            if (isAlreadyChangedState)
            {
                Debug.LogWarning("Already changed state create new chuzzle");
                return;
            }

            if (!AnimatedChuzzles.Contains(chuzzle))
            {
                AnimatedChuzzles.Add(chuzzle);
                chuzzle.AnimationFinished += OnAnimationFinished;
            }
        }

        public override void OnExit()
        {
            if (AnimatedChuzzles.Any())
            {
                Debug.LogError("FUCK YOU FROM CREATE NEW STATE: " + AnimatedChuzzles.Count);
            }
            Gamefield.NewTilesInColumns = new int[Gamefield.Level.Cells.Width];
        }

        public void OnAnimationFinished(Chuzzle chuzzle)
        {
            chuzzle.Real = chuzzle.Current = chuzzle.MoveTo;

            chuzzle.AnimationFinished -= OnAnimationFinished;   
            AnimatedChuzzles.Remove(chuzzle);

            if (isAlreadyChangedState)
            {
                Debug.LogWarning("Finished in CRNC state ");
            }

            if (!AnimatedChuzzles.Any() && !isAlreadyChangedState)
            {
                //Gamefield.Level.UpdateActive();

                var combinations = GamefieldUtility.FindCombinations(Gamefield.Level.Chuzzles.GetTiles());
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
                isAlreadyChangedState = true;
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
            var hasNew = Gamefield.NewTilesInColumns.Any(x => x > 0);
            if (!hasNew)
            {
                Gamefield.SwitchStateTo(Gamefield.CheckSpecialState);
                return false;
            }

            //check if need create new tiles
            for (var x = 0; x < Gamefield.NewTilesInColumns.Length; x++)
            {
                var newInColumn = Gamefield.NewTilesInColumns[x];
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
            for (var x = 0; x < Gamefield.NewTilesInColumns.Length; x++)
            {
                var newInColumn = Gamefield.NewTilesInColumns[x];
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

            foreach (var c in Gamefield.Level.Chuzzles.GetTiles())
            {
                if (c.MoveTo.y != c.Current.y)
                {   
                    c.AnimateMoveTo(c.MoveTo.Position);
                }
            }
            return true;
        }

  
    }
}