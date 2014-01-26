using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class FlyingPoints : MonoBehaviour
{

    public Gamefield Gamefield;
    public Camera UiCamera;

    public FlyingLabelPool Pool;

	// Use this for initialization
	void Start ()
	{
	    Gamefield.PointSystem.PointsForDestroy += OnPointsForDestroy;
	}

    private void OnPointsForDestroy(IEnumerable<Chuzzle> comb, int points)
    {
        var label = Pool.GetLabel;
        var minx = comb.Min(x => x.Current.Position.x);
        var maxx = comb.Max(x => x.Current.Position.x);
        var miny = comb.Min(x => x.Current.Position.y);
        var maxy = comb.Max(x => x.Current.Position.y);
        var center = new Vector2((minx + maxx) / 2f, (miny + maxy) / 2f);
        label.transform.position = UiCamera.ScreenPointToRay(Camera.main.WorldToScreenPoint(center)).GetPoint(0);
        label.text = string.Format("+{0}", points);
        label.gameObject.SetActive(true);
        label.gameObject.GetComponent<TweenScale>().Play(true);
        label.gameObject.GetComponent<TweenAlpha>().Play(true);
    }

    void OnDestroy()
    {
        Gamefield.PointSystem.PointsForDestroy -= OnPointsForDestroy;
    }
}