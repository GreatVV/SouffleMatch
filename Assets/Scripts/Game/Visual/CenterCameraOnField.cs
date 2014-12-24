#region

using System.Collections.Generic;
using System.Linq;
using Game.Gameplay;
using Game.Gameplay.Chuzzles;
using Plugins;
using UnityEngine;

#endregion

namespace Game.Visual
{
    [ExecuteInEditMode]
    public class CenterCameraOnField : MonoBehaviour
    {
        public static CenterCameraOnField Instance;

        public Camera[] Cameras;
        public Vector3 DefaultPosition = new Vector3(240, 400, -10);

        public bool IsTweening;

        public Vector2 normalizedSize = new Vector2(0.8f, 0.8f);
        private Vector2 previousNormalizedSize;

        #region Event Handlers
    
        public void CenterCameraOnChuzzles(IEnumerable<Chuzzle> targetChuzzles, bool instantly)
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
            var currentCamera = Camera.main;
            //foreach (var currentCamera in Cameras)
            {
            

                //Debug.Log("Field ratio: "+fieldRatio);
                //Debug.Log("Aspect: "+Camera.aspect);
                currentCamera.rect = new Rect(0, 0, 1, 1);
                currentCamera.ResetAspect();
                var baseAspect = currentCamera.aspect;

                currentCamera.aspect = fieldRatio;
                currentCamera.orthographicSize = fh/2;

                if (baseAspect < 1)
                {
                    float height = baseAspect*normalizedSize.x/fieldRatio;

                    currentCamera.rect = new Rect(
                        (1 - normalizedSize.x)/2f,
                        (1 - height)/2f,
                        normalizedSize.x,
                        height
                        );
                }
                else
                {
                    float width = normalizedSize.y*fieldRatio/baseAspect;
                    currentCamera.rect = new Rect(
                        (1 - width)/2f,
                        (1 - normalizedSize.y)/2f,
                        width,
                        normalizedSize.y
                        );
                }

                centerPosition = new Vector3(minX + currentCamera.aspect*currentCamera.orthographicSize - 0.5f,
                    maxY - currentCamera.orthographicSize + 0.5f, DefaultPosition.z);

                if (instantly)
                {
                    currentCamera.transform.position = centerPosition;

                    /*   var chuzzle = Gamefield.Chuzzles.First(x => x.Current.Right != null && x.Current.Right.Top != null);
            Debug.Log("Pos: "+Camera.WorldToScreenPoint(chuzzle.transform.position));
            Debug.Log("Pos2:"+Camera.WorldToScreenPoint(chuzzle.Current.Right.Top.Sprite.transform.position));
            Debug.Log("Difference: " + (Camera.WorldToScreenPoint(chuzzle.transform.position) - Camera.WorldToScreenPoint(chuzzle.Current.Right.Top.Sprite.transform.position)));*/

                }
                else
                {


                    if (Vector3.Distance(currentCamera.transform.position, centerPosition) > 0.1f)
                    {
                        iTween.MoveTo(currentCamera.gameObject,
                            iTween.Hash("x", centerPosition.x, "y", centerPosition.y, "z", centerPosition.z, "time", 2f,
                                "oncomplete", "OnTweenComplete", "oncompletetarget",
                                gameObject, "oncompleteparams", centerPosition));
                        IsTweening = true;
                    }
                    else
                    {
                        currentCamera.transform.position = centerPosition;
                    }
                }
            }

            foreach (var tCamera in Cameras)
            {
                tCamera.transform.position = Camera.main.transform.position;
            }
        }

        private void OnTweenComplete(Vector3 targetPosition)
        {
            IsTweening = false;
        }

        void OnDestroy()
        {
            if (this == Instance)
            {
                Instance = null;
            }
        }

        #endregion

        public void Awake()
        {
            if (Instance)
            {
                Debug.Log("It's should be only one center camera on field");
                Destroy(this);
                return;
            }

            Instance = this;
            previousNormalizedSize = normalizedSize;

            Gamefield = FindObjectOfType<Gamefield>();
        }

        public Gamefield Gamefield { get; set; }

        public void ToDefault()
        {
            foreach (var currentCamera in Cameras)
            {
                currentCamera.transform.position = DefaultPosition;
            }
        }

        void Update()
        {
            if (previousNormalizedSize != normalizedSize)
            {
                Instance.CenterCameraOnChuzzles(Gamefield.Level.Chuzzles, false);
                previousNormalizedSize = normalizedSize;
            }
        }
    }
}