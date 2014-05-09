using System;
using UnityEngine;

namespace TutorialSpace 
{
    internal class TutorialPage : MonoBehaviour
    {
        public event Action<TutorialPage> End;

        protected virtual void FireEnd(TutorialPage page)
        {
            var handler = End;
            if (handler != null) handler(page);
        }

        #region In Inspector

        public TutorialPage NextPage;


        public IntVector2 fromFingerPosition;
        public IntVector2 toFingerPosition;

        public iTweenEvent finger;
        [SerializeField]
        private Gamefield gamefield;

        public TutorialCloud tutorialCloud;

        #endregion

        void Awake()
        {
            
        }

        public void Show()
        {
            gameObject.SetActive(true);
            
            IntVector2 targetPosition;
            Chuzzle arrowChuzzle;
            GamefieldUtility.Tip(gamefield.Level.ActiveChuzzles, out targetPosition, out arrowChuzzle);
            if (arrowChuzzle && arrowChuzzle.Current != null)
            {
                fromFingerPosition = arrowChuzzle.Current.IntVector2Position;
            }
            toFingerPosition = targetPosition;

            var fromPosition = GamefieldUtility.ConvertXYToPosition((int) fromFingerPosition.x, (int) fromFingerPosition.y, Chuzzle.Scale);
            finger.transform.position = fromPosition;
            finger.Values["position"] = GamefieldUtility.ConvertXYToPosition((int)toFingerPosition.x, (int)toFingerPosition.y, Chuzzle.Scale);
            finger.Play();

            Tutorial.instance.targetCell = gamefield.Level.GetCellAt(targetPosition);
            Tutorial.instance.takeableChuzzle = gamefield.Level.At(fromFingerPosition.x, fromFingerPosition.y);
            gamefield.TileDestroyed += OnTileDestroyed;

            tutorialCloud.SetText("Drag to destroy zombie veggies.\n That's all");
            tutorialCloud.SetPosition(Camera.main.WorldToScreenPoint(fromPosition + Vector3.up * 0.5f));
            tutorialCloud.Show();
        }

        private void OnTileDestroyed(Chuzzle chuzzle)
        {
            FireEnd(this);
            gamefield.TileDestroyed -= OnTileDestroyed;
        }

        public void MakeTutorial(GameObject go, int order)
        {
           //NGUITools.SetLayer(go, LayerMask.NameToLayer("Tutorial"));
            var renderers = go.GetComponentsInChildren<SpriteRenderer>();
            foreach (var render in renderers)
            {
                render.sortingOrder = order;
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            tutorialCloud.Hide();
        }
    }
}