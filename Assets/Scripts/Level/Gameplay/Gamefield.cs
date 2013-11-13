#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

[RequireComponent(typeof (Level))]
[RequireComponent(typeof (StageManager))]
[RequireComponent(typeof (Points))]
[RequireComponent(typeof (CheckSpecialState))]
[RequireComponent(typeof (CreateNewState))]
[RequireComponent(typeof (GameOverState))]
[RequireComponent(typeof (WinState))]
[RequireComponent(typeof (FieldState))]
[RequireComponent(typeof (RemoveCombinationState))]
[RequireComponent(typeof (InitState))]
public class Gamefield : MonoBehaviour
{
    public LayerMask ChuzzleMask;

    #region State

    private GamefieldState _currentState;

    [HideInInspector] public CheckSpecialState CheckSpecialState = null;

    [HideInInspector] public CreateNewState CreateNewState = null;

    [HideInInspector] public GameOverState GameOverState = null;
    
    [HideInInspector] public WinState WinState = null;

    [HideInInspector] public FieldState FieldState = null;

    [HideInInspector] public RemoveCombinationState RemoveState = null;

    [HideInInspector] public InitState InitState = null;

    #endregion

    public int[] NewTilesInColumns;

    public GameMode GameMode;

    [HideInInspector] public Level Level;

    [HideInInspector] public Points PointSystem;

    [HideInInspector] public StageManager StageManager;

    public bool IsPause;


    public event Action<List<Chuzzle>> CombinationDestroyed;

    public event Action<Gamefield> GameStarted;

    public event Action<Chuzzle> TileDestroyed;


    private void OnGameOver()
    {
        SwitchStateTo(GameOverState);
        Player.Instance.Lifes.SpentLife();
    }

    private void OnWin()
    {
        SwitchStateTo(WinState);
    }

    public virtual void InvokeCombinationDestroyed(List<Chuzzle> combination)
    {
        var handler = CombinationDestroyed;
        if (handler != null) handler(combination);
    }

    public virtual void InvokeGameStarted()
    {
        var handler = GameStarted;
        if (handler != null) handler(this);
    }

    private void InvokeTileDestroyed(Chuzzle destroyedChuzzle)
    {
        if (TileDestroyed != null)
        {
            TileDestroyed(destroyedChuzzle);
        }
    }


    public void Awake()
    {
        InitState = GetComponent<InitState>();
        CheckSpecialState = GetComponent<CheckSpecialState>();
        CreateNewState = GetComponent<CreateNewState>();
        RemoveState = GetComponent<RemoveCombinationState>();
        GameOverState = GetComponent<GameOverState>();
        WinState = GetComponent<WinState>();
        FieldState = GetComponent<FieldState>();

        Level = GetComponent<Level>();
        StageManager = GetComponent<StageManager>();
        PointSystem = GetComponent<Points>();
    }


    private void LateUpdate()
    {
        if (_currentState != null && !IsPause)
        {
            _currentState.LateUpdateState();
        }
    }


    /// <summary>
    ///     Remove chuzzle from game logic and add new tiles in column
    /// </summary>
    /// <param name="chuzzle">Chuzzle to remove</param>
    /// <param name="invokeEvent">Need to invoke event or not</param>
    public void RemoveChuzzle(Chuzzle chuzzle, bool invokeEvent = true)
    {
        Level.Chuzzles.Remove(chuzzle);
        if (Level.ActiveChuzzles.Contains(chuzzle))
        {
            Level.ActiveChuzzles.Remove(chuzzle);
        }
        NewTilesInColumns[chuzzle.Current.x]++;
        if (invokeEvent)
        {
            InvokeTileDestroyed(chuzzle);
        }
    }

    public void StartGame(SerializedLevel level = null)
    {
        Player.Instance.LastPlayedLevel = level;
        _currentState = InitState;
        _currentState.OnEnter();
    }

    public void AddEventHandlers()
    {
        RemoveEventHandlers();
        GameMode.Win += OnWin;
        GameMode.GameOver += OnGameOver;
    }

    private void RemoveEventHandlers()
    {
        GameMode.Win -= OnWin;
        GameMode.GameOver -= OnGameOver;
    }

    private void Update()
    {
        if (_currentState != null && !IsPause)
        {
            _currentState.UpdateState();
        }
    }

    public void SwitchStateTo(GamefieldState newState)
    {
        _currentState.OnExit();
        _currentState = newState;
        Debug.Log("Switch to: " + _currentState);
        _currentState.OnEnter();
    }
}