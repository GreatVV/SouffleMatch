using UnityEngine;
using System.Collections;

public class RandomRotationOnEnable : MonoBehaviour
{

    [SerializeField] private Vector3[] randomRotations;
    void OnEnable()
    {
        transform.localRotation = Quaternion.Euler(randomRotations[Random.Range(0, randomRotations.Length)]);
    }
}
