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
            AnimatedChuzzles.Clear();
            Chuzzle.DropEventHandlers();
            Chuzzle.AnimationStarted += OnAnimationStarted;

            var anyCombination = GamefieldUtility.FindOnlyOneCombination(Gamefield.Chuzzles);
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
            if (AnimatedChuzzles.Any())
            {
                Debug.LogError("FUCK YOU FROM REMOVE COMBINATION: "+AnimatedChuzzles.Count);
            }

            PowerUpDestroyManager.Instance.IsInDestroyState = false;
        }

        public void OnAnimationFinished(Chuzzle chuzzle)
        {
            chuzzle.AnimationFinished -= OnAnimationFinished;
            AnimatedChuzzles.Remove(chuzzle);
            if (!AnimatedChuzzles.Any())
            {
                Gamefield.SwitchStateTo(Gamefield.CreateNewChuzzlesState);
            }
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
            var powerUpCombination = GamefieldUtility.FindOnlyOneCombinationWithCondition(Gamefield.Chuzzles,
                GamefieldUtility.IsPowerUp);

            //if has any powerups
            if (powerUpCombination.Any())
            {
                //destroy step by step
                PowerUpDestroyManager.Instance.Destroy(powerUpCombination);

                if (!AnimatedChuzzles.Any())
                {
                    Gamefield.SwitchStateTo(Gamefield.CreateNewChuzzlesState);
                }
            }
            else
            {

                var combinations = GamefieldUtility.FindCombinations(global::Gamefield.Chuzzles);
                //remove combinations
                foreach (var combination in combinations)
                {
                    Gamefield.InvokeCombinationDestroyed(combination);

                    foreach (var chuzzle in combination)
                    {
                        chuzzle.Destroy(true);
                    }

                    if (!AnimatedChuzzles.Any())
                    {
                        Gamefield.SwitchStateTo(Gamefield.CreateNewChuzzlesState);
                    }
                    yield return new WaitForSeconds(0.05f);
                }
            }

            yield return new WaitForEndOfFrame();
        }

        private void OnAnimationStarted(Chuzzle chuzzle)
        {
            if (!AnimatedChuzzles.Contains(chuzzle))
            {
                AnimatedChuzzles.Add(chuzzle);
                chuzzle.AnimationFinished += OnAnimationFinished;
            }
        }

    }
}