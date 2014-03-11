using System;
using UnityEngine;

namespace TutorialSpace 
{
    internal class TutorialPage : MonoBehaviour
    {
        [Serializable]
        public class Position
        {
            public int x;
            public int y;
        }

        public event Action<TutorialPage> End;

        protected virtual void FireEnd(TutorialPage page)
        {
            var handler = End;
            if (handler != null) handler(page);
        }

        #region In Inspector

        public TutorialPage NextPage;

        public int[] blockedRows;
        public int[] blockedColumns;

        public Position fromFingerPosition;
        public Position toFingerPosition;

        public iTweenEvent finger;

        #endregion

        void Awake()
        {
            
        }

        public void Show()
        {
            Tutorial.blockedRows = blockedRows;
            Tutorial.blockedColumns = blockedColumns;

            finger.transform.position = GamefieldUtility.ConvertXYToPosition((int) fromFingerPosition.x, (int) fromFingerPosition.y, Chuzzle.Scale);
            finger.Values["position"] = GamefieldUtility.ConvertXYToPosition((int)toFingerPosition.x, (int)toFingerPosition.y, Chuzzle.Scale);
        }

        public void Hide()
        {

        }


    }

}