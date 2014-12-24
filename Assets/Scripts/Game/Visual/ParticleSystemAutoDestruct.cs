using System.Collections;
using UnityEngine;

namespace Game.Visual
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemAutoDestruct : MonoBehaviour
    {
        [SerializeField] private bool _onlyDeactivate;

        void OnEnable()
        {
            StartCoroutine(Check());
        }
	
        IEnumerator Check()
        {
            while(true)
            {
                yield return new WaitForSeconds(0.5f);
                if(!particleSystem.IsAlive(true))
                {
                    if (_onlyDeactivate)
                    {
                        gameObject.SetActive(false);
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                    break;
                }
            }
        }
    }
}
