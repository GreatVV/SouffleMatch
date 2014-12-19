using Assets.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI
{
    [ExecuteInEditMode]
    public class MapId : MonoBehaviour
    {
        public int Index;
        public int Pack;
        public GameObject currentLevelParticle;
        private GameObject effect;

        #region Events Subscribers

        public void OnClick()
        {
            Instance.SessionRestorer.StartLevel(Pack, Index);
        }

        #endregion

        #region Unity Methods

        public void Awake()
        {
            name = "0";
        }

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            if (effect)
            {
                Destroy(effect);
            }
        }

        #endregion

        public void UpdateName()
        {
            transform.GetComponentInChildren<Text>().text = name;
        }

        public void ShowCurrent()
        {
            effect = (GameObject) Instantiate(currentLevelParticle);
            effect.transform.parent = transform;
            effect.transform.localPosition = Vector3.zero;
            effect.transform.localScale = Vector3.one;
            effect.particleSystem.renderer.sortingLayerName = LayerMask.LayerToName(5);
        }
    }
}