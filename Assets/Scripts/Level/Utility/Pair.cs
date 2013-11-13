#region

using System;
using UnityEngine;

#endregion

[Serializable]
public class Pair
{
    [SerializeField] public ChuzzleType ChuzzleType;
    [SerializeField] public GameObject Prefab;
    [SerializeField] public PowerType Type;
}