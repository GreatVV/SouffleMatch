#region

using System;
using System.Collections;
using System.Linq;
using UnityEngine;

#endregion

namespace GamefieldStates
{
    [Serializable]
    public class RemoveCombinationState : GamefieldState
    {

        #region Event Handlers

        public override void OnEnter()
        {
            TilesCollection = Gamefield.Level.Chuzzles;
            TilesCollection.AnimationFinished += OnAnimationFinished;

            var anyCombination = GamefieldUtility.FindOnlyOneCombination(TilesCollection);
            if (anyCombination.Any())
            {   
                StartCoroutine(RemoveCombinations());
            }
            else
            {
                Gamefield.SwitchStateTo(Gamefield.FieldState);
            }
        }

        public override void OnExit()
        {
            if (TilesCollection.IsAnyAnimated)
            {
                Debug.LogError("FUCK YOU FROM REMOVE COMBINATION: "+TilesCollection.Count);
            }
        }

        public void OnAnimationFinished()
        {
            Gamefield.SwitchStateTo(Gamefield.CreateNewChuzzlesState);
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
            var powerUpCombination = GamefieldUtility.FindOnlyOneCombinationWithCondition(TilesCollection,GamefieldUtility.IsPowerUp);

            //if has any powerups
            if (powerUpCombination.Any())
            {
                //destroy step by step
                //PowerUpDestroyManager.Instance.Destroy(powerUpCombination);

                if (!TilesCollection.IsAnyAnimated)
                {
                    Gamefield.SwitchStateTo(Gamefield.CreateNewChuzzlesState);
                }
            }
            else
            {

                var combinations = GamefieldUtility.FindCombinations(TilesCollection);
                //remove combinations
                foreach (var combination in combinations)
                {
                    Gamefield.InvokeCombinationDestroyed(combination);

                    foreach (var chuzzle in combination)
                    {
                        chuzzle.Destroy(true);
                    }

                    if (!TilesCollection.IsAnyAnimated)
                    {
                        Gamefield.SwitchStateTo(Gamefield.CreateNewChuzzlesState);
                    }
                    yield return new WaitForSeconds(0.05f);
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }
}