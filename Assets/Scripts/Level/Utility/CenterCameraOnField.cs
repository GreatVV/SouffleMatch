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

    public Vector2 normalizedSize = new Vector2(0.8f, 0.8f);
    private Vector2 previousNormalizedSize;

    #region Event Handlers
    
    public void CenterCameraOnChuzzles(List<Chuzzle> targetChuzzles, bool instantly)
    {
        var minX = targetChuzzles.Min(x => x.transform.position.x);
        var minY = targetChuzzles.Min(x => x.transform.position.y);
        var maxX = targetChuzzles.Max(x => x.transform.position.x);
        var maxY = targetChuzzles.Max(x => x.transform.position.y);

        //var centerPosition = new Vector3((minX + maxX)/2,(minY + maxY)/2, DefaultPosition.z);
        var centerPosition = new Vector3(minX, maxY, DefaultPosition.z);
        /*Debug.Log("minx:"+ minX);
        Debug.Log("miny:"+ minY);
        Debug.Log("maxx:"+ maxX);
        Debug.Log("maxy:"+ maxY);
*/
        var fw = maxX - minX+1f;
        var fh = maxY - minY+1f;

        var fieldRatio = fw/fh;
        //Debug.Log("Field ratio: "+fieldRatio);
        //Debug.Log("Aspect: "+Camera.aspect);
        Camera.rect = new Rect(0,0,1,1);
        Camera.ResetAspect();
        var baseAspect = Camera.aspect;

        Camera.aspect = fieldRatio;
        Camera.orthographicSize = fh / 2;

        float height;


        height = baseAspect * normalizedSize.x / fieldRatio;
        

        Camera.rect = new Rect(
                       (1 - normalizedSize.x) / 2f,
                       (1 - height) / 2f,
                       normalizedSize.x,
                       height
                       );

        centerPosition = new Vector3(minX + Camera.aspect * Camera.orthographicSize - 0.5f,
                maxY - Camera.orthographicSize + 0.5f, DefaultPosition.z);

        if (instantly)
        {
            Camera.transform.position = centerPosition;

         /*   var chuzzle = Gamefield.Chuzzles.First(x => x.Current.Right != null && x.Current.Right.Top != null);
            Debug.Log("Pos: "+Camera.WorldToScreenPoint(chuzzle.transform.position));
            Debug.Log("Pos2:"+Camera.WorldToScreenPoint(chuzzle.Current.Right.Top.Sprite.transform.position));
            Debug.Log("Difference: " + (Camera.WorldToScreenPoint(chuzzle.transform.position) - Camera.WorldToScreenPoint(chuzzle.Current.Right.Top.Sprite.transform.position)));*/
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
        previousNormalizedSize = normalizedSize;
    }

    public void ToDefault()
    {
        Camera.transform.position = DefaultPosition;
    }

    void Update()
    {
        


        if (previousNormalizedSize != normalizedSize)
        {
            Instance.CenterCameraOnChuzzles(Gamefield.Chuzzles, false);
            previousNormalizedSize = normalizedSize;
        }
    }
}