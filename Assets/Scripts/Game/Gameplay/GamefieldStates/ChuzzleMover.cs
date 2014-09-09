using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamefieldStates
{
    public class ChuzzleMover
    {
        public static ChuzzleMover Instance;

        public class MoveDesc
        {
            public Chuzzle Chuzzle;
            public Vector3 From;
            public Vector3 To;

            public static MoveDesc Create(Chuzzle chuzzle, Vector3 @from, Vector3 to)
            {
                return new MoveDesc()
                {
                    Chuzzle = chuzzle,
                    From = @from,
                    To = to
                };
            }
        }

        public List<MoveDesc> Moves = new List<MoveDesc>();

        public void MoveTo(Chuzzle chuzzle, Vector3 from, Vector3 to, float time = 0.3f)
        {
            Moves.Add(MoveDesc.Create(chuzzle, from, to));
            AnimateMoveTo(chuzzle, to, time);
        }

        public void AnimateMoveTo(Chuzzle chuzzle, Vector3 targetPosition, float time = 0.3f)
        {
            if (chuzzle.IsDead)
            {
                return;
            }
            //Debug.Log("Move: "+name+" "+GetInstanceID());

            if (Vector3.Distance(targetPosition, chuzzle.transform.position) > 0.01f)
            {
                if (chuzzle.IsAnimationStarted)
                {
                    return;
                }
                chuzzle.InvokeAnimationStarted();
                iTween.MoveTo(chuzzle.gameObject,
                    iTween.Hash(
                        "x", targetPosition.x,
                        "y", targetPosition.y,
                        "z", targetPosition.z,
                        "time", time,
                        "easetype", iTween.EaseType.easeInOutQuad,
                        "oncomplete", new Action<object>(OnAnimateMoveEnd),
                        "oncompleteparams", chuzzle.gameObject
                        ));
            }
            else
            {
                chuzzle.transform.position = targetPosition;
            }
        }

        private void OnAnimateMoveEnd(object chuzzle)
        {
            ((GameObject) chuzzle).GetComponent<Chuzzle>().InvokeAnimationFinished();
        }
    }
}