#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace GamefieldStates
{
    [Serializable]
    public class WinRemoveCombinationState : GamefieldState
    {
        #region Event Handlers

        public override void OnEnter()
        {
            AnimatedChuzzles.Clear();
            Chuzzle.DropEventHandlers();
            Chuzzle.AnimationStarted += OnAnimationStarted;

            var powerUpChuzzles = Gamefield.Chuzzles.Where(GamefieldUtility.IsPowerUp).ToArray();

            foreach (var ch in powerUpChuzzles)
            {
                ch.Destroy(true);
            }

            var combinations = GamefieldUtility.FindCombinations(Gamefield.Chuzzles);
            if (combinations.Any())
            {
                RemoveCombinations(combinations);
            }
            else if(!powerUpChuzzles.Any())
            {
                StartCoroutine(GameModeCheck());    
            }
        }

        IEnumerator GameModeCheck()
        {
            yield return new WaitForSeconds(1.5f);
            Gamefield.GameMode.Check();
        }

        public override void OnExit()
        {
            if (AnimatedChuzzles.Any())
            {
                Debug.LogError("FUCK YOU FROM WIN REMOVE: " + AnimatedChuzzles.Count);
            }
        }

        public void OnAnimationFinished(Chuzzle chuzzle)
        {
            chuzzle.AnimationFinished -= OnAnimationFinished;
            AnimatedChuzzles.Remove(chuzzle);
            if (!AnimatedChuzzles.Any())
            {
                Gamefield.SwitchStateTo(Gamefield.WinCreateNewChuzzlesState);
            }
        }

        #endregion

        public override void UpdateState()
        {
        }

        public override void LateUpdateState()
        {
        }

        private void RemoveCombinations(IEnumerable<List<Chuzzle>> combinations)
        {
            //remove combinations
            foreach (var combination in combinations)
            {
                Gamefield.InvokeCombinationDestroyed(combination);

                //count points
                Gamefield.PointSystem.CountForCombinations(combination);

                foreach (var chuzzle in combination)
                {
                    chuzzle.Destroy(true);
                }
            }
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
