#region

using System;
using System.Collections.Generic;
using System.Linq;
using Game.Gameplay.Chuzzles;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

#endregion

namespace Game.Gameplay.GamefieldStates
{
    [Serializable]
    public class PowerUpAnalyzeState : GameState
    {   

        #region Event Handlers

        public PowerUpAnalyzeState(Gamefield gamefield) : base(gamefield)
        {
        }

        public override void OnEnter()
        {
            Gamefield.SwitchStateTo(Gamefield.RemoveState);
            return;/*
            var combinations = GamefieldUtility.FindCombinations(TilesCollection);
            if (!CheckForSpecial(combinations))
            {
                Gamefield.SwitchStateTo(Gamefield.RemoveState);
            }*/
        }

        public override void OnExit()
        {
            if (TilesCollection.IsAnyAnimated)
            {
                Debug.LogError("FUCK YOU: " + TilesCollection.AnimatedCount);
            }
        }

        #endregion

        public override void UpdateState()
        {
        }

        public override void LateUpdateState()
        {
        }

        public bool CheckForSpecial(List<List<Chuzzle>> combinations)
        {
            var isNewSpecial = false;
            /*
        foreach (var comb in combinations)
        {
            //if any tile is powerup - then don't check for new bonuses
            //or any tile has counter
            if (comb.Any(x => !(GamefieldUtility.IsUsual(x))))
            {
                continue;
            }

            if (comb.Count == 4)
            {
                isNewSpecial = CreateLine(comb);
            }
            else
            {
                if (comb.Count >= 5)
                {
                    isNewSpecial = CreateBomb(comb);
                }
            }
        }*/

            return isNewSpecial;
        }

        private bool CreateBomb(List<Chuzzle> comb)
        {
            return CreateSpecialWithType(comb, Instance.TilesFactory.BombChuzzlePrefabs);
        }

        public bool CreateSpecialWithType(List<Chuzzle> ordered, GameObject[] prefabs)
        {

            var targetTile = ordered[Random.Range(0, ordered.Count)];
            foreach (var chuzzle in ordered)
            {
                if (chuzzle != targetTile)
                {
                    ChuzzleMover.Instance.MoveTo(chuzzle, chuzzle.transform.position, targetTile.Current.Position);
                }
            }                               

            var powerUp = prefabs.First(x => x.GetComponent<Chuzzle>().Color == targetTile.Color);
            Instance.TilesFactory.CreateChuzzle(targetTile.Current, powerUp);
            targetTile.Destroy(false, false);
            ordered.Remove(targetTile);
            return true;
        }

        private bool CreateLine(List<Chuzzle> comb)
        {
            return CreateSpecialWithType(comb,
                Random.Range(0, 100) > 50 ? Instance.TilesFactory.HorizontalLineChuzzlePrefabs : Instance.TilesFactory.VerticalLineChuzzlePrefabs);
        }
    }
}