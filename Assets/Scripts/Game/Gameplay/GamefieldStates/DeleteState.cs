#region

using System;
using System.Collections;
using System.Linq;
using Game.Utility;
using UnityEngine;

#endregion

namespace Game.Gameplay.GamefieldStates
{
    [Serializable]
    public class DeleteState : GameState
    {
        public DeleteState(Gamefield gamefield) : base(gamefield)
        {
        }

        void OnDestroy()
        {
            TilesCollection.AnimationFinished -= OnAnimationFinished;
        }


        #region Event Handlers

        public override void OnEnter()
        {
            TilesCollection.AnimationFinished += OnAnimationFinished;

            var anyCombination = GamefieldUtility.FindOnlyOneCombination(TilesCollection);
            if (anyCombination.Any())
            {   
                Gamefield.StartCoroutine(RemoveCombinations());
            }
            else
            {
                Gamefield.SwitchStateTo(Gamefield.FieldState);
            }
        }

        public override void OnExit()
        {
            TilesCollection.AnimationFinished -= OnAnimationFinished;
            if (TilesCollection.IsAnyAnimated)
            {
                Debug.LogError("FUCK YOU FROM REMOVE COMBINATION: "+TilesCollection.AnimatedCount);
            }
        }

        public void OnAnimationFinished()
        {
            Gamefield.SwitchStateTo(Gamefield.CreateState);
        }

        #endregion

        public override void UpdateState()
        {
        }

        public override void LateUpdateState()
        {
        }

        private IEnumerator RemoveCombinations()
        {
           // Debug.Log("Start remove combinations");
            var powerUpCombination = GamefieldUtility.FindOnlyOneCombinationWithCondition(TilesCollection,GamefieldUtility.IsPowerUp);

            //if has any powerups
            if (powerUpCombination.Any())
            {
                //destroy step by step
                //PowerUpDestroyManager.Instance.Destroy(powerUpCombination);

                if (!TilesCollection.IsAnyAnimated)
                {
                    Gamefield.SwitchStateTo(Gamefield.CreateState);
                }
            }
            else
            {

                var combinations = GamefieldUtility.FindCombinations(TilesCollection);
                //remove combinations
                Debug.Log("Combination destroyed: "+combinations.Count);
                foreach (var combination in combinations)
                {
                    Gamefield.InvokeCombinationDestroyed(combination);

                    foreach (var chuzzle in combination)
                    {
                        chuzzle.Destroy(true);
                    }

                    if (!TilesCollection.IsAnyAnimated)
                    {
                        Gamefield.SwitchStateTo(Gamefield.CreateState);
                    }
                    yield return new WaitForSeconds(0.05f);
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }
}