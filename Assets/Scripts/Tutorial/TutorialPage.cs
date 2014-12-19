using System;
using Assets.Game.Gameplay;
using Assets.Game.Gameplay.Chuzzles;
using Assets.Game.Utility;
using Assets.iTweenEditor;
using Assets.UI.Localization;
using UnityEngine;

namespace Assets.Tutorial
{
    internal class TutorialPage : MonoBehaviour
    {
        private Gamefield _gamefield;
        private TutorialCloud _tutorialCloud;

        #region Events

        public event Action<TutorialPage> End;

        protected virtual void FireEnd(TutorialPage page)
        {
            Action<TutorialPage> handler = End;
            if (handler != null) handler(page);
        }

        private void AddHandlers()
        {
            RemoveHandlers();
            _gamefield.Level.Chuzzles.TileDestroyed += OnTileDestroyed;
        }

        private void RemoveHandlers()
        {
            _gamefield.Level.Chuzzles.TileDestroyed -= OnTileDestroyed;
        }

        #endregion

        #region Events Subscribers

        private void OnTileDestroyed(Chuzzle chuzzle)
        {
            FireEnd(this);
            RemoveHandlers();
        }

        #endregion

        #region Unity Methods

        private void OnDestroy()
        {
            RemoveHandlers();
        }

        private void Start()
        {
            _gamefield = FindObjectOfType<Gamefield>();
            _tutorialCloud = FindObjectOfType<TutorialCloud>();
        }

        #endregion

        #region In Inspector

        public TutorialPage NextPage;
        public iTweenEvent finger;

        public IntVector2 fromFingerPosition;
        public IntVector2 toFingerPosition;

        #endregion

        public void Show()
        {
            Debug.Log("Show");
            gameObject.SetActive(true);

            IntVector2 targetPosition;
            Chuzzle arrowChuzzle;
            GamefieldUtility.Tip(_gamefield.Level.Chuzzles, out targetPosition, out arrowChuzzle);
            if (arrowChuzzle && arrowChuzzle.Current != null)
            {
                fromFingerPosition = arrowChuzzle.Current.IntVector2Position;
            }
            toFingerPosition = targetPosition;

            Vector3 fromPosition = GamefieldUtility.ConvertXYToPosition(fromFingerPosition.x, fromFingerPosition.y,
                Chuzzle.Scale);
            finger.transform.position = fromPosition;
            finger.Values["position"] = GamefieldUtility.ConvertXYToPosition(toFingerPosition.x, toFingerPosition.y,
                Chuzzle.Scale);
            finger.Play();

            Tutorial.Instance.targetCell = _gamefield.Level.Cells.GetCellAt(targetPosition);
            Tutorial.Instance.takeableChuzzle = _gamefield.Level.Chuzzles.GetTileAt(fromFingerPosition.x, fromFingerPosition.y);

            AddHandlers();

            _tutorialCloud.SetText(Localization.Get("Tutorial_Drag"));
            _tutorialCloud.SetPosition(Camera.main.WorldToScreenPoint(fromPosition + Vector3.up*0.5f));
            _tutorialCloud.Show();
        }

        public void MakeTutorial(GameObject go, int order)
        {
            //NGUITools.SetLayer(go, LayerMask.NameToLayer("Tutorial"));
            SpriteRenderer[] renderers = go.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer render in renderers)
            {
                render.sortingOrder = order;
            }
        }

        public void Hide()
        {
            RemoveHandlers();
            Debug.Log("Hide");
            gameObject.SetActive(false);
            _tutorialCloud.Hide();
        }
    }
}