using System;
using System.Collections.Generic;
using System.Linq;
using Game.GameMode;
using Game.Gameplay;
using Game.Gameplay.Chuzzles;
using Game.Player;
using Plugins;
using UnityEngine;

namespace UI
{
    public class FlyPointsManager : MonoBehaviour
    {
        public Transform GoalTarget;
        public Transform TotalManaTarget;

        public float flyTime = 1f;

        [SerializeField] private ManaManager _manaManagerSystem = null;
        [SerializeField] private List<Transform> _targets = new List<Transform>();
        [SerializeField] private Gamefield gamefield = null;

        public Canvas Canvas;

        public FlyingLabelPool Pool;

        void Awake ()
        {
            gamefield.GameStarted += OnGameStarted;
            _manaManagerSystem.PointsForDestroy += OnManaManagersForDestroy;
        }

        private void OnManaManagersForDestroy(IEnumerable<Chuzzle> comb, int points)
        {
            if (!comb.Any())
            {
                return;
            }

            foreach (var target in _targets)
            {
                var label = Pool.GetLabel;
                /*var minx = comb.Min(x => x.Current.Position.x);
            var maxx = comb.Max(x => x.Current.Position.x);
            var miny = comb.Min(x => x.Current.Position.y);
            var maxy = comb.Max(x => x.Current.Position.y);
            var center = new Vector2((minx + maxx)/2f, (miny + maxy)/2f);
            label.transform.position = UiCamera.ScreenPointToRay(Camera.main.WorldToScreenPoint(center)).GetPoint(0);
            */
                var rectTransform = label.GetComponent<RectTransform>();
                var screenPos = Camera.main.WorldToScreenPoint(comb.First().transform.position);
                //   var size = rectTransform.anchorMax - rectTransform.anchorMin;
                //label.transform.position = Camera.main.WorldToScreenPoint(comb.First().transform.position);
                //rectTransform.anchorMin = new Vector2(screenPos.x/Screen.width, screenPos.y/Screen.height);
                //rectTransform.anchorMax = rectTransform.anchorMin + size;
                rectTransform.position = screenPos;
                rectTransform.sizeDelta = Vector2.zero;
                label.gameObject.SetActive(true);
                label.Init(points);
                iTween.MoveTo(label.gameObject,
                              iTween.Hash(
                                          "x", target.position.x,
                                          "y", target.position.y,
                                          "z", target.position.z,
                                          "time", flyTime,
                                          "easetype", iTween.EaseType.easeInOutQuad,
                                          "oncomplete", new Action<object>(Pool.Return),
                                          "oncompleteparams", label.gameObject
                                  ));
            }
        }

        void OnDestroy()
        {
            if (gamefield)
            {
                gamefield.GameStarted -= OnGameStarted;
            }

            if (_manaManagerSystem)
            {
                _manaManagerSystem.PointsForDestroy -= OnManaManagersForDestroy;
            }
        }

        private void OnGameStarted(Gamefield field)
        {
            _targets.Clear();

            _targets.Add(TotalManaTarget);

            if (field.GameMode is TargetScoreGameMode)
            {
                _targets.Add(GoalTarget);
            }
        }
    }
}
