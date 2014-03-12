using System;
using System.Linq;
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

        public UI2DSprite blackBg;

        #region In Inspector

        public TutorialPage NextPage;

        public int[] blockedY;
        public int[] blockedX;

        public Position fromFingerPosition;
        public Position toFingerPosition;

        public iTweenEvent finger;
        [SerializeField]
        private Gamefield gamefield;

        #endregion

        void Awake()
        {
            
        }

        public void Show()
        {
            blackBg.gameObject.SetActive(true);
            Tutorial.instance.blockedY = blockedY;
            Tutorial.instance.blockedX = blockedX;

            finger.transform.position = GamefieldUtility.ConvertXYToPosition((int) fromFingerPosition.x, (int) fromFingerPosition.y, Chuzzle.Scale);
            finger.Values["position"] = GamefieldUtility.ConvertXYToPosition((int)toFingerPosition.x, (int)toFingerPosition.y, Chuzzle.Scale);

            var chuzzles = Gamefield.Chuzzles.Where(x=>Tutorial.instance.CantMoveThisChuzzle(x));
            foreach (var chuzzle in chuzzles)
            {
                MakeTutorial(chuzzle.gameObject);
            }

            var cells = gamefield.Level.Cells.Where(c=>Tutorial.instance.CantMoveThisCell(c));
            foreach (var cell in cells)
            {
                MakeTutorial(cell.Sprite.gameObject);
            }

        }

        public void MakeTutorial(GameObject go)
        {
           NGUITools.SetLayer(go, LayerMask.NameToLayer("Tutorial"));
        }

        public void Hide()
        {
            blackBg.gameObject.SetActive(false);
            Tutorial.instance.blockedY = new int[0];
            Tutorial.instance.blockedX = new int[0];
        }
    }

}