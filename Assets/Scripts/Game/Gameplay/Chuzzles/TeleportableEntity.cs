using Plugins;
using UnityEngine;

#region

#endregion

//[RequireComponent(typeof (Chuzzle))]
using Utils;

namespace Game.Gameplay.Chuzzles
{
    public class TeleportableEntity : MonoBehaviour
    {
        public GameObject Copy;

        #region Event Handlers

        public void OnDestroy()
        {
            Destroy(Copy);
        }

        #endregion

        public bool HasCopy
        {
            get { return Copy != null && Copy.activeSelf; }
        }

        public void Show()
        {
            if (Copy == null)
            {
                var chuzzle = gameObject.GetComponent<Chuzzle>();
                var prefab = Instance.TilesFactory.PrefabOfColor(chuzzle.Color);
                Copy = Instantiate(prefab) as GameObject;
                Copy.gameObject.name += " is copy";
                Copy.transform.parent = gameObject.transform;
                iTween.Stop(Copy);
                Destroy(Copy.GetComponent<Chuzzle>());
            }
        

            if (!Copy.activeSelf)
            {
                Copy.SetActive(true);
            }
        
        }

        public void Hide()
        {
            if (HasCopy)
            {
                Copy.SetActive(false);
            }
        }
    }
}