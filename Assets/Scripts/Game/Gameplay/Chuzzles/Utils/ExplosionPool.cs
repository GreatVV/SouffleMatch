using Plugins;
using UnityEngine;

namespace Game.Gameplay.Chuzzles.Utils
{
    public class ExplosionPool : MonoBehaviour
    {
        private static Camera EffectsCamera;

        [SerializeField]
        private ChuzzleExplosion explosionPrefab;

        public void Awake()
        {
            if (!EffectsCamera)
            {
                EffectsCamera = GameObject.Find("EffectCamera").camera;
            }
        }

        public void Explode(Chuzzle chuzzle)
        {
            var ps = GetExplosion();

            iTween.ScaleTo(chuzzle.gameObject,
                           iTween.Hash(
                                       "x", 0,
                                       "y", 0,
                                       "z", 0,
                                       "time", ps.particleSystem.duration));

            ps.transform.position = EffectsCamera.ScreenToWorldPoint(Camera.main.WorldToScreenPoint(chuzzle.transform.position));
            ps.Init(chuzzle.Color);
        }

        private ChuzzleExplosion GetExplosion()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (!child.gameObject.activeInHierarchy)
                {
                    child.gameObject.SetActive(true);
                    return child.GetComponent<ChuzzleExplosion>();
                }
            }

            var explosion = Instantiate(explosionPrefab) as ChuzzleExplosion;
            //  Debug.Log("Explosion: "+explosion);
            explosion.transform.parent = transform;
            return explosion.GetComponent<ChuzzleExplosion>();
        }
    }
}