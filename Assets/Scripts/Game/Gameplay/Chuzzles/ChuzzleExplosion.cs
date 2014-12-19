using UnityEngine;

namespace Assets.Game.Gameplay.Chuzzles
{
    internal class ChuzzleExplosion : MonoBehaviour
    {
        // public List<ExplosionDesc> explosionColors;

        public ParticleSystem ps;
        /*
    [Serializable]
    internal class ExplosionDesc
    {
        public ChuzzleColor color;
        public Color[] possibleColors;
    }
    */
        void Awake()
        {
            if (!ps)
            {
                ps = particleSystem;
            }
        }

        public void Init(ChuzzleColor color)
        {
            //var posibleColors = explosionColors.FirstOrDefault(x => x.color == color).possibleColors;
            //   ps.startColor = posibleColors[Random.Range(0, posibleColors.Length)];
            ps.Play(true);
        }
    }
}