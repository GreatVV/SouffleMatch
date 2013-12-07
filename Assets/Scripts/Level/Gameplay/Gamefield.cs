#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

[RequireComponent(typeof (Level))]
[RequireComponent(typeof (StageManager))]
[RequireComponent(typeof (Points))]
[RequireComponent(typeof (CheckSpecialState))]
[RequireComponent(typeof (CreateNewChuzzlesState))]
[RequireComponent(typeof (GameOverState))]
[RequireComponent(typeof (WinState))]
[RequireComponent(typeof (FieldState))]
[RequireComponent(typeof (RemoveCombinationState))]
[RequireComponent(typeof (InitState))]
public class Gamefield : MonoBehaviour
{
    public LayerMask ChuzzleMask;

    public GameMode GameMode;
    public bool IsPause;

    [HideInInspector] public Level Level;
    public int[] NewTilesInColumns;

    [HideInInspector] public Points PointSystem;

    [HideInInspector] public StageManager StageManager;

    #region State

    [HideInInspector] public CheckSpecialState CheckSpecialState = null;

    [HideInInspector] public CreateNewChuzzlesState CreateNewChuzzlesState = null;

    [HideInInspector] public FieldState FieldState = null;
    [HideInInspector] public GameOverState GameOverState = null;

    [HideInInspector] public InitState InitState = null;
    [HideInInspector] public RemoveCombinationState RemoveState = null;
    [HideInInspector] public WinState WinState = null;
    [HideInInspector] public WinRemoveCombinationState WinRemoveCombinationState = null;
    [HideInInspector] public WinCreateNewChuzzlesState WinCreateNewChuzzlesState = null;
    [HideInInspector] public WinCheckSpecialState WinCheckSpecialState = null;
    private GamefieldState _currentState;

    #endregion

    #region Events

    public event Action<List<Chuzzle>> CombinationDestroyed;

    public event Action<Gamefield> GameStarted;

    public event Action<Chuzzle> TileDestroyed;

    #endregion

    #region Event Handlers

    private void OnGameOver()
    {
        SwitchStateTo(GameOverState);
        Player.Instance.Lifes.SpentLife();
    }

    private void OnWin()
    {
        SwitchStateTo(WinState);
    }

    #endregion

    #region Event Invokators

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

    public void InvokeTileDestroyed(Chuzzle destroyedChuzzle)
    {
        if (TileDestroyed != null)
        {
            TileDestroyed(destroyedChuzzle);
        }
    }

    #endregion

    private static Gamefield Instance;
    public void Awake()
    {   
        Instance = this;
        InitState = GetComponent<InitState>();
        CheckSpecialState = GetComponent<CheckSpecialState>();
        CreateNewChuzzlesState = GetComponent<CreateNewChuzzlesState>();
        RemoveState = GetComponent<RemoveCombinationState>();
        GameOverState = GetComponent<GameOverState>();
        WinState = GetComponent<WinState>();
        FieldState = GetComponent<FieldState>();
        WinCheckSpecialState = GetComponent<WinCheckSpecialState>();
        WinCreateNewChuzzlesState = GetComponent<WinCreateNewChuzzlesState>();
        WinRemoveCombinationState = GetComponent<WinRemoveCombinationState>();


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
        if (chuzzle.NeedCreateNew)
        {
            NewTilesInColumns[chuzzle.Current.x]++;
        }
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

    void OnDestroy()
    {
        RemoveEventHandlers();
    }

    private void RemoveEventHandlers()
    {
        if (GameMode != null)
        {
            GameMode.Win -= OnWin;
            GameMode.GameOver -= OnGameOver;
        }
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
        Debug.Log("Old state: "+_currentState);
        _currentState.OnExit();
        _currentState = newState;
        Debug.Log("Switch to: " + _currentState);
        _currentState.OnEnter();
    }

    public static List<Chuzzle> Chuzzles
    {
        get
        {
            return Instance.Level.ActiveChuzzles;
        }
    }      
}