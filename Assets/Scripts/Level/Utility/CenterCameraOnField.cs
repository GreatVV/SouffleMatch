#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

public class CenterCameraOnField : MonoBehaviour
{
    public static CenterCameraOnField Instance;

    public Camera Camera;
    public Vector3 DefaultPosition = new Vector3(240, 400, -10);

    public bool IsTweening;

    #region Event Handlers

    public void CenterCameraOnChuzzles(List<Chuzzle> targetChuzzles, bool instantly)
    {
        var minX = targetChuzzles.Min(x => x.transform.position.x);
        var minY = targetChuzzles.Min(x => x.transform.position.y);
        var maxX = targetChuzzles.Max(x => x.transform.position.x);
        var maxY = targetChuzzles.Max(x => x.transform.position.y);

        var centerPosition = new Vector3((minX + maxX)/2,
            (minY + maxY)/2, DefaultPosition.z);

        if (instantly)
        {
            Camera.transform.position = centerPosition;
            return;
        }

        if (Vector3.Distance(Camera.transform.position, centerPosition) > 0.1f)
        {
            iTween.MoveTo(Camera.gameObject,
                iTween.Hash("x", centerPosition.x, "y", centerPosition.y, "z", centerPosition.z, "time", 2f,
                    "oncomplete", "OnTweenComplete", "oncompletetarget",
                    gameObject, "oncompleteparams", centerPosition));
            IsTweening = true;
        }
        else
        {
            Camera.transform.position = centerPosition;
        }
    }

    private void OnTweenComplete(Vector3 targetPosition)
    {
        IsTweening = false;
    }

    #endregion

    public void Awake()
    {
        Instance = this;
    }

    public void ToDefault()
    {
        Camera.transform.position = DefaultPosition;
    }
}