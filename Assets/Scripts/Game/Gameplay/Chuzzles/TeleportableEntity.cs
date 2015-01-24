using Game.Gameplay.Chuzzles.Utils;
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
                var chuzzle = GetComponent<Chuzzle>();
                var visualRoot = chuzzle.VisualRoot;
                Copy = Instantiate(visualRoot) as GameObject;
                Copy.gameObject.name += " is copy";
                Copy.transform.SetParent(visualRoot.transform.parent, false);
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