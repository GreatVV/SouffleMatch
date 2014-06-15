using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using Game.GameMode;
using UnityEngine;

public class FlyPointsManager : MonoBehaviour
{
    public Transform GoalTarget;
    public Transform TotalManaTarget;

    public float flyTime = 1f;

    [SerializeField] private Points pointSystem = null;
    [SerializeField] private List<Transform> _targets = new List<Transform>();
    [SerializeField] private Gamefield gamefield = null;

    public Camera UiCamera;

    public FlyingLabelPool Pool;

	void Awake ()
	{
	    gamefield.GameStarted += OnGameStarted;
	    pointSystem.PointsForDestroy += OnPointsForDestroy;
	}

    private void OnPointsForDestroy(IEnumerable<Chuzzle> comb, int points)
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
            label.transform.position = UiCamera.ScreenPointToRay(Camera.main.WorldToScreenPoint(comb.First().transform.position)).GetPoint(0);
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

        if (pointSystem)
        {
            pointSystem.PointsForDestroy -= OnPointsForDestroy;
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
