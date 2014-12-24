using System.Linq;
using UnityEngine;

namespace UI
{
    public class RandomRotationOnEnable : MonoBehaviour
    {

        [SerializeField] private Vector3[] randomRotations;
        void OnEnable()
        {
            if (randomRotations.Any())
            {
                transform.localRotation = Quaternion.Euler(randomRotations[Random.Range(0, randomRotations.Length)]);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-45, 45));
            }
        }
    }
}
