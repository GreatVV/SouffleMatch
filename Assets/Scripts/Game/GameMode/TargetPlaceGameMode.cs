using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Game.Gameplay.Cells;
using Assets.Game.Gameplay.Chuzzles;
using Assets.Game.Utility;
using UnityEngine;

namespace Assets.Game.GameMode
{
    [Serializable]
    public class TargetPlaceGameMode : GameMode
    {
        public List<IntVector2> CurrentPlaceCoordinates = new List<IntVector2>();
        public List<IntVector2> PlaceCoordinates = new List<IntVector2>();

        public TargetPlaceGameMode(GameModeDescription description) : base(description)
        {
        }

        public override void OnDestroy()
        {
        }

        private void OnTileDestroyed(Chuzzle destroyedChuzzle)
        {
            if (PlaceCoordinates.Count == 0 || destroyedChuzzle.IsReplacingOnDeath ||
                !GamefieldUtility.IsOrdinaryDestroyable(destroyedChuzzle))
            {
                return;
            }


            IntVector2 place =
                CurrentPlaceCoordinates.FirstOrDefault(
                    x => x.x == destroyedChuzzle.Current.X && x.y == destroyedChuzzle.Current.Y);
            if (place != null)
            {
                destroyedChuzzle.Current.IsPlace = false;
                CurrentPlaceCoordinates.Remove(place);
            }

            if (CurrentPlaceCoordinates.Count == 0)
            {
                IsWin = true;
            }
        }

        public override void HumanTurn()
        {
            SpendTurn();
        }

        protected override void OnInit()
        {
            Gamefield.Level.Chuzzles.TileDestroyed += OnTileDestroyed;
            Gamefield.Level.Chuzzles.TileDestroyed += OnTileDestroyed;

            PlaceCoordinates.Clear();
            IEnumerable<Cell> placeCell = Gamefield.Level.Cells.GetCells(x => x.CreationType == CreationType.Place);
            Cell[] enumerable = placeCell as Cell[] ?? placeCell.ToArray();
            Debug.Log("Now of cells: " + enumerable.Count());
            foreach (Cell cell in enumerable)
            {
                PlaceCoordinates.Add(new IntVector2(cell.X, cell.Y));
            }
            OnReset();
        }

        public override void OnReset()
        {
            Gamefield.Level.Chuzzles.TileDestroyed -= OnTileDestroyed;
            CurrentPlaceCoordinates.Clear();
            CurrentPlaceCoordinates.AddRange(PlaceCoordinates);
        }

        public override string ToString()
        {
            return string.Format("You should clear all {0} cells", PlaceCoordinates.Count);
        }
    }
}