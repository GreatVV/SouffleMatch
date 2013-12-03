#region

using System;
using UnityEngine;

#endregion

[Serializable]
public class Pair
{
    [SerializeField] public ChuzzleColor ChuzzleColor;
    [SerializeField] public GameObject Prefab;
    [SerializeField] public PowerType Type;
}