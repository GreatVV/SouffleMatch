#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

[RequireComponent(typeof(Gamefield))]
public abstract class GamefieldState : MonoBehaviour
{
    public List<Chuzzle> AnimatedChuzzles = new List<Chuzzle>();  
    void Awake()
    {
        Gamefield = GetComponent<Gamefield>();   
    }                
  
    public Gamefield Gamefield { get; private set; }

    #region Event Handlers

    public abstract void OnEnter();
    public abstract void OnExit();

    #endregion

    public abstract void UpdateState();
    public abstract void LateUpdateState();
}